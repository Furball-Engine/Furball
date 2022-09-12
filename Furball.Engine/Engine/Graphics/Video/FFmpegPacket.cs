/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Originally written by Beyley Thomas <ep1cm1n10n123@gmail.com>
 */

using System;
using FFmpeg.AutoGen;

namespace Furball.Engine.Engine.Graphics.Video; 

public unsafe class FFmpegPacket : IDisposable {
    public AVPacket* Packet;
    public FFmpegPacket(AVPacket* avPacketAlloc) {
        this.Packet = avPacketAlloc;
    }
    private void ReleaseUnmanagedResources() {
        if (this.Packet == null)
            return;
        AVPacket* packet = this.Packet;
        ffmpeg.av_packet_free(&packet);
        this.Packet = null;
    }
    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    ~FFmpegPacket() {
        ReleaseUnmanagedResources();
    }
}
