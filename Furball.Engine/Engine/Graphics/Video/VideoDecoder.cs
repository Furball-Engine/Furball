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
using JetBrains.Annotations;
using Kettu;
using Silk.NET.Core.Native;
using static FFmpeg.AutoGen.ffmpeg;

namespace Furball.Engine.Engine.Graphics.Video;

/*
 * Load():
 *  File loaded
 *  Decoders detected
 *  Codec chosen according to HW decoder options and input file
 *  Decoding thread started
 *
 * Decoding Thread Run():
 *  while running:
 *   \ Dequeue and invoke any commands sent to the command queue
 *     loop to make sure we are always _bufferSize frames ahead of the read cursor position
 *      \ Read the next frame packet
 *        Send the frame packet to the decoder
 *        Try to recieve the AVFrame for said packet
 *        If seeking, and we arent at the wanted frame, `continue` (do nothing and try the next packet)
 *        If we have finished recieving a whole AVFrame from the current packet
 *         \ If the AVFrame uses a hardware format, transfer it to CPU memory
 *           Convert the AVFrame to RGBA format (8 bits per channel)
 *           Copy the frame to the respective frame buffer for the write position
 *           Write the timestamp of the frame to the framebuffer times array
 *           Increment the write cursor
 *
 * GetFrame(time):
 *  while the read cursor is behind the write cursor and the time of the frame is less than `time`
 *   \ increment read cursor
 *  if we are behind the write cursor, then we are at a completely new frame, and return it
 *  else return null, which means we are at a frame we have already returned before
 */

// ReSharper disable UnusedMember.Local
public unsafe class VideoDecoder : IDisposable {
    private readonly int _bufferSize;

    private readonly ConcurrentQueue<Delegate> _commandQueue = new ConcurrentQueue<Delegate>();

    private readonly Thread   _decodingThread;
    private readonly byte[][] _frameBuffer;

    private readonly double[] _frameBufferTimes;

    private Stream _fileStream;
    private bool   _isDisposed = false;
    private long   _lastPts;
    private int    _readCursor;
    private bool   _runDecoding = true;

    private long _seekingTo;
    private bool _videoLoaded = false;
    private int  _writeCursor;
    /// <summary>
    /// Intermediary frame used just before copying data to the frame buffer (separate words :^)
    /// </summary>
    private FFmpegFrame AvFrame;
    /// <summary>
    /// Staging frame used when we have to copy from a HW frame to a HW accessible CPU frame
    /// </summary>
    private FFmpegFrame AvStagingFrame;
    private AVIOContext*        AvioContext;
    public  FFmpegCodecContext  CodecContext;
    public  FFmpegCodec         Codec;
    public  AVHWDeviceType      HwCodecType;
    private FFmpegSwsContext    ConvertContext;
    private FFmpegCodec         Decoder;
    private FFmpegFormatContext FormatContext;

    public  double       FrameDelay;
    private byte*        InternalBuffer;
    private FFmpegPacket Packet;

    private avio_alloc_context_read_packet readPacketCallback;
    private avio_alloc_context_seek        seekCallback;
    
    private FFmpegFrame                    RenderFrame;
    private FFmpegStream                   VideoStream;
    private int                            VideoStreamIndex;

    public VideoDecoder(int bufferSize) {
        //Having too small of a buffer size causes issues
        this._bufferSize = Math.Max(4, bufferSize);

        this._frameBufferTimes = new double[this._bufferSize];
        this._frameBuffer      = new byte[this._bufferSize][];

        this._decodingThread = new Thread(this.DecodingRun);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            RootPath = "/usr/lib";
    }
    public double CurrentDisplayTime { get; private set; } = 0d;

    public double Length {
        get {
            long duration = this.VideoStream.Stream->duration;
            if (duration < 0) return 36000000;
            return duration * this.FrameDelay;
        }
    }

    public int Width => this.CodecContext.CodecContext->width;
    public int Height => this.CodecContext.CodecContext->height;
    private double StartTimeMs => 1000 * this.VideoStream.Stream->start_time * this.FrameDelay;

    public void Load(string path, HardwareDecoderType wantedDecoderTypes) {
        this.Load(File.OpenRead(path), wantedDecoderTypes);
    }

