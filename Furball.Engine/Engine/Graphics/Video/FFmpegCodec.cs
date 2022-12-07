/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Originally written by Beyley Thomas <ep1cm1n10n123@gmail.com>
 */

using System;
using System.Collections.Generic;
using FFmpeg.AutoGen;
using Silk.NET.Core.Native;

namespace Furball.Engine.Engine.Graphics.Video;

public unsafe class FFmpegCodec {
    public AVCodec* Codec;

    public Lazy<IReadOnlyList<AVHWDeviceType>> SupportedHardwareDevices;

    public FFmpegCodec(AVCodec* codec) {
        this.Codec = codec;

        this.SupportedHardwareDevices = new Lazy<IReadOnlyList<AVHWDeviceType>>(
        () => {
            List<AVHWDeviceType> list = new List<AVHWDeviceType>();

            int              i      = 0;
            AVCodecHWConfig* config = null;

            //Iterate over all hw configs, getting the device types
            while ((config = ffmpeg.avcodec_get_hw_config(codec, i)) != null) {
                list.Add(config->device_type);

                i++;
            }

            return list;
        }
        );
    }
    public string Name => SilkMarshal.PtrToString((nint)this.Codec->name);
}

