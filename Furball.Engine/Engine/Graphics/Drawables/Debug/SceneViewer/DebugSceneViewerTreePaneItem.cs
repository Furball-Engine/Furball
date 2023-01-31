using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.Debug.SceneViewer; 

public class DebugSceneViewerTreePaneItem : CompositeDrawable {
    private readonly Drawable           _drawable;
    private readonly Drawable           _parent;
    private readonly int                _depth;
    private readonly Bindable<Drawable> _selected;

    public const  float ITEM_HEIGHT = 24;
    private const float MARGIN      = 2f;
    
    public override Vector2 Size => new Vector2(DebugSceneViewerTreePane.WIDTH, ITEM_HEIGHT);

    public DebugSceneViewerTreePaneItem(Drawable drawable, Drawable parent, int depth, Bindable<Drawable> selected) {
        this._drawable = drawable;
        this._parent   = parent;
        this._depth    = depth;
        this._selected = selected;

        this.ChildrenInvisibleToInput = true;
        
        this.Children.Add(new TextDrawable(new Vector2(MARGIN), FurballGame.DefaultFont, drawable.GetType().Name, ITEM_HEIGHT - MARGIN * 2) {
            Effect = FontSystemEffect.Stroked,
            EffectStrength = 1
        });
    }

    public override void Update(double time) {
        base.Update(time);

        this.ColorOverride = this._selected.Value == this._drawable ? Color.White : Color.LightGray;
    }
}
