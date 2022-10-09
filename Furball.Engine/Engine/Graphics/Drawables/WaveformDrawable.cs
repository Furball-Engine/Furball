using System;
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Timing;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Helpers;
using Silk.NET.Maths;
using sowelipisona;
using Color=Furball.Vixie.Backends.Shared.Color;
using Rectangle=System.Drawing.Rectangle;

namespace Furball.Engine.Engine.Graphics.Drawables;

/// <summary>
///     Wraps RawWaveformDrawable and provides cached texture data, and a way to crop to specific parts of the waveform
/// </summary>
public class WaveformDrawable : Drawable {
    private readonly Waveform            _waveform;
    private readonly RawWaveformDrawable _rawWaveform;

    /// <summary>
    /// A texture containing the waveform data, drawn using RawWaveformDrawable
    /// </summary>
    private readonly RenderTarget _renderTarget;

    /// <summary>
    /// The start crop of the waveform, in miliseconds
    /// </summary>
    private double _startCrop = 0;
    /// <summary>
    /// The end crop of the waveform, in miliseconds
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
        if (EndCrop == double.MaxValue)
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

        //We render 1 px width for every 1 sample, so we can just use the sample count as the width
        this._renderTarget = new RenderTarget((uint)waveform.Points!.Length, (uint)Math.Ceiling(verticalHeight));

        this._size = new Vector2(waveform.Points.Length, verticalHeight);
        
        this.DrawWaveform();
        
        this.RecalculateSize();
    }

    private void DrawWaveform() {
        //Create a new batch we will use for drawing
        DrawableBatch batch = new DrawableBatch();
        
        //Begin the drawable batch
        batch.Begin();

        //Bind the render target
        this._renderTarget.Bind();
        
        //Clear the render target
        GraphicsBackend.Current.Clear();
        
        //Draw the raw waveform
        this._rawWaveform.Draw(
        0,
        batch,
        new DrawableManagerArgs {
            Position = Vector2.Zero,
            Scale    = Vector2.One,
            Rotation = 0,
            Color    = Color.White,
            Effects  = TextureFlip.None
        }
        );
        
        //End the batch (this also draws to the render target)
        batch.End();
        
        //Unbind the render target
        this._renderTarget.Unbind();

        //Clean up after ourselves
        batch.Dispose();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        //Draw the render target to the screen
        batch.Draw(this._renderTarget, args.Position, args.Scale, args.Rotation, args.Color, new Rectangle(this._cropPositions.X, 0, this._cropPositions.Y, this._renderTarget.Size.Y), args.Effects, this.RotationOrigin);
    }
}
