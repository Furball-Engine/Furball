using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens.Tests; 

public class RichTextDrawableTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(
        new RichTextDrawable(
        new Vector2(10),
        "/tuFirst /td/c[red]line/cd./n/tuSecond/td /ts/f[Comic Sans MS,50]line/td, this time in /c[blue]Com/i[test.png,0.5]ic /c[red]Sans /c[green]MS./cd/n/fdThis /f[Arial,60]is Arial, /fdand /c[blue]THIS/cd is normal",
        FurballGame.DefaultFont,
        30
        )
        );
    }
}
