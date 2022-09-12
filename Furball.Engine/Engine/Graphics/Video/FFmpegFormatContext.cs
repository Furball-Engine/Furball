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
    
    public FFmpegFormatContext(AVFormatContext* avFormatContext) {
        this.FormatContext = avFormatContext;
    }
    private void ReleaseUnmanagedResources() {
        if(this.FormatContext == null)
            return;

        ffmpeg.avformat_free_context(this.FormatContext);
        this.FormatContext = null;
    }
    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    ~FFmpegFormatContext() {
        ReleaseUnmanagedResources();
    }
}
