using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using FFmpeg.AutoGen;
using Kettu;
using static FFmpeg.AutoGen.ffmpeg;

namespace Furball.Engine.Engine.Graphics {
    public unsafe class VideoDecoder : IDisposable {
        private struct AppData {
            public AVFormatContext* FormatContext;
            public int              VideoStreamIndex;
            public AVStream*        VideoStream;
            public AVCodecContext*  CodecContext;
            public AVCodec*         Decoder;
            public AVFrame*         AVFrame;
            public AVFrame*         RenderFrame;
            public byte*            InternalBuffer;
            public AVPacket*        Packet;
            public SwsContext*      ConvertContext;

            public AppData(bool fuckDotnet5) {
                this.FormatContext    = null;
                this.VideoStreamIndex = -1;
                this.VideoStream      = null;
                this.CodecContext     = null;
                this.Decoder          = null;
                this.AVFrame          = null;
                this.RenderFrame      = null;
                this.InternalBuffer   = null;
                this.Packet           = null;
                this.ConvertContext   = null;
            }
        }

        private AppData Data;

        public double FrameDelay;
        public double CurrentDisplayTime { get; private set; } = 0d;

        public double Length {
            get {
                long duration = this.Data.VideoStream->duration;
                if (duration < 0) return 36000000;
                return duration * this.FrameDelay;
            }
        }

        public  int    Width       => this.Data.CodecContext->width;
        public  int    Height      => this.Data.CodecContext->height;
        private double StartTimeMs => 1000 * this.Data.VideoStream->start_time * this.FrameDelay;

        private readonly Thread _decodingThread;
        private          bool   _isDisposed = false;

        private readonly double[] _frameBufferTimes;
        private readonly byte[][] _frameBuffer;
        private readonly int      _bufferSize;
        private          bool     _videoLoaded = false;
        private          bool     _runDecoding = true;
        private          int      writeCursor;
        private          int      readCursor;
        private          long     lastPts;

        public VideoDecoder(int bufferSize) {
            this._bufferSize = bufferSize;

            this._frameBufferTimes = new double[this._bufferSize];
            this._frameBuffer      = new byte[this._bufferSize][];

            this._decodingThread = new Thread(this.DecodingRun);
        }

        public void Load(string path) {
            if (this._videoLoaded)
                throw new InvalidOperationException("Unable to load 2 videos with the same VideoDecoder!");

            this._videoLoaded = true;

            try {
                Logger.Log($"AV version info {av_version_info()}", VideoDecoderLoggerLevel.InstanceInfo);
            }
            catch {
                Logger.Log("Unable to get AV version info!", VideoDecoderLoggerLevel.InstanceError);

                throw new NotSupportedException("Video decoding seems to not be supported on your platform!");
            }

            AppData data = new(true);

            // open video
            if (avformat_open_input(&data.FormatContext, path, null, null) < 0) {
                Logger.Log($"Failed to open input file {path}!", VideoDecoderLoggerLevel.InstanceError);

                throw new Exception($"Failed to open input file {path}!");
            }

            // find stream info
            if (avformat_find_stream_info(data.FormatContext, null) < 0) {
                Logger.Log("Failed to get stream info!", VideoDecoderLoggerLevel.InstanceError);

                throw new Exception("Failed to get stream info!");
            }

            av_dump_format(data.FormatContext, 0, path, 0);

            for (int i = 0; i < data.FormatContext->nb_streams; ++i)
                if (data.FormatContext->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO) {
                    data.VideoStreamIndex = i;
                    break;
                }

            if (data.VideoStreamIndex == -1) {
                Logger.Log("Unable to find a video stream in the file!", VideoDecoderLoggerLevel.InstanceError);

                throw new KeyNotFoundException("Unable to find a video stream in the file!");
            }

            //Get the video stream
            data.VideoStream = data.FormatContext->streams[data.VideoStreamIndex];

            //Create and fill the codec context
            data.CodecContext = avcodec_alloc_context3(null);
            avcodec_parameters_to_context(data.CodecContext, data.VideoStream->codecpar);

            //Find a decoder
            data.Decoder = avcodec_find_decoder(data.CodecContext->codec_id);
            if (data.Decoder == null) {
                Logger.Log("Failed to find decoder!", VideoDecoderLoggerLevel.InstanceError);

                throw new Exception("Failed to find decoder!");
            }

            //Open the codec
            if (avcodec_open2(data.CodecContext, data.Decoder, null) < 0) {
                Logger.Log("Failed to open codec!", VideoDecoderLoggerLevel.InstanceError);

                throw new Exception("Failed to open codec!");
            }

            // allocate the video frames
            data.AVFrame     = av_frame_alloc();
            data.RenderFrame = av_frame_alloc();
            //Get buffer size
            int size = av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_RGBA, data.CodecContext->width, data.CodecContext->height, 1);
            //Allocate internal buffer
            data.InternalBuffer = (byte*)av_malloc((ulong)(size * sizeof(byte)));

            byte_ptrArray4 data_array4     = new();
            int_array4     lineSize_array4 = new();
            data_array4.UpdateFrom(data.RenderFrame->data);
            lineSize_array4.UpdateFrom(data.RenderFrame->linesize);

            //Fill the internal buffer
            av_image_fill_arrays(
            ref data_array4,
            ref lineSize_array4,
            data.InternalBuffer,
            AVPixelFormat.AV_PIX_FMT_RGBA,
            data.CodecContext->width,
            data.CodecContext->height,
            1
            );
            data.RenderFrame->data.UpdateFrom(data_array4);
            data.RenderFrame->linesize.UpdateFrom(lineSize_array4);

            //Allocate the packet
            data.Packet = av_packet_alloc();

