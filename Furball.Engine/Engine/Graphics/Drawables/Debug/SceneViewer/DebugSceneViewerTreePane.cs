using System.Numerics;

namespace Furball.Engine.Engine.Graphics.Drawables.Debug.SceneViewer;

public class DebugSceneViewerTreePane : CompositeDrawable {
    private readonly FixedTimeStepMethod _refresh;

    private Screen _currentScreen;

    public const float WIDTH = 400;
    
    public override Vector2 Size => new Vector2(WIDTH, 600);

    public DebugSceneViewerTreePane() {
        this._currentScreen = FurballGame.Instance.RunningScreen;

        FurballGame.Instance.AfterScreenChange += this.AfterScreenChange;

        FurballGame.TimeStepMethods.Add(this._refresh = new FixedTimeStepMethod(1000, this.RefreshUI));
    }

    public override void Dispose() {
        base.Dispose();

        FurballGame.Instance.AfterScreenChange -= this.AfterScreenChange;

        FurballGame.TimeStepMethods.Remove(this._refresh);
    }

    private void RefreshUI() {
        //Remove all the children
        this.Children.Clear();

        //Do nothing if the screen is null
        if (this._currentScreen == null)
            return;

        float y = 0;
        void WalkEvent(Drawable drawable, Drawable parent, int depth) {
            DebugSceneViewerTreePaneItem item = new DebugSceneViewerTreePaneItem(drawable, parent, depth);

            item.Position.X = depth * 10;
            item.Position.Y = y;
            
            y += item.Size.Y;
            
            this.Children.Add(item);
        }
        
        DrawableTreeWalker.Walk(this._currentScreen.Manager, WalkEvent);
    }

    private void AfterScreenChange(object sender, Screen e) {
        this._currentScreen = e;
    }
}
