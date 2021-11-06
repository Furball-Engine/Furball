using Furball.Vixie.Graphics.Renderers;

namespace Furball.Engine.Engine.Graphics {
    /// <summary>
    /// A Basic Abstraction for Sprite and Shape batch
    /// </summary>
    public class DrawableBatch {
        public ITextureRenderer Renderer;

        private bool _begun;
        public bool Begun => _begun;
        
        public DrawableBatch(ITextureRenderer renderer) {
            this.Renderer = renderer;
        }

        public void Begin() {
            this.Renderer.Begin();
            this._begun = true;
        }

        public void End() {
            this.Renderer.End();
            this._begun = false;
        }
    }
}
