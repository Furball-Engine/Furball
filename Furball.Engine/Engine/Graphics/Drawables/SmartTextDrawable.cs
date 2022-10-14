using System;
using System.Numerics;
using System.Threading.Tasks;
using Furball.Vixie;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class SmartTextDrawable : TexturedDrawable {
    private readonly Color _finalColor;

    private Vector2 _size;

    public override Vector2 Size => this._size;

    //TODO: recalc size on user window resize
    //this can be done transparently, as we can continue to scale up the old one (with slightly poorer quality) until the new image is generated
    //problems will occur if the user resizes while the text is still generating, so the only solution i can think of is to begin one, 
    //then if a request to resize it happens, keep the latest request, then once the first request finishes, start the latest request
    //this prevents there being 80 threads when the user resizes the window by hand, and prevents it *not* happening
    public SmartTextDrawable(Vector2 position, string text, FontFamily fontFamily, float textSize, Vector2 scale, Color color) : base(FurballGame.WhitePixel, position) {
        this.ColorOverride = new Color(color.Rf, color.Gf, color.Bf, 0f);
        this.Scale         = scale;

        this.Text       = text;
        this.Family     = fontFamily;
        this.TextSize   = textSize;
        this._finalColor = color;
        
        this.Recreate();
    }

    public void Recreate() {
        Task.Run(
        () => {
            Font font = this.Family.CreateFont(this.TextSize);

            TextOptions options = new(font);

            FontRectangle bounds = TextMeasurer.Measure(this.Text, options);

            this._size = new Vector2(bounds.Width + bounds.X, bounds.Height + bounds.Y);
            
            Image<Rgba32> image = new((int)Math.Ceiling(bounds.Width) + (int)Math.Ceiling(bounds.X), (int)Math.Ceiling(bounds.Height) + (int)Math.Ceiling(bounds.Y), new Rgba32(0f, 0f, 0f, 0f));

            image.Mutate(x => x.DrawText(options, this.Text, SixLabors.ImageSharp.Color.White));

            FurballGame.GameTimeScheduler.ScheduleMethod(
            _ => {
                if (this._first) {
                    this.FadeColor(this._finalColor, 100);
                    this._first = false;
                } else {
                    this.Texture.Dispose();
                }
                
                this.Texture      = Game.ResourceFactory.CreateTextureFromImage(image);
                this.Texture.Name = "SmartText";
                
                image.Dispose();
            });
        });
    }

    public FontFamily Family;
    public float      TextSize;
    public string     Text;
    
    private bool _first = true;
}
