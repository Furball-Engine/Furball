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
        // fixed_height: 22
        text: ""Test Text""
        text_alignment: ""Center""
    }
    @GUI::Button {
        // fixed_height: 22
        text: ""Test Button""
    }
}";

        GMLFile file = new Parser().Parse(gml);

        GMLFileDrawable drawable = new GMLFileDrawable(new Vector2(10));
        
        drawable.SetGMLFile(file);
        
        this.Manager.Add(drawable);
    }
}
