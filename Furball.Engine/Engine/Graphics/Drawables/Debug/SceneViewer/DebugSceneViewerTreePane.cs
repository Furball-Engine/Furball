using System.Numerics;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Graphics.Drawables.Debug.SceneViewer;

public class DebugSceneViewerTreePane : CompositeDrawable {
    private readonly Bindable<Drawable>  _selected;
    private readonly FixedTimeStepMethod _refresh;

    private Screen _currentScreen;

    public const float WIDTH = 200;

    public override Vector2 Size => new Vector2(WIDTH, 600);

    public DebugSceneViewerTreePane(Bindable<Drawable> selected) {
        this._selected = selected;

        this._currentScreen = FurballGame.Instance.RunningScreen;

        this.InvisibleToInput = true;

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
            DebugSceneViewerTreePaneItem item = new DebugSceneViewerTreePaneItem(drawable, parent, depth, this._selected);

            item.Position.X = depth * 10;
            item.Position.Y = y;

            y += item.Size.Y;
            
            item.OnClick += (_, _) => this._selected.Value = drawable;

            this.Children.Add(item);
        }

        DrawableTreeWalker.Walk(this._currentScreen.Manager, WalkEvent);
    }

    private void AfterScreenChange(object sender, Screen e) {
        this._currentScreen = e;

        this._selected.Value = null;
    }
}