    private byte[]   _readBuf = new byte[128];
    private GCHandle _gcHandle;
    private static int ReadPacketCallbackDef(void* opaque, byte* bufferPtr, int bufferSize) {
        GCHandle handle = GCHandle.FromIntPtr((IntPtr)opaque);

        if (!handle.IsAllocated)
            throw new Exception();

        VideoDecoder @this = (VideoDecoder)handle.Target;
        
        #if NETSTANDARD2_1_OR_GREATER
        return @this._fileStream.Read(new Span<byte>(bufferPtr, bufferSize));
        #else
        //Make sure the read buffer is large enough
        if (@this._readBuf.Length < bufferSize)
            @this._readBuf = new byte[bufferSize];
        //Read into the read buffer
        int read = @this._fileStream.Read(@this._readBuf, 0, bufferSize);

        //Copy the memory into FFmpeg's pointer
        fixed (void* ptr = @this._readBuf) {
            Buffer.MemoryCopy(ptr, bufferPtr, bufferSize, read);
        }

        return read;
        #endif
    }

    private static long StreamSeekCallbackDef(void* opaque, long offset, int whence) {
        GCHandle handle = GCHandle.FromIntPtr((IntPtr)opaque);

        if (!handle.IsAllocated)
            throw new Exception();

        VideoDecoder @this = (VideoDecoder)handle.Target;
 
        if (!@this._fileStream.CanSeek)
            throw new InvalidOperationException("Tried seeking on a video sourced by a non-seekable stream.");

        switch (whence) {
            case (int)SeekType.Current:
                @this._fileStream.Seek(offset, SeekOrigin.Current);
                break;
            case (int)SeekType.End:
                @this._fileStream.Seek(offset, SeekOrigin.End);
                break;
            case (int)SeekType.Set:
                @this._fileStream.Seek(offset, SeekOrigin.Begin);
                break;
            case AVSEEK_SIZE:
                return @this._fileStream.Length;
            default:
                return -1;
        }

        return @this._fileStream.Position;
    }

    private List<(AVHWDeviceType, FFmpegCodec)> GetPossibleDecoders(AVCodecID codecId, HardwareDecoderType wantedHwDecodingType) {
        AVHWDeviceTypePerformanceComparer comparer = new AVHWDeviceTypePerformanceComparer();
        List<(AVHWDeviceType, FFmpegCodec)> list = new List<(AVHWDeviceType, FFmpegCodec)>();

        //This is the first matching codec found, which should aloways be the SW fallback
        FFmpegCodec first = null;

        void* itr = null;
        FFmpegCodec codec = null;

        while ((codec = new FFmpegCodec(av_codec_iterate(&itr))).Codec != null) {
            //If the ID doesnt match or its not a decoder, skip it 
            if (codec.Codec->id != codecId || av_codec_is_decoder(codec.Codec) == 0)
                continue;

            //Set the first codec if it is not set
            first ??= codec;

            //If we dont want hardware decoding, break out, as we found a SW decoder
            if (wantedHwDecodingType == HardwareDecoderType.None)
                break;

            //Iterate through the supported hardware devices of the codec
            foreach (AVHWDeviceType avhwDeviceType in codec.SupportedHardwareDevices.Value) {
                HardwareDecoderType? type = avhwDeviceType.ToHardwareDecoderType();

                //If its not a valid hw decoder type or its not on our target list, ignore it
                if (!type.HasValue || !wantedHwDecodingType.HasFlag(type.Value))
                    continue;

                //We found a hardware decoder, and its on our list of wanted ones
                list.Add((avhwDeviceType, codec));
            }
        }

        //If we found a SW decoder, add it to the list
        if (first != null)
            list.Add((AVHWDeviceType.AV_HWDEVICE_TYPE_NONE, first));

        list.Sort((x, h) => comparer.Compare(x.Item1, h.Item1));

        return list;
    }

