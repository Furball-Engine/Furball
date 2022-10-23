using System;
using System.Linq;
using System.Numerics;
using FontStashSharp;

namespace Furball.Engine.Engine.Graphics.Drawables.Debug.SceneViewer; 

public class DebugSceneViewerTreePaneItem : CompositeDrawable {
    private readonly Drawable _drawable;
    private readonly Drawable _parent;
    private readonly int      _depth;

    public const  float ITEM_HEIGHT = 24;
    private const float MARGIN      = 2f;
    
    public override Vector2 Size => new Vector2(DebugSceneViewerTreePane.WIDTH, ITEM_HEIGHT);

    public DebugSceneViewerTreePaneItem(Drawable drawable, Drawable parent, int depth) {
        this._drawable = drawable;
        this._parent   = parent;
        this._depth    = depth;
        
        this.Children.Add(new TextDrawable(new Vector2(MARGIN), FurballGame.DefaultFont, drawable.GetType().Name, ITEM_HEIGHT - MARGIN * 2) {
            Effect = FontSystemEffect.Stroked,
            EffectStrength = 1
        });
    }
}
