using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Furball.Game.Screens.Tests;

public class TexturePackerTest : TestScreen {
    private PackedTexture _packed;
    private Texture       _texture;

    public override bool RequireLoadingScreen => true;

    public override void BackgroundInitialize() {
        base.BackgroundInitialize();

        DirectoryInfo content = new DirectoryInfo(Path.Combine(FurballGame.AssemblyPath, "Content"));

        FileInfo[] files = content.GetFiles("*.png");

        this.LoadingStatus = "Gathering Images...";

        List<Image<Rgba32>> images = new List<Image<Rgba32>>();
        for (int i = 0; i < files.Length; i++) {

            FileInfo fileInfo = files[i];
            this.LoadingStatus = $"Gathering Image... {fileInfo.Name}";
            images.Add(Image.Load<Rgba32>(fileInfo.FullName));

            this.LoadingProgress = i / (files.Length + 1);
        }

        this.LoadingStatus = "Packing Image...";

        Profiler.StartProfile("texture_pack");
        TexturePacker packer = new TexturePacker(images);

        this._packed = packer.Pack();
        Profiler.EndProfileAndPrint("texture_pack");

        this.LoadingProgress = 1;

        this.LoadingComplete = true;
    }

    public override void Initialize() {
        base.Initialize();

        Texture tex = Vixie.Game.ResourceFactory.CreateTextureFromImage(this._packed.Image);

        this._texture = tex;
        
        this._packed.Image.Dispose();
        this._packed.Image = null;

        Vector2 scale = new Vector2((float)FurballGame.DEFAULT_WINDOW_HEIGHT / tex.Height);
        
        this.Manager.Add(
        new TexturedDrawable(tex, new Vector2(0)) {
            Scale = scale
        }
        );
        
        foreach (TexturePacker.TakenSpace space in this._packed.Spaces) {
            this.Manager.Add(new RectanglePrimitiveDrawable(space.Rectangle.Location.ToVector2() * scale, space.Rectangle.Size.ToVector2() * scale, 1, false));
        }
        
        //Make sure to clear everything so the GC picks it up
        this._packed = null;
    }
}
