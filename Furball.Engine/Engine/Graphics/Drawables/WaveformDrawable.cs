using System;
using System.Collections.Generic;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Helpers;
using Silk.NET.Maths;
using sowelipisona;
using Rectangle = System.Drawing.Rectangle;

namespace Furball.Engine.Engine.Graphics.Drawables;

/// <summary>
///     Wraps RawWaveformDrawable and provides cached texture data, and a way to crop to specific parts of the waveform
/// </summary>
public class WaveformDrawable : Drawable {
    private readonly Waveform            _waveform;
    private readonly RawWaveformDrawable _rawWaveform;

    private readonly List<RenderTarget> _renderTargets = new List<RenderTarget>();

    /// <summary>
    ///     The start crop of the waveform, in miliseconds
    /// </summary>
    private double _startCrop = 0;
    /// <summary>
    ///     The end crop of the waveform, in miliseconds
    /// </summary>
    private double _endCrop = double.MaxValue;

    public double StartCrop {
        get => this._startCrop;
        set {
            this._startCrop = value;
            this.RecalculateSize();
        }
    }

    public double EndCrop {
        get => this._endCrop;
        set {
            this._endCrop = value;
            this.RecalculateSize();
        }
    }

    private Vector2D<int> _cropPositions;

    private void RecalculateSize() {
        // Calculate the start and end crop positions
        int startIndex = this._waveform.GetPointFromTime(this._startCrop);
        int endIndex   = this._waveform.GetPointFromTime(this._endCrop);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        //If the end crop is the max value, then we want to draw to the end of the waveform
        if (this.EndCrop == double.MaxValue)
            endIndex = this._waveform.Points!.Length;

        this._cropPositions = new Vector2D<int>(startIndex, endIndex);

        //Get the width of the cropped waveform
        this._size.X = Math.Abs(endIndex - startIndex);
    }

    private Vector2 _size;

    public override Vector2 Size => this._size * this.Scale;

    public WaveformDrawable(Vector2 position, Waveform waveform, float verticalHeight) {
        this._waveform = waveform;
        this.Position  = position;

        Guard.EnsureNonNull(waveform.Points);

        this._rawWaveform = new RawWaveformDrawable(waveform, verticalHeight);

        //Calculate how many render targets are need for the waveform, where each render target is 1024 pixels wide
        int renderTargetCount = (int)Math.Ceiling(waveform.Points!.Length / (float)TEXTURE_SIZE);

        //Create the render targets
        for (int i = 0; i < renderTargetCount; i++) {
            RenderTarget renderTarget = new RenderTarget(TEXTURE_SIZE, (uint)this._rawWaveform.Size.Y);
            this._renderTargets.Add(renderTarget);
        }

        this._size = new Vector2(waveform.Points.Length, verticalHeight);

        this.DrawWaveform();

        this.RecalculateSize();
    }

    private const int TEXTURE_SIZE = 1024;

    private void DrawWaveform() {
        //Create a new batch we will use for drawing
        DrawableBatch batch = new DrawableBatch();

        //Draw the waveform to each render target offset by 1024 pixels
        for (int i = 0; i < this._renderTargets.Count; i++) {
            RenderTarget target = this._renderTargets[i];
            //Bind the render target
            target.Bind();

            //Clear the render target
            GraphicsBackend.Current.Clear();

            //Begin the batch
            batch.Begin();

            //Set the start and end index we draw
            this._rawWaveform.StartIndex = i       * TEXTURE_SIZE;
            this._rawWaveform.EndIndex   = (i + 1) * TEXTURE_SIZE;

            //Draw the waveform
            this._rawWaveform.Draw(
            0,
            batch,
            new DrawableManagerArgs {
                Position = new Vector2(-i * 1024, 0),
                Scale    = Vector2.One,
                Rotation = 0,
                Color    = Color.White,
                Effects  = TextureFlip.None
            }
            );

            //End the batch and draw
            batch.End();

            //Unbind the render target
            target.Unbind();
        }

        //Clean up after ourselves
        batch.Dispose();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        //Find the first render target we will be drawing, and the last render target we will be drawing
        int startTextureIndex = (int)Math.Floor(this._cropPositions.X   / (float)TEXTURE_SIZE);
        int endTextureIndex   = (int)Math.Ceiling(this._cropPositions.Y / (float)TEXTURE_SIZE);

        float offsetFromStart = this._cropPositions.X % TEXTURE_SIZE;
        float offsetFromEnd   = this._cropPositions.Y % TEXTURE_SIZE;

        //If we only need to draw one of the textures, then do special logic
        if (Math.Abs(this._cropPositions.Y - this._cropPositions.X) < TEXTURE_SIZE) {
            batch.Draw(
            this._renderTargets[startTextureIndex],
            args.Position,
            args.Scale,
            args.Rotation,
            Color.White,
            new Rectangle(this._cropPositions.X, 0, this._cropPositions.Y, (int)this._size.Y)
            );

            return;
        }

        //Stitch the render targets together to make a cohesive waveform
        for (int i = startTextureIndex; i < endTextureIndex; i++) {
            RenderTarget target = this._renderTargets[i];

            //If we are drawing the first waveform texture
            if (i == startTextureIndex)
                batch.Draw(
                target,
                args.Position + new Vector2(0, 0),
                args.Scale,
                0,
                Color.White,
                new Rectangle((int)offsetFromStart, 0, (int)(TEXTURE_SIZE - offsetFromStart), (int)this.Size.Y),
                args.Effects
                );
            //If we are drawing the last waveform texture
            else if (i == endTextureIndex - 1)
                batch.Draw(
                target,
                args.Position + new Vector2(TEXTURE_SIZE * (i - startTextureIndex) - offsetFromStart, 0) * args.Scale,
                args.Scale,
                0,
                Color.White,
                new Rectangle(0, 0, (int)(TEXTURE_SIZE - offsetFromEnd), (int)this.Size.Y),
                args.Effects
                );
            //If we are drawing one of the ones in the middle
            else
                batch.Draw(
                target,
                args.Position + new Vector2(TEXTURE_SIZE * (i - startTextureIndex) - offsetFromStart, 0) * args.Scale,
                args.Scale,
                0,
                Color.White,
                args.Effects
                );
        }
    }

    public override void Dispose() {
        base.Dispose();

        foreach (RenderTarget renderTarget in this._renderTargets)
            renderTarget.Dispose();

        this._renderTargets.Clear();
    }
}
