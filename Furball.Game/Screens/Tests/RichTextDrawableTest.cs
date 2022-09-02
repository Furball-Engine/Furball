using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens.Tests; 

public class RichTextDrawableTest : TestScreen {
    public override void Initialize() {
        base.Initialize();
        
        this.Manager.Add(new RichTextDrawable(new Vector2(10), "First /c[red]line/cd./nSecond /f[Comic Sans MS,50]line, this time in /c[blue]Com/i[test.png]ic /c[red]Sans /c[green]MS./cd/n/fdThis /f[Arial,60]is Arial, /fdand /c[blue]THIS/cd is normal", FurballGame.DEFAULT_FONT, 30));
    }
}
