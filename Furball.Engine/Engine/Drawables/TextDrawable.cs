using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Drawables {
	public class TextDrawable : ManagedDrawable {
		public SpriteFont Font;
		public string     Text;
		
		public TextDrawable(SpriteFont font, string text) {
			this.Font = font;
			this.Text = text;
		}
		
		public override void Draw(GameTime time, SpriteBatch batch) {
			batch.DrawString(this.Font, this.Text, this.Position, this.ColorOverride, this.Rotation, Vector2.Zero, this.Scale, this.SpriteEffect, 0f);
		}
	}
}
