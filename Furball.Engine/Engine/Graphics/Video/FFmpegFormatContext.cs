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

public unsafe class FFmpegFormatContext : IDisposable {
    public AVFormatContext* FormatContext;

    public FFmpegFormatContext(AVFormatContext* avFormatContext) => this.FormatContext = avFormatContext;
    public void Dispose() {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    private void ReleaseUnmanagedResources() {
        if (this.FormatContext == null)
            return;

        ffmpeg.avformat_free_context(this.FormatContext);
        this.FormatContext = null;
    }
    ~FFmpegFormatContext() {
        this.ReleaseUnmanagedResources();
    }
}

