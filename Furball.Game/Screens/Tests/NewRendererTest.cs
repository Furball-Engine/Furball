using System;
using System.Collections.Generic;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.Renderers;

namespace Furball.Game.Screens.Tests; 

public class NewRendererTest : TestScreen {

    private class MeshDrawable : Drawable {
        private readonly Renderer _renderer;
        
        public readonly List<Vertex> Vertices = new();
        public readonly List<ushort> Indices  = new();

        public override Vector2 Size => new(100, 100); 

        public MeshDrawable() {
            this._renderer = GraphicsBackend.Current.CreateRenderer();

            this.Vertices.Add(
            new Vertex {
                Color    = Color.White,
                Position = Vector2.Zero
            }
            );
            this.Vertices.Add(
            new Vertex {
                Color    = Color.Red,
                Position = new Vector2(100, 0),
            }
            ); 
            this.Vertices.Add(
            new Vertex {
                Color    = Color.Blue,
                Position = new Vector2(0, 100),
            }
            );
        
            this.Indices.Add(0);
            this.Indices.Add(2);
            this.Indices.Add(1);
            
            this.RecalcRender();
        }

        public unsafe void RecalcRender() {
            this._renderer.Begin();

            Vertex[] vertices = this.Vertices.ToArray();
            ushort[] indices  = this.Indices.ToArray();

            long texid = this._renderer.GetTextureId(FurballGame.WhitePixel);

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].TexId = texid;

            MappedData map = this._renderer.Reserve((ushort)vertices.Length, (uint)indices.Length);
        
            //Copy the vertex data
            fixed (void* ptr = vertices) {
                Buffer.MemoryCopy(ptr, map.VertexPtr, vertices.Length * sizeof(Vertex), vertices.Length * sizeof(Vertex));
            }
        
            //Copy the index data
            fixed (ushort* ptr = indices) {
                for (int i = 0; i < map.IndexCount; i++)
                    ptr[i] += (ushort)map.IndexOffset;

                Buffer.MemoryCopy(ptr, map.IndexPtr, indices.Length * sizeof(ushort), indices.Length * sizeof(ushort));
            }
        
            this._renderer.End();
        }
        
        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) { 
            FurballGame.DrawableBatch.End();
            this._renderer.Draw();
            FurballGame.DrawableBatch.Begin();
        }
    }

    public override void Initialize() {
        base.Initialize();
        
        this.Manager.Add(new MeshDrawable());
    }
}
