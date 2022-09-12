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

public unsafe class FFmpegSwsContext : IDisposable{
    public SwsContext* SwsContext;
    public FFmpegSwsContext(SwsContext* swsGetContext) {
        this.SwsContext = swsGetContext;
    }
    private void ReleaseUnmanagedResources() {
        if (this.SwsContext == null)
            return;
        ffmpeg.sws_freeContext(this.SwsContext);
        this.SwsContext = null;
    }
    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    ~FFmpegSwsContext() {
        ReleaseUnmanagedResources();
    }
}
