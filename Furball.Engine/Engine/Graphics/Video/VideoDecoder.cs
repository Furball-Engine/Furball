/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Originally written by Beyley Thomas <ep1cm1n10n123@gmail.com>
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using FFmpeg.AutoGen;
using Kettu;
using Silk.NET.Core.Native;
using static FFmpeg.AutoGen.ffmpeg;

namespace Furball.Engine.Engine.Graphics.Video;

// ReSharper disable UnusedMember.Local
public unsafe class VideoDecoder : IDisposable {
    private FFmpegFrame         AvFrame;
    private FFmpegFrame         RenderFrame;
    private FFmpegSwsContext    ConvertContext;
    private FFmpegPacket        Packet;
    private FFmpegStream        VideoStream;
    private FFmpegFormatContext FormatContext;
    private FFmpegCodecContext  CodecContext;
    private FFmpegCodec         Decoder;
    private int                 VideoStreamIndex;
    private byte*               InternalBuffer;
    private AVIOContext*        AvioContext;

    private Stream _fileStream;

    public double FrameDelay;
    public double CurrentDisplayTime { get; private set; } = 0d;

    public double Length {
        get {
            long duration = this.VideoStream.Stream->duration;
            if (duration < 0) return 36000000;
            return duration * this.FrameDelay;
        }
    }

    public  int    Width       => this.CodecContext.CodecContext->width;
    public  int    Height      => this.CodecContext.CodecContext->height;
    private double StartTimeMs => 1000 * this.VideoStream.Stream->start_time * this.FrameDelay;

    private readonly Thread _decodingThread;
    private          bool   _isDisposed = false;

    private readonly double[] _frameBufferTimes;
    private readonly byte[][] _frameBuffer;
    private readonly int      _bufferSize;
    private          bool     _videoLoaded = false;
    private          bool     _runDecoding = true;
    private          int      _writeCursor;
    private          int      _readCursor;
    private          long     _lastPts;

    public VideoDecoder(int bufferSize) {
        //Having too small of a buffer size causes issues
        this._bufferSize = Math.Max(4, bufferSize);

        this._frameBufferTimes = new double[this._bufferSize];
        this._frameBuffer      = new byte[this._bufferSize][];

        this._decodingThread = new Thread(this.DecodingRun);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            RootPath = "/usr/lib";
    }

    public void Load(string path) {
        this.Load(File.OpenRead(path));
    }

    private avio_alloc_context_read_packet readPacketCallback;
    private avio_alloc_context_seek        seekCallback;

    private int ReadPacketCallbackDef(void* opaque, byte* bufferPtr, int bufferSize) {
        byte[] arr  = new byte[bufferSize];
        int    read = this._fileStream.Read(arr, 0, bufferSize);

        fixed (void* ptr = arr) {
            Buffer.MemoryCopy(ptr, bufferPtr, bufferSize, read);
        }

        return read;
    }

    private enum SeekType {
        SeekSet,
        SeekCur,
        SeekEnd
    }

    private long StreamSeekCallbackDef(void* opaque, long offset, int whence) {
        if (!this._fileStream.CanSeek)
            throw new InvalidOperationException("Tried seeking on a video sourced by a non-seekable stream.");

        switch (whence) {
            case (int)SeekType.SeekCur:
                this._fileStream.Seek(offset, SeekOrigin.Current);
                break;
            case (int)SeekType.SeekEnd:
                this._fileStream.Seek(offset, SeekOrigin.End);
                break;
            case (int)SeekType.SeekSet:
                this._fileStream.Seek(offset, SeekOrigin.Begin);
                break;
            case AVSEEK_SIZE:
                return this._fileStream.Length;
            default:
                return -1;
        }

        return this._fileStream.Position;
    }

