using System.Numerics;
using Furball.Engine.Engine.Graphics.GML;
using GMLSharp;

namespace Furball.Game.Screens.Tests; 

public class GMLTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        string gml = @"
@GUI::Widget {
    fixed_width: 260
    fixed_height: 85
    fill_with_background_color: true
    @GUI::Label {
        text: ""Test Text""
        text_alignment: ""TopLeft""
        fixed_width: 30
    }
}";

        GMLFile file = new Parser().Parse(gml);

        GMLFileDrawable drawable = new GMLFileDrawable(Vector2.Zero);
        
        drawable.SetGMLFile(file);
        
        this.Manager.Add(drawable);
    }
}