    public void Load(Stream stream, HardwareDecoderType wantedDecoderTypes) {
        int ret = 0;
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
        byte* contextBuffer = (byte*)av_malloc(contextBufferSize);

        this.readPacketCallback = ReadPacketCallbackDef;
        this.seekCallback       = StreamSeekCallbackDef;

        this._gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
        AVIOContext* ioContext = avio_alloc_context(contextBuffer, contextBufferSize, 0, (void*)GCHandle.ToIntPtr(this._gcHandle), this.readPacketCallback, null, this.seekCallback);

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

        List<(AVHWDeviceType, FFmpegCodec)> validDecoders = this.GetPossibleDecoders(this.VideoStream.Stream->codecpar->codec_id, wantedDecoderTypes);

        AVCodecContext* codecContext = null;

        foreach ((AVHWDeviceType, FFmpegCodec) validDecoder in validDecoders) {
            AVHWDeviceType type = validDecoder.Item1;
            FFmpegCodec codec = validDecoder.Item2;

            //Free a previous codec context, if there
            if (codecContext != null) {
                avcodec_free_context(&codecContext);
                this.CodecContext = null;
            }

            //Create and fill the codec context
            codecContext               = avcodec_alloc_context3(codec.Codec);
            codecContext->pkt_timebase = this.VideoStream.Stream->time_base;

            //If we failed to create the codec context, try the next one
            if (codecContext == null) {
                Logger.Log($"Failed to create codec context for decoder {SilkMarshal.PtrToString((nint)codec.Codec->name)}", VideoDecoderLoggerLevel.InstanceInfo);
                continue;
            }

            //Copy the codec parameters to the codec context
            ret = avcodec_parameters_to_context(codecContext, this.VideoStream.Stream->codecpar);
            if (ret < 0) {
                Logger.Log(
                $"Couldnt copy codec parameters from video stream to codec context. Err:{this.FFmpegErrorToString(ret)}",
                VideoDecoderLoggerLevel.InstanceInfo
                );
                continue;
            }

            //If the type is a hardware decoder
            if (type != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE) {
                ret = av_hwdevice_ctx_create(&codecContext->hw_device_ctx, type, null, null, 0);
                if (ret < 0) {
                    Logger.Log($"Failed to init hardware device context! Err:{this.FFmpegErrorToString(ret)}", VideoDecoderLoggerLevel.InstanceInfo);
                    continue;
                }

                Logger.Log(
                $"Successfully created hardware video decoder {type} for codec {SilkMarshal.PtrToString((nint)codec.Codec->name)}",
                VideoDecoderLoggerLevel.InstanceInfo
                );
            }

            //Open the codec
            ret = avcodec_open2(codecContext, codec.Codec, null);
            if (ret < 0) {
                Logger.Log($"Failed to open codec {codec.Name}!", VideoDecoderLoggerLevel.InstanceInfo);
                continue;
            }

            //If we got here, it should work, so use it
            this.CodecContext = new FFmpegCodecContext(codecContext);
            this.Codec        = codec;
            this.HwCodecType  = type;

            Logger.Log($"Successfully initialized codec {SilkMarshal.PtrToString((nint)codec.Codec->long_name)}", VideoDecoderLoggerLevel.InstanceInfo);
            break;
        }

        if (this.CodecContext == null)
            throw new Exception($"No usable decoder found! Tried {validDecoders.Count}!");

        // allocate the video frames
        this.AvFrame        = new FFmpegFrame();
        this.AvStagingFrame = new FFmpegFrame();
        this.RenderFrame    = new FFmpegFrame();
        //Get buffer size
        int size = av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_RGBA, this.CodecContext.CodecContext->width, this.CodecContext.CodecContext->height, 1);
        //Allocate internal buffer
        this.InternalBuffer = (byte*)av_malloc((ulong)(size * sizeof(byte)));

        byte_ptrArray4 dataArray4 = new byte_ptrArray4();
        int_array4 lineSizeArray4 = new int_array4();
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

    private string FFmpegErrorToString(int err) {
        byte* temp = (byte*)SilkMarshal.Allocate(AV_ERROR_MAX_STRING_SIZE);

        av_make_error_string(temp, AV_ERROR_MAX_STRING_SIZE, err);

        string s = SilkMarshal.PtrToString((nint)temp);

        SilkMarshal.Free((nint)temp);

        return s;
    }