    public void Load(Stream stream) {
        if (this._videoLoaded)
            throw new InvalidOperationException("Unable to load 2 videos with the same VideoDecoder!");

        this._videoLoaded = true;
        this._fileStream  = stream;

        try {
            Logger.Log($"AV version info {av_version_info()}", VideoDecoderLoggerLevel.InstanceInfo);
        }
        catch {
            Logger.Log("Unable to get AV version info!", VideoDecoderLoggerLevel.InstanceError);

            throw new NotSupportedException("Video decoding seems to not be supported on your platform!");
        }

        const int contextBufferSize = 4096;
        byte*     contextBuffer     = (byte*)av_malloc(contextBufferSize);

        this.readPacketCallback = this.ReadPacketCallbackDef;
        this.seekCallback       = this.StreamSeekCallbackDef;

        AVIOContext* ioContext = avio_alloc_context(contextBuffer, contextBufferSize, 0, (void*)0, this.readPacketCallback, null, this.seekCallback);

        AVFormatContext* formatContext = avformat_alloc_context();
        formatContext->pb    =  ioContext;
        formatContext->flags |= AVFMT_FLAG_GENPTS;

        // Open video file
        if (avformat_open_input(&formatContext, "dummy", null, null) < 0)
            Logger.Log("Failed to open input file!", VideoDecoderLoggerLevel.InstanceError);
        // throw new Exception($"Failed to open input file {path}!");
        this.FormatContext = new FFmpegFormatContext(formatContext);

        // find stream info
        if (avformat_find_stream_info(this.FormatContext.FormatContext, null) < 0) {
            Logger.Log("Failed to get stream info!", VideoDecoderLoggerLevel.InstanceError);

            throw new Exception("Failed to get stream info!");
        }

        // av_dump_format(this.FormatContext.FormatContext, 0, path, 0);

        for (int i = 0; i < this.FormatContext.FormatContext->nb_streams; ++i)
            if (this.FormatContext.FormatContext->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO) {
                this.VideoStreamIndex = i;
                break;
            }

        if (this.VideoStreamIndex == -1) {
            Logger.Log("Unable to find a video stream in the file!", VideoDecoderLoggerLevel.InstanceError);

            throw new KeyNotFoundException("Unable to find a video stream in the file!");
        }

        //Get the video stream
        this.VideoStream = new FFmpegStream(this.FormatContext.FormatContext->streams[this.VideoStreamIndex]);

        //Create and fill the codec context
        this.CodecContext = new FFmpegCodecContext(avcodec_alloc_context3(null));
        avcodec_parameters_to_context(this.CodecContext.CodecContext, this.VideoStream.Stream->codecpar);

        //Find a decoder
        this.Decoder = new FFmpegCodec(avcodec_find_decoder(this.CodecContext.CodecContext->codec_id));
        if (this.Decoder.Codec == null) {
            Logger.Log("Failed to find decoder!", VideoDecoderLoggerLevel.InstanceError);

            throw new Exception("Failed to find decoder!");
        }

        //Open the codec
        if (avcodec_open2(this.CodecContext.CodecContext, this.Decoder.Codec, null) < 0) {
            Logger.Log("Failed to open codec!", VideoDecoderLoggerLevel.InstanceError);

            throw new Exception("Failed to open codec!");
        }

        // allocate the video frames
        this.AvFrame     = new FFmpegFrame();
        this.RenderFrame = new FFmpegFrame();
        //Get buffer size
        int size = av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_RGBA, this.CodecContext.CodecContext->width, this.CodecContext.CodecContext->height, 1);
        //Allocate internal buffer
        this.InternalBuffer = (byte*)av_malloc((ulong)(size * sizeof(byte)));

        byte_ptrArray4 dataArray4     = new byte_ptrArray4();
        int_array4     lineSizeArray4 = new int_array4();
        dataArray4.UpdateFrom(this.RenderFrame.Frame->data);
        lineSizeArray4.UpdateFrom(this.RenderFrame.Frame->linesize);

        //Fill the internal buffer
        av_image_fill_arrays(
        ref dataArray4,
        ref lineSizeArray4,
        this.InternalBuffer,
        AVPixelFormat.AV_PIX_FMT_RGBA,
        this.CodecContext.CodecContext->width,
        this.CodecContext.CodecContext->height,
        1
        );
        this.RenderFrame.Frame->data.UpdateFrom(dataArray4);
        this.RenderFrame.Frame->linesize.UpdateFrom(lineSizeArray4);

        //Allocate the packet
        this.Packet = new FFmpegPacket(av_packet_alloc());

        if (this.Packet.Packet == null) {
            Logger.Log("Allocating packet failed!", VideoDecoderLoggerLevel.InstanceError);

            throw new Exception("Allocating packet failed!");
        }

        this.FrameDelay = av_q2d(this.VideoStream.Stream->time_base);

        for (int i = 0; i < this._bufferSize; i++)
            this._frameBuffer[i] = new byte[this.Width * this.Height * 4];

