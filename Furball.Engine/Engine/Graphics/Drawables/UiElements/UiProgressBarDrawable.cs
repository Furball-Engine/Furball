using System.Drawing;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a simple Progress Bar
    /// </summary>
    public class UiProgressBarDrawable : ManagedDrawable {
        /// <summary>
        ///     The format string for the progress bar
        /// </summary>
        private string _formatString = "{0:0}%";

        /// <summary>
        ///     The internal TextDrawable used to draw the text
        /// </summary>
        public TextDrawable TextDrawable;

        /// <summary>
        ///     The progress to completion
        /// </summary>
        public  float   Progress;
        /// <summary>
        ///     The colour of the outline
        /// </summary>
        public  Color   OutlineColor;
        /// <summary>
        ///     The size of the progress bar
        /// </summary>
        public  Vector2 BarSize;
        /// <summary>
        ///     The width of the progress part itself
        /// </summary>
        private float   _progressWidth;
        /// <summary>
        ///     The thickness of the outline
        /// </summary>
        public  float   OutlineThickness = 1f;

        public override Vector2 Size => this.BarSize * this.Scale;
        
        /// <summary>
        /// Creates a Progress Bar
        /// </summary>
        /// <param name="position">Where to Draw it</param>
        /// <param name="font">What SpriteFont to use</param>
        /// <param name="size">What size should it be?</param>
        /// <param name="outlineColor">Outline Color</param>
        /// <param name="barColor">Bar Color</param>
        /// <param name="textColor">Text Color</param>
        public UiProgressBarDrawable(Vector2 position, FontSystem font, Vector2 size, Color outlineColor, Color barColor, Color textColor) {
            this.Position      = position;
            this.BarSize       = size;
            this.OutlineColor  = outlineColor;
            this.ColorOverride = barColor;

            this.TextDrawable = new TextDrawable(Vector2.Zero, font, "", (int)(size.Y * 0.9f)) {
                ColorOverride = textColor
            };
        }

        public override void Update(double time) {
            this.TextDrawable.Text = string.Format(this._formatString, this.Progress * 100);

            this._progressWidth = this.BarSize.X * this.Progress;

            base.Update(time);
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.Renderer.FillRectangle(
                args.Position * FurballGame.VerticalRatio, 
                new Vector2(this._progressWidth, this.BarSize.Y) * FurballGame.VerticalRatio,
                args.Color,
                0f
            );
            batch.Renderer.DrawRectangle(
                args.Position * FurballGame.VerticalRatio, 
                this.BarSize * FurballGame.VerticalRatio, 
                this.OutlineColor, 
                this.OutlineThickness * FurballGame.VerticalRatio, 
                0f
            );
            
            // FIXME: this is a bit of a hack, it should definitely be done differently
            DrawableManagerArgs tempArgs = args;
            // Center the text in the middle of the bar
            tempArgs.Position.X += this.BarSize.X / 2f;
            tempArgs.Position.Y += this.BarSize.Y / 2f;
            // Do the equivalent of settings the origin to center
            tempArgs.Position.X -= this.TextDrawable.Size.X / 2f;
            tempArgs.Position.Y -= this.TextDrawable.Size.Y / 2f;
            
            tempArgs.Color =  this.TextDrawable.ColorOverride;
            // tempArgs.Origin     =  new Vector2(this.TextDrawable.Size.X / 2f, this.TextDrawable.Size.Y / 2);
            this.TextDrawable.Draw(time, batch, tempArgs);
        }
    }
}