            if (data.Packet == null) {
                Logger.Log("Allocating packet failed!", VideoDecoderLoggerLevel.InstanceError);

                throw new Exception("Allocating packet failed!");
            }

            this.FrameDelay = av_q2d(data.VideoStream->time_base);

            this.Data = data;

            for (int i = 0; i < this._bufferSize; i++)
                this._frameBuffer[i] = new byte[this.Width * this.Height * 4];

            this._decodingThread.Start();
        }

        private void DecodingRun() {
            try {
                bool gotNewFrame;

                while (this._runDecoding) {
                    gotNewFrame = false;

                    lock (this) {
                        while (this.writeCursor - this.readCursor < this._bufferSize && av_read_frame(this.Data.FormatContext, this.Data.Packet) >= 0) {
                            if (!this._runDecoding)
                                return;

                            if (this.Data.Packet->stream_index == this.Data.VideoStreamIndex) {
                                bool frameFinished = false;

                                int response;
                                if (this.Data.CodecContext->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO ||
                                    this.Data.CodecContext->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO) {
                                    response = avcodec_send_packet(this.Data.CodecContext, this.Data.Packet);
                                    if (response < 0 && response != AVERROR(EAGAIN) && response != AVERROR_EOF) {} else {
                                        if (response >= 0)
                                            this.Data.Packet->size = 0;
                                        response = avcodec_receive_frame(this.Data.CodecContext, this.Data.AVFrame);
                                        if (response >= 0)
                                            frameFinished = true;
                                        //if (response == AVERROR(EAGAIN) || response == AVERROR_EOF)
                                        //response = 0;
                                    }
                                }

                                if (frameFinished && this.Data.Packet->data != null) {
                                    if (this.Data.ConvertContext == null)
                                        this.Data.ConvertContext = sws_getContext(
                                        this.Data.CodecContext->width,
                                        this.Data.CodecContext->height,
                                        this.Data.CodecContext->pix_fmt,
                                        this.Data.CodecContext->width,
                                        this.Data.CodecContext->height,
                                        AVPixelFormat.AV_PIX_FMT_RGBA,
                                        SWS_BICUBIC,
                                        null,
                                        null,
                                        null
                                        );

                                    sws_scale(
                                    this.Data.ConvertContext,
                                    this.Data.AVFrame->data,
                                    this.Data.AVFrame->linesize,
                                    0,
                                    this.Data.CodecContext->height,
                                    this.Data.RenderFrame->data,
                                    this.Data.RenderFrame->linesize
                                    );

                                    Marshal.Copy(
                                    (IntPtr)this.Data.RenderFrame->data[0],
                                    this._frameBuffer[this.writeCursor % this._bufferSize],
                                    0,
                                    this._frameBuffer[this.writeCursor % this._bufferSize].Length
                                    );

                                    this._frameBufferTimes[this.writeCursor % this._bufferSize] =
                                        (this.Data.Packet->dts - this.Data.VideoStream->start_time) * this.FrameDelay * 1000;

                                    this.writeCursor++;

                                    this.lastPts = this.Data.Packet->dts;

                                    gotNewFrame = true;
                                }
                            }
                        }
                    }

                    if (!gotNewFrame)
                        Thread.Sleep(15);
                }
            }
            catch (Exception e) {
                File.WriteAllText("video-error.log", e.ToString());
            }
        }

        /// <summary>
        ///     Decodes one frame.
        ///     Returns null if decoding full video has finished.
        /// </summary>
        /// <returns></returns>
        public byte[] GetFrame(int time) {
            while (this.readCursor < this.writeCursor - 1 && this._frameBufferTimes[(this.readCursor + 1) % this._bufferSize] <= time)
                this.readCursor++;

            if (this.readCursor < this.writeCursor) {
                this.CurrentDisplayTime = this._frameBufferTimes[this.readCursor % this._bufferSize];
                return this._frameBuffer[this.readCursor % this._bufferSize];
            }

            return null;
        }

        public void Seek(int time) {
            lock (this) {
                int    flags     = 0;
                double timestamp = (double)time / 1000 / this.FrameDelay + this.Data.VideoStream->start_time;

                if (timestamp < this.lastPts)
                    flags = AVSEEK_FLAG_BACKWARD;
                av_seek_frame(this.Data.FormatContext, this.Data.VideoStreamIndex, (long)timestamp, flags);
                this.readCursor  = 0;
                this.writeCursor = 0;
            }
        }

        ~VideoDecoder() {
            this.Dispose();
        }

        public void Dispose() {
            if (this._isDisposed)
                return;
            this._isDisposed = true;

            this._runDecoding = false;

            this._decodingThread.Join();

            AppData data = this.Data;

            //Clean up after ourselves
            avformat_close_input(&data.FormatContext);

            if (data.AVFrame       != null) av_free(data.AVFrame);
            if (data.RenderFrame   != null) av_free(data.RenderFrame);
            if (data.Packet        != null) av_free(data.Packet);
            if (data.CodecContext  != null) avcodec_close(data.CodecContext);
            if (data.FormatContext != null) avformat_free_context(data.FormatContext);
        }
    }

    public class VideoDecoderLoggerLevel : LoggerLevel {
        private enum ChannelEnum {
            Info,
            Warning,
            Error
        }

        public static VideoDecoderLoggerLevel InstanceInfo    = new(ChannelEnum.Info);
        public static VideoDecoderLoggerLevel InstanceWarning = new(ChannelEnum.Warning);
        public static VideoDecoderLoggerLevel InstanceError   = new(ChannelEnum.Error);

        private VideoDecoderLoggerLevel(ChannelEnum @enum) => this.Channel = @enum.ToString();
    }
}