        this._decodingThread.Start();
    }

    private string DescribeError(int err) {
        byte* temp = (byte*)SilkMarshal.Allocate(AV_ERROR_MAX_STRING_SIZE);

        av_make_error_string(temp, AV_ERROR_MAX_STRING_SIZE, err);

        string s = SilkMarshal.PtrToString((nint)temp);

        SilkMarshal.Free((nint)temp);

        return s;
    }

    private readonly ConcurrentQueue<Delegate> _commandQueue = new ConcurrentQueue<Delegate>();

    private void DecodingRun() {
        try {
            bool gotNewFrame;

            while (this._runDecoding) {
                gotNewFrame = false;

                if (this._commandQueue.TryDequeue(out Delegate result))
                    result.DynamicInvoke();

                lock (this) {
                    while (this._writeCursor - this._readCursor < this._bufferSize && av_read_frame(this.FormatContext.FormatContext, this.Packet.Packet) >= 0) {
                        if (!this._runDecoding)
                            return;

                        if (this.Packet.Packet->stream_index == this.VideoStreamIndex) {
                            bool frameFinished = false;

                            int response;
                            if (this.CodecContext.CodecContext->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO ||
                                this.CodecContext.CodecContext->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO) {
                                response = avcodec_send_packet(this.CodecContext.CodecContext, this.Packet.Packet);
                                if (response < 0 && response != AVERROR(EAGAIN) && response != AVERROR(AVERROR_EOF)) {} else {
                                    if (response >= 0)
                                        this.Packet.Packet->size = 0;
                                    response = avcodec_receive_frame(this.CodecContext.CodecContext, this.AvFrame.Frame);
                                    if (response == AVERROR(EAGAIN) || response == AVERROR(AVERROR_EOF))
                                        response = 0;
                                    if (response >= 0)
                                        frameFinished = true;
                                }
                            }

                            if (this.Packet.Packet->dts < this._seekingTo)
                                continue;

                            this._seekingTo = 0;

                            if (frameFinished && this.Packet.Packet->data != null) {
                                if (this.ConvertContext == null)
                                    this.ConvertContext = new FFmpegSwsContext(
                                    sws_getContext(
                                    this.CodecContext.CodecContext->width,
                                    this.CodecContext.CodecContext->height,
                                    this.CodecContext.CodecContext->pix_fmt,
                                    this.CodecContext.CodecContext->width,
                                    this.CodecContext.CodecContext->height,
                                    AVPixelFormat.AV_PIX_FMT_RGBA,
                                    0,
                                    null,
                                    null,
                                    null
                                    )
                                    );

                                sws_scale(
                                this.ConvertContext.SwsContext,
                                this.AvFrame.Frame->data,
                                this.AvFrame.Frame->linesize,
                                0,
                                this.CodecContext.CodecContext->height,
                                this.RenderFrame.Frame->data,
                                this.RenderFrame.Frame->linesize
                                );

                                Marshal.Copy(
                                (IntPtr)this.RenderFrame.Frame->data[0],
                                this._frameBuffer[this._writeCursor % this._bufferSize],
                                0,
                                this._frameBuffer[this._writeCursor % this._bufferSize].Length
                                );

                                this._frameBufferTimes[this._writeCursor % this._bufferSize] =
                                    (this.Packet.Packet->dts - this.VideoStream.Stream->start_time) * this.FrameDelay * 1000;

                                this._writeCursor++;

                                this._lastPts = this.Packet.Packet->dts;

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
        while (this._readCursor < this._writeCursor - 1 && this._frameBufferTimes[(this._readCursor + 1) % this._bufferSize] <= time)
            this._readCursor++;

        if (this._readCursor < this._writeCursor) {
            this.CurrentDisplayTime = this._frameBufferTimes[this._readCursor % this._bufferSize];
            return this._frameBuffer[this._readCursor % this._bufferSize];
        }

        return null;
    }

    private long _seekingTo;
    public void Seek(double time) {
        lock (this) {
            Logger.Log($"Seeking to {time}ms", VideoDecoderLoggerLevel.InstanceInfo);

            this._commandQueue.Enqueue(
            new Action(
            delegate {
                int    flags     = 0;
                double timestamp = time / 1000 / this.FrameDelay + this.VideoStream.Stream->start_time;

                if (timestamp < this._lastPts)
                    flags = AVSEEK_FLAG_BACKWARD;
                avformat_seek_file(this.FormatContext.FormatContext, this.VideoStreamIndex, 0, (long)timestamp, long.MaxValue, flags);
                this._seekingTo   = (long)timestamp;
                this._readCursor  = 0;
                this._writeCursor = 0;
            }
            )
            );
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

        if (this._decodingThread.IsAlive)
            this._decodingThread.Join();
        
        this._fileStream.Dispose();

        // AppData data = this._data;

        //Clean up after ourselves
        // avformat_close_input(&data.FormatContext);

        // if (data.AvFrame        != null) av_free(data.AvFrame);
        // if (data.RenderFrame    != null) av_free(data.RenderFrame);
        // if (data.Packet         != null) av_free(data.Packet);
        // if (data.CodecContext   != null) avcodec_close(data.CodecContext);
        // if (data.FormatContext  != null) avformat_free_context(data.FormatContext);
        // if (data.ConvertContext != null) sws_freeContext(data.ConvertContext);
    }
}

public class VideoDecoderLoggerLevel : LoggerLevel {
    private enum ChannelEnum {
        Info,
        Warning,
        Error
    }

    public static VideoDecoderLoggerLevel InstanceInfo    = new VideoDecoderLoggerLevel(ChannelEnum.Info);
    public static VideoDecoderLoggerLevel InstanceWarning = new VideoDecoderLoggerLevel(ChannelEnum.Warning);
    public static VideoDecoderLoggerLevel InstanceError   = new VideoDecoderLoggerLevel(ChannelEnum.Error);

    private VideoDecoderLoggerLevel(ChannelEnum @enum) => this.Channel = @enum.ToString();
}
