using System;
using System.Collections.Generic;
using Furball.Vixie.Helpers.Helpers;
using Silk.NET.Maths;
using Rectangle=System.Drawing.Rectangle;

namespace Furball.Engine.Engine.Graphics.TexturePacker;

public class TexturePacker {
    private readonly List<TextureToPack> _images;

    public TexturePacker(List<TextureToPack> images) {
        int PathologicalMultiplier(TextureToPack tex) {
            return Math.Max(tex.Width, tex.Height) / Math.Min(tex.Width, tex.Height) * tex.Width * tex.Height;
        }

        images.Sort((y, x) => PathologicalMultiplier(x).CompareTo(PathologicalMultiplier(y)));

        this._images = images;
    }

    private List<TakenSpace> _lastGoodTakenSpaces;

    private List<TakenSpace> _takenSpaces = new List<TakenSpace>();
    private List<Rectangle>  _emptySpaces = new List<Rectangle>();

    public PackedTexture Pack(int clampSize = 4096) {
        Vector2D<int> size = FurballGame.MaxTextureSize;

        size.X = size.X.Clamp(0, clampSize);
        size.Y = size.Y.Clamp(0, clampSize);

        Vector2D<int> origSize = size;

        bool TryPack(Vector2D<int> fullSize) {
            this._emptySpaces = new List<Rectangle>();
            this._takenSpaces = new List<TakenSpace>();

            this._emptySpaces.Add(new Rectangle(0, 0, fullSize.X, fullSize.Y));

            int IndexOfFirstCandidate(Rectangle rect) {
                for (int i = this._emptySpaces.Count - 1; i >= 0; i--) {
                    Rectangle rect1 = this._emptySpaces[i];

                    if (rect1.Width >= rect.Width && rect1.Height >= rect.Height)
                        return i;
                }

                return -1;
            }

            foreach (TextureToPack image in this._images) {
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
                        Rectangle   = candidate,
                        TextureName = image.Name
                    }
                    );
                    continue;
                }

                //Corner case 2: the candidate is a perfect match on 1 axis, so create only one split
                if (candidate.Width == image.Width) {
                    this._takenSpaces.Add(
                    new TakenSpace {
                        Rectangle = candidate with {
                            Height = image.Height
                        },
                        TextureName = image.Name
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
                        Rectangle = candidate with {
                            Width = image.Width
                        },
                        TextureName = image.Name
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
                    TextureName = image.Name
                }
                );
            }

            return true;
        }

        Vector2D<int> lastGoodSize = size;

        { //Get a good pack with a square
            int stepSize = 25;

            //Shrink a bunch
            while (TryPack(size)) {
                this._lastGoodTakenSpaces = this._takenSpaces;
                lastGoodSize              = size;

                size.X -= stepSize;
                size.Y -= stepSize;
            }

            for (int i = 0; i < stepSize; i++) {
                size.X += 1;
                size.Y += 1;
            
                if (TryPack(size)) {
                    this._lastGoodTakenSpaces = this._takenSpaces;
                    lastGoodSize              = size;
                    break;
                }
            }
        }

        { //Further refine the pack by shrinking width
            int stepSize = 5;
            
            size.X -= stepSize;
            while (TryPack(size)) {
                this._lastGoodTakenSpaces = this._takenSpaces;
                lastGoodSize              = size; 
                
                size.X -= stepSize;
            }
        
            for (int i = 0; i < stepSize; i++) {
                size.X += 1;
        
                if (TryPack(size)) {
                    this._lastGoodTakenSpaces = this._takenSpaces;
                    lastGoodSize              = size;
                    break; 
                }
            }
        }
        
        { //Further refine the pack by shrinking height
            int stepSize = 5;
            
            size.Y -= stepSize;
            while (TryPack(size)) {
                this._lastGoodTakenSpaces = this._takenSpaces;
                lastGoodSize              = size; 
                
                size.Y -= stepSize;
            }
            
            for (int i = 0; i < stepSize; i++) {
                size.Y += 1;
        
                if (TryPack(size)) {
                    this._lastGoodTakenSpaces = this._takenSpaces;
                    lastGoodSize              = size;
                    break; 
                }
            }
        }

        if (lastGoodSize.X > origSize.X || lastGoodSize.Y > origSize.Y)
            throw new Exception("Could not pack texture, try increasing the clamp size");

        if (this._lastGoodTakenSpaces == null) {
            throw new Exception("Unable to pack!");
        }

        return new PackedTexture {
            Spaces = this._lastGoodTakenSpaces,
            Size   = lastGoodSize
        };
    }
}
