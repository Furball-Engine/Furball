using System;
using System.Collections.Generic;
using System.Linq;
using Furball.Vixie;
using Furball.Vixie.Helpers.Helpers;
using Silk.NET.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle=System.Drawing.Rectangle;

namespace Furball.Engine.Engine.Graphics;

public class TexturePacker {
    private readonly List<Image<Rgba32>> _images;

    public TexturePacker(List<Image<Rgba32>> images) {
        images.Sort((y, x) => (x.Width * x.Height) - (y.Width * y.Height));

        this._images = images;
    }

    public struct TakenSpace {
        public Rectangle     Rectangle;
        public Image<Rgba32> Image;
    }

    private List<TakenSpace> _lastGoodTakenSpaces;
    private List<Rectangle> _lastGoodEmptySpaces;
    
    private List<TakenSpace> _takenSpaces = new List<TakenSpace>();
    private List<Rectangle>  _emptySpaces = new List<Rectangle>();

    public PackedTexture Pack() {
        Vector2D<int> size = FurballGame.MaxTextureSize;

        size.X = size.X.Clamp(0, 4096);
        size.Y = size.Y.Clamp(0, 4096);

        bool TryPack(Vector2D<int> size) {
            this._emptySpaces = new List<Rectangle>();
            this._takenSpaces = new List<TakenSpace>();
            
            this._emptySpaces.Add(new Rectangle(0, 0, size.X, size.Y));

            int IndexOfFirstCandidate(Rectangle rect) {
                for (int i = this._emptySpaces.Count - 1; i >= 0; i--) {
                    Rectangle rect1 = this._emptySpaces[i];

                    if (rect1.Width >= rect.Width && rect1.Height >= rect.Height)
                        return i;
                }

                return -1;
            }

            foreach (Image<Rgba32> image in this._images) {
                int index = IndexOfFirstCandidate(new Rectangle(0, 0, image.Width, image.Height));

                if (index == -1) {
                    return false;
                }

                Rectangle candidate = this._emptySpaces[index];

                //Swap and remove the last element
                this._emptySpaces.RemoveAt(index);

                //Corner case 1: the candidate is a perfect match, so dont create 2 splits
                if (candidate.Width == image.Width && candidate.Height == image.Height) {
                    this._takenSpaces.Add(
                    new TakenSpace {
                        Rectangle = candidate,
                        Image     = image
                    }
                    );
                    continue;
                }

                //Corner case 2: the candidate is a perfect match on 1 axis, so create only one split
                if (candidate.Width == image.Width) {
                    this._takenSpaces.Add(
                    new TakenSpace {
                        Rectangle = candidate,
                        Image     = image
                    }
                    );

                    this._emptySpaces.Add(
                    candidate with {
                        Y = candidate.Y + image.Height,
                        Height = candidate.Height - image.Height
                    }
                    );

                    continue;
                }
                if (candidate.Height == image.Height) {
                    this._takenSpaces.Add(
                    new TakenSpace {
                        Rectangle = candidate,
                        Image     = image
                    }
                    );

                    this._emptySpaces.Add(
                    candidate with {
                        X = candidate.X + image.Width,
                        Width = candidate.Width - image.Width
                    }
                    );

                    continue;
                }

                //Split the candidate into two rectangles of the remaining space
                if (candidate.Width - image.Width > 0) {
                    Rectangle rect1 = new Rectangle(candidate.X + image.Width, candidate.Y, candidate.Width - image.Width, image.Height);
                    Rectangle rect2 = candidate with {
                        Y = candidate.Y + image.Height,
                        Height = candidate.Height - image.Height
                    };

                    //If the first rect is smaller than the second, swap them
                    if (rect1.Width * rect1.Height < rect2.Width * rect2.Height) {
                        (rect1, rect2) = (rect2, rect1);
                    }

                    //Add the rects to the list
                    this._emptySpaces.Add(rect1);
                    this._emptySpaces.Add(rect2);
                }

                this._takenSpaces.Add(
                new TakenSpace {
                    Rectangle = candidate with {
                        Width = image.Width,
                        Height = image.Height
                    },
                    Image = image
                }
                );
            }

            return true;
        }

        while (TryPack(size)) {
            this._lastGoodEmptySpaces = this._emptySpaces;
            this._lastGoodTakenSpaces = this._takenSpaces;

            //10 is rather rough, but we want this to be relatively fast, so /shrug
            size.X -= 10;
            size.Y -= 10;
        }

        Image<Rgba32> packed = new Image<Rgba32>(size.X, size.Y);

        packed.Mutate(
        context => {
            context.Clear(Color.Transparent);

            foreach (TakenSpace takenSpace in this._lastGoodTakenSpaces) {
                context.DrawImage(takenSpace.Image, new Point(takenSpace.Rectangle.X, takenSpace.Rectangle.Y), 1);
            }
        }
        );

        return new PackedTexture {
            Image = packed, 
            Spaces = this._lastGoodTakenSpaces
        };
    }
}
