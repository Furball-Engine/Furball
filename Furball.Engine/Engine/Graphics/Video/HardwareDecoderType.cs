/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Originally Copyright (c) ppy Pty Ltd <contact@ppy.sh> under MIT
 * Modified by Beyley Thomas <ep1cm1n10n123@gmail.com>, changes under MPL
 */

#nullable disable

using System;
using System.ComponentModel;
using FFmpeg.AutoGen;

// ReSharper disable InconsistentNaming

namespace Furball.Engine.Engine.Graphics.Video;

/// <summary>
///     Represents a list of usable hardware video decoders.
/// </summary>
/// <remarks>
///     Contains decoders for ALL platforms. Not just the ones for the current platform
/// </remarks>
[Flags]
public enum HardwareDecoderType {
    /// <summary>
    ///     Disables hardware decoding.
    /// </summary>
    [Description("None")]
    None,

    /// <remarks>
    ///     Windows and Linux only.
    /// </remarks>
    [Description("Nvidia NVDEC (CUDA)")]
    NVDEC = 1,

    /// <summary>
    ///     Windows and Linux only.
    /// </summary>
    [Description("Intel Quick Sync Video")]
    QuickSyncVideo = 1 << 2,

    /// <remarks>
    ///     Windows only.
    /// </remarks>
    [Description("DirectX Video Acceleration 2.0")]
    DXVA2 = 1 << 3,

    /// <remarks>
    ///     Linux only.
    /// </remarks>
    [Description("VDPAU")]
    VDPAU = 1 << 4,

    /// <remarks>
    ///     Linux only.
    /// </remarks>
    [Description("VA-API")]
    VAAPI = 1 << 5,

    /// <remarks>
    ///     Android only.
    /// </remarks>
    [Description("Android MediaCodec")]
    MediaCodec = 1 << 6,

    /// <remarks>
    ///     Apple devices only.
    /// </remarks>
    [Description("Apple VideoToolbox")]
    VideoToolbox = 1 << 7,

    [Description("Any")]
    Any = int.MaxValue
}

public static class HardwareDecoderTypeExtensions {
    public static HardwareDecoderType? ToHardwareDecoderType(this AVHWDeviceType hwDeviceType) {
        switch (hwDeviceType) {
            case AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA:
                return HardwareDecoderType.NVDEC;

            case AVHWDeviceType.AV_HWDEVICE_TYPE_QSV:
                return HardwareDecoderType.QuickSyncVideo;

            case AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2:
                return HardwareDecoderType.DXVA2;

            case AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU:
                return HardwareDecoderType.VDPAU;

            case AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI:
                return HardwareDecoderType.VAAPI;

            case AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC:
                return HardwareDecoderType.MediaCodec;

            case AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX:
                return HardwareDecoderType.VideoToolbox;

            default:
                return null;
        }
    }

    public static bool IsHardwarePixelFormat(this AVPixelFormat pixFmt) {
        switch (pixFmt) {
            case AVPixelFormat.AV_PIX_FMT_VDPAU:
            case AVPixelFormat.AV_PIX_FMT_CUDA:
            case AVPixelFormat.AV_PIX_FMT_VAAPI:
            case AVPixelFormat.AV_PIX_FMT_DXVA2_VLD:
            case AVPixelFormat.AV_PIX_FMT_QSV:
            case AVPixelFormat.AV_PIX_FMT_VIDEOTOOLBOX:
            case AVPixelFormat.AV_PIX_FMT_D3D11:
            case AVPixelFormat.AV_PIX_FMT_D3D11VA_VLD:
            case AVPixelFormat.AV_PIX_FMT_DRM_PRIME:
            case AVPixelFormat.AV_PIX_FMT_OPENCL:
            case AVPixelFormat.AV_PIX_FMT_MEDIACODEC:
            case AVPixelFormat.AV_PIX_FMT_VULKAN:
            case AVPixelFormat.AV_PIX_FMT_MMAL:
            case AVPixelFormat.AV_PIX_FMT_XVMC:
                return true;

            default:
                return false;
        }
    }
}
