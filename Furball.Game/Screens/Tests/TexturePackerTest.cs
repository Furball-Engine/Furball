using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using FontStashSharp;
using Furball.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.TexturePacker;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Gdk;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color=Furball.Vixie.Backends.Shared.Color;
using Image=SixLabors.ImageSharp.Image;
using Rectangle=System.Drawing.Rectangle;
using Size=System.Drawing.Size;

namespace Furball.Game.Screens.Tests;

public class TexturePackerTest : TestScreen {
    private PackedTexture _packed;

    private Dictionary<string, Image<Rgba32>> _sourceImages = new Dictionary<string, Image<Rgba32>>();

    public override bool RequireLoadingScreen => true;

    public override void BackgroundInitialize() {
        base.BackgroundInitialize();

        DirectoryInfo content = new DirectoryInfo(Path.Combine(FurballGame.AssemblyPath, "Content"));

        FileInfo[] files = content.GetFiles("*.png");

        this.LoadingStatus = "Gathering Images...";

        List<TextureToPack> toPack = new List<TextureToPack>();
        for (int i = 0; i < files.Length; i++) {
            FileInfo fileInfo = files[i];
            this.LoadingStatus = $"Gathering Image... {fileInfo.Name}";
            Image<Rgba32> img;
            _sourceImages[fileInfo.FullName] = img = Image.Load<Rgba32>(fileInfo.FullName);
            toPack.Add(
            new TextureToPack {
                Name = fileInfo.FullName,
                Size = new Size(img.Width, img.Height)
            }
            );

            this.LoadingProgress = i / (files.Length + 1);
        }

        this.LoadingStatus = "Packing Image...";

        Profiler.StartProfile("texture_pack");
        TexturePacker packer = new TexturePacker(toPack);

        this._packed = packer.Pack();
        Profiler.EndProfileAndPrint("texture_pack");

        this.LoadingProgress = 1;

        this.LoadingComplete = true;
    }

    public override void Initialize() {
        base.Initialize();

        Texture tex = Vixie.Game.ResourceFactory.CreateEmptyTexture((uint)this._packed.Size.X, (uint)this._packed.Size.Y);

        Vector2 scale = new Vector2((float)FurballGame.DEFAULT_WINDOW_HEIGHT / tex.Height);

        this.Manager.Add(
        new TexturedDrawable(tex, new Vector2(0)) {
            Scale = scale
        }
        );

        foreach (TakenSpace space in this._packed.Spaces) {
            Image<Rgba32> sourceImage = this._sourceImages[space.TextureName];

            sourceImage.ProcessPixelRows(
            x => {
                for (int i = 0; i < x.Height; i++) {
                    tex.SetData<Rgba32>(
                    x.GetRowSpan(i),
                    space.Rectangle with {
                        Width = x.Width,
                        Y = space.Rectangle.Y + i,
                        Height = 1
                    }
                    );
                }
            }
            );

            // sourceImage.Dispose();

            this.Manager.Add(
            new RectanglePrimitiveDrawable(space.Rectangle.Location.ToVector2() * scale, space.Rectangle.Size.ToVector2() * scale, 1, false) {
                ColorOverride = Color.Green
            }
            );
        }

        foreach (Rectangle space in this._packed.EmptySpaces) {
            this.Manager.Add(
            new RectanglePrimitiveDrawable(space.Location.ToVector2() * scale, space.Size.ToVector2() * scale, 1, false) {
                ColorOverride = Color.Red
            }
            );

        }

        this.Manager.Add(
        new TextDrawable(new Vector2(10), FurballGame.DefaultFont, $"Final Size: {this._packed.Size}", 24) {
            Effect         = FontSystemEffect.Stroked,
            EffectStrength = 1
        }
        );

        //Make sure to clear everything so the GC picks it up
        this._sourceImages = null;
        this._packed       = null;
    }
}
