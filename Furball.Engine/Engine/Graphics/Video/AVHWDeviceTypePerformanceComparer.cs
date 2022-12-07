/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Originally Copyright (c) ppy Pty Ltd <contact@ppy.sh> under MIT
 * Modified by Beyley Thomas <ep1cm1n10n123@gmail.com>, changes under MPL
 */

#nullable disable

using System.Collections.Generic;
using FFmpeg.AutoGen;

namespace Furball.Engine.Engine.Graphics.Video; 

// ReSharper disable once InconsistentNaming
internal class AVHWDeviceTypePerformanceComparer : Comparer<AVHWDeviceType> {
    // Higher on the list means faster
    private static readonly IReadOnlyDictionary<AVHWDeviceType, int> priorityList = new Dictionary<AVHWDeviceType, int> {
        // Windows + Linux
        {
            AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA, 10
        }, {
            AVHWDeviceType.AV_HWDEVICE_TYPE_QSV, 9
        },
        // Windows only
        {
            AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2, 8
        },
        // Linux
        {
            AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU, 10
        }, {
            AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI, 9
        },
        // Android
        {
            AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC, 10
        },
        // iOS, macOS
        {
            AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX, 10
        }
    };

    public override int Compare(AVHWDeviceType x, AVHWDeviceType y) {
        if (!priorityList.TryGetValue(x, out int xScore))
            xScore = int.MinValue;
        if (!priorityList.TryGetValue(y, out int yScore))
            yScore = int.MinValue;

        return -Comparer<int>.Default.Compare(xScore, yScore);
    }
}