    private void DecodingRun() {
        try {
            while (this._runDecoding) {
                bool gotNewFrame = false;

                if (this._commandQueue.TryDequeue(out Delegate result))
                    result.DynamicInvoke();

                lock (this) {
                    //While we have a difference of write position to read position less than the buffer size, read new frames
                    //This ensures that we are constantly reading new frames to fill the buffer size
                    while (this._writeCursor - this._readCursor < this._bufferSize) {
                        //If we should stop decoding, return out of the function all together
                        if (!this._runDecoding)
                            return;
                        
                        //If we reach EOF, break
                        if (av_read_frame(this.FormatContext.FormatContext, this.Packet.Packet) < 0)
                            break;
                        
                        //If the recieved packet is not from the video stream, ignore it
                        if (this.Packet.Packet->stream_index != this.VideoStreamIndex)
                            continue;
                        
                        //Whether we have finished recieving a frame or not
                        bool frameFinished = false;

                        int ret;
                        //If the codec context type is Video or Audio
                        if (this.CodecContext.CodecContext->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO ||
                            this.CodecContext.CodecContext->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO) {
                            //Send the read frame packet to the decoder
                            ret = avcodec_send_packet(this.CodecContext.CodecContext, this.Packet.Packet);
                            //If it succeeded or the error is EAGAIN or EOF
                            if (ret >= 0 || ret == AVERROR(EAGAIN) || ret == AVERROR(AVERROR_EOF)) {
                                //If we succeeded, set the packet size to 0
                                if (ret >= 0)
                                    this.Packet.Packet->size = 0;
                                //Recieve the next frame 
                                ret = avcodec_receive_frame(this.CodecContext.CodecContext, this.AvFrame.Frame);
                                //If we failed to recieve frame and the error is EAGAIN or EOF, then say we succeeded
                                if (ret == AVERROR(EAGAIN) || ret == AVERROR(AVERROR_EOF))
                                    ret = 0;
                                //If we succeeded then mark that we finished a frame
                                if (ret >= 0)
                                    frameFinished = true;
                            }
                        }
                        
                        //If the decoding timestamp of the packet is less than what we are seeking to, continue and keep trying to decode
                        if (this.Packet.Packet->dts < this._seekingTo)
                            continue;

                        //Once we reach here we are guarenteed to either not be seeking or to be past or equal to where we want to seek to
                        this._seekingTo = 0;

                        //If we have finished recieving a frame and the packet data is not null
                        if (frameFinished && this.Packet.Packet->data != null) {
                            //Set the frame to copy from to the frame we just recieved
                            FFmpegFrame copyFrame = this.AvFrame;

                            //If its a hardware pixel format, transfer the data to the temp frame and copy from that instead
                            if (((AVPixelFormat)this.AvFrame.Frame->format).IsHardwarePixelFormat()) {
                                //Transfer the hardware frame into the staging frame
                                ret = av_hwframe_transfer_data(this.AvStagingFrame.Frame, this.AvFrame.Frame, 0);

                                if (ret < 0) {
                                    Logger.Log(
                                    $"Failed to get data from hardware frame! Err:{this.FFmpegErrorToString(ret)}",
                                    VideoDecoderLoggerLevel.InstanceError
                                    );

                                    //If we failed to copy the hardware pixel format to the CPU, something bad went wrong, so just return out
                                    //TODO: add the ability to notify when something goes wrong decoding wise
                                    return;
                                }
                                
                                //Set the frame to copy from to the staging frame
                                copyFrame = this.AvStagingFrame;
                            } 

                            //Create the convert context which will convert the native format of the frame to Rgba 8 bits per channel
                            this.ConvertContext ??= new FFmpegSwsContext(
                            sws_getContext(
                            this.CodecContext.CodecContext->width,
                            this.CodecContext.CodecContext->height,
                            (AVPixelFormat)copyFrame.Frame->format,
                            this.CodecContext.CodecContext->width,
                            this.CodecContext.CodecContext->height,
                            AVPixelFormat.AV_PIX_FMT_RGBA,
                            0,
                            null,
                            null,
                            null
                            )
                            );

                            //Scale the frame
                            sws_scale(
                            this.ConvertContext.SwsContext,
                            copyFrame.Frame->data,
                            copyFrame.Frame->linesize,
                            0,
                            this.CodecContext.CodecContext->height,
                            this.RenderFrame.Frame->data,
                            this.RenderFrame.Frame->linesize
                            );

                            //Copy the frame to the buffer
                            Marshal.Copy(
                            (IntPtr)this.RenderFrame.Frame->data[0],
                            this._frameBuffer[this._writeCursor % this._bufferSize],
                            0,
                            this._frameBuffer[this._writeCursor % this._bufferSize].Length
                            );

                            //The best possible timestamp for the frame, best_effort_timestamp if possible, if not, then use decompression time
                            long frameTimestamp = this.AvFrame.Frame->best_effort_timestamp != AV_NOPTS_VALUE
                                                      ? this.AvFrame.Frame->best_effort_timestamp
                                                      : this.Packet.Packet->dts;
                            
                            //Set the time of the frame buffer to the (frame timestamp - start time) of the video stream
                            this._frameBufferTimes[this._writeCursor % this._bufferSize] =
                                (frameTimestamp - this.VideoStream.Stream->start_time) * this.FrameDelay * 1000;

                            //Increment the write cursor
                            this._writeCursor++;

                            //Set the last playback timestamp to the timestamp of the frame
                            this._lastPts = frameTimestamp;

                            //Mark that we have got a new frame
                            gotNewFrame = true;
                        }
                    }
                }

                //If we havent got a new frame yet, wait 15ms before trying again
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
    ///     Returns null if no new frame is available
    /// </summary>
    /// <returns></returns>
    [CanBeNull]
    public byte[] GetFrame(int time) {
        //While the read cursor is less than the previous position of the write cursor,
        //and the time of the associated framebuffer (to the next position of the read cursor) is less than or equal to the requested time
        while (this._readCursor < this._writeCursor - 1 && this._frameBufferTimes[(this._readCursor + 1) % this._bufferSize] <= time)
            //Increment the read cursor
            this._readCursor++;

        //NOTE: We reach here if the read cursor is at or in front of the write cursor,
        //      and the time of the framebuffer associated with the read cursor is greater than the requested time
        
        //TODO: should currFramebuffer <= time be less than instead? I think it may, as if we request a time that is *exact* to the framebuffer time, it will go one frame ahead?

        //If the read cursor is less than the write cursor
        if (this._readCursor < this._writeCursor) {
            //Set the current display time to the time in the frame buffer for the current position of the read cursor
            this.CurrentDisplayTime = this._frameBufferTimes[this._readCursor % this._bufferSize];
            //Return the associated framebuffer for the position of the read cursor
            return this._frameBuffer[this._readCursor % this._bufferSize];
        }

        //If the read cursor is greater greater than the write cursor, return null, meaning there is no new frame
        return null;
    }
    
    public void Seek(double time) {
        lock (this) {
            Logger.Log($"Seeking to {time}ms", VideoDecoderLoggerLevel.InstanceInfo);

            this._commandQueue.Enqueue(
            new Action(
            delegate {
                int flags = 0;
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

    public void Dispose() {
        if (this._isDisposed)
            return;
        this._isDisposed = true;

        lock (this) {
            //Notify the decoding thread to stop
            this._runDecoding = false;
        }
        
        //Wait for the decoding thread to close before finishing dispose
        if (this._decodingThread.IsAlive)
            this._decodingThread.Join();
        
        //Free the AVIO context
        AVIOContext* context = this.AvioContext;
        avio_context_free(&context);
        this.AvioContext = context;


        //Dispose the stream
        this._fileStream.Dispose();
        //Free the GC handle
        this._gcHandle.Free();
    }
    
    ~VideoDecoder() {
        this.Dispose();
    }

    private enum SeekType {
        Set,
        Current,
        End
    }
}

public class VideoDecoderLoggerLevel : LoggerLevel {

    public static VideoDecoderLoggerLevel InstanceInfo    = new VideoDecoderLoggerLevel(ChannelEnum.Info);
    public static VideoDecoderLoggerLevel InstanceWarning = new VideoDecoderLoggerLevel(ChannelEnum.Warning);
    public static VideoDecoderLoggerLevel InstanceError   = new VideoDecoderLoggerLevel(ChannelEnum.Error);

    private VideoDecoderLoggerLevel(ChannelEnum @enum) => this.Channel = @enum.ToString();

    private enum ChannelEnum {
        Info,
        Warning,
        Error
    }
}
