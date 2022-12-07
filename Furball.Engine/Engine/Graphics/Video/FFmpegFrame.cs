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

public unsafe class FFmpegFrame : IDisposable {

    private bool     _released = false;
    public  AVFrame* Frame;

    public FFmpegFrame() => this.Frame = ffmpeg.av_frame_alloc();

    public void Dispose() {

        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~FFmpegFrame() {
        this.ReleaseUnmanagedResources();
    }
    private void ReleaseUnmanagedResources() {
        if (this._released)
            return;

        this._released = true;

        AVFrame* ptr = this.Frame;
        ffmpeg.av_frame_free(&ptr);
        this.Frame = null;
    }
}

