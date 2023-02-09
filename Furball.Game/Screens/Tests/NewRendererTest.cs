using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.Renderers;
using Silk.NET.Input;

namespace Furball.Game.Screens.Tests;

public class NewRendererTest : TestScreen {
    private MeshDrawable _mesh;

    private class MeshDrawable : Drawable {
        private readonly Renderer _renderer;

        public readonly List<Vertex> Vertices = new();
        public readonly List<ushort> Indices  = new();

        public override Vector2 Size => new(100, 100);

        public MeshDrawable() {
            this._renderer = Vixie.Game.ResourceFactory.CreateRenderer();

            this.Vertices.Add(
            new Vertex {
                Color    = Color.Green,
                Position = new Vector2(100)
            }
            );
            this.Vertices.Add(
            new Vertex {
                Color    = Color.Red,
                Position = new Vector2(200, 100)
            }
            );
            this.Vertices.Add(
            new Vertex {
                Color    = Color.Blue,
                Position = new Vector2(100, 200)
            }
            );

            this.Indices.Add(0);
            this.Indices.Add(2);
            this.Indices.Add(1);

            this.RecalcRender();
        }

        public unsafe void RecalcRender() {
            this._renderer.Begin();

            if (this.Vertices.Count != 0 && this.Indices.Count != 0) {
                Vertex[] vertices = this.Vertices.ToArray();
                ushort[] indices  = this.Indices.ToArray();

                MappedData map = this._renderer.Reserve((ushort)vertices.Length, (uint)indices.Length, FurballGame.WhitePixel);
                
                long texid = map.TextureId;

                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].TexId = texid;

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
            }

            this._renderer.End();
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.End();
            this._renderer.Draw();
            batch.Begin();
        }
    }

    private readonly List<VertexHandle> _vertexPoints = new();

    private readonly ObservableCollection<VertexHandle> _selectedVertices = new();

    private DrawableColorPicker _colorPicker;
    private bool                _recalcNeeded;

    public override void Initialize() {
        base.Initialize();

        this.Manager.Add(this._mesh = new MeshDrawable());

        FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
        FurballGame.InputManager.OnKeyDown   += this.OnKeyDown;

        this._selectedVertices.CollectionChanged += this.OnSelectedChange;

        this.Manager.Add(this._colorPicker = new DrawableColorPicker(new Vector2(5), FurballGame.DefaultFont, 30, Color.White));
        this._colorPicker.Color.OnChange += this.OnColorChange;

        this.Recalc();
    }

    private List<int> GetIndexOfIndicesForVertex(VertexHandle handle) {
        int vertexIndex = this._vertexPoints.IndexOf(handle);

        List<int> found = new();
        //We want to iterate backwards so its easier to delete
        for (int i = this._mesh.Indices.Count - 3; i >= 0; i -= 3) {
            ushort index1 = this._mesh.Indices[i];
            ushort index2 = this._mesh.Indices[i + 1];
            ushort index3 = this._mesh.Indices[i + 2];

            //If we found a tri that uses our vertex, then add it to the list
            if (index1 == vertexIndex || index2 == vertexIndex || index3 == vertexIndex) {
                found.Add(i);
            }
        }
        return found;
    }

    private void OnColorChange(object sender, Color e) {
        FurballGame.GameTimeScheduler.ScheduleMethod(
        _ => {
            foreach (VertexHandle vertex in this._selectedVertices) {
                Vertex meshVertex = this._mesh.Vertices[this._vertexPoints.IndexOf(vertex)];

                meshVertex.Color = e;

                this._mesh.Vertices[this._vertexPoints.IndexOf(vertex)] = meshVertex;
            }
            this._mesh.RecalcRender();
        }
        );
    }

    private void OnKeyDown(object sender, KeyEventArgs keyEventArgs) {
        switch (keyEventArgs.Key) {
            case Key.A: {
                if (this._selectedVertices.Count != 3)
                    break;

                bool Match(ushort x) => x == this._vertexPoints.IndexOf(this._selectedVertices[0]) || x == this._vertexPoints.IndexOf(this._selectedVertices[1]) ||
                                        x == this._vertexPoints.IndexOf(this._selectedVertices[2]);

                if (this._mesh.Indices.Count(Match) == 3) {
                    this._mesh.Indices.RemoveAll(Match);
                    this._recalcNeeded = true;
                    return;
                }

                if (keyEventArgs.Keyboard.ControlHeld) {
                    this._mesh.Indices.Add((ushort)this._vertexPoints.IndexOf(this._selectedVertices[0]));
                    this._mesh.Indices.Add((ushort)this._vertexPoints.IndexOf(this._selectedVertices[2]));
                    this._mesh.Indices.Add((ushort)this._vertexPoints.IndexOf(this._selectedVertices[1]));
                } else {
                    for (int i = 0; i < 3; i++)
                        //Add the indices for the selected vertices
                        this._mesh.Indices.Add((ushort)this._vertexPoints.IndexOf(this._selectedVertices[i]));
                }

                this._recalcNeeded = true;

                break;
            }
        }
    }

    public override void Dispose() {
        base.Dispose();

        FurballGame.InputManager.OnMouseDown -= this.OnMouseDown;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e) {
        if (e.Button != MouseButton.Left || !FurballGame.InputManager.ControlHeld)
            return;

        this._mesh.Vertices.Add(
        new Vertex {
            Color    = Color.White,
            Position = e.Mouse.Position,
        }
        );

        this._recalcNeeded = true;
    }
    public override void Update(double gameTime) {
        base.Update(gameTime);

        if (this._recalcNeeded) {
            this._mesh.RecalcRender();
            this.Recalc();

            this._recalcNeeded = false;
        }
    }

    private void OnSelectedChange(object sender, NotifyCollectionChangedEventArgs e) {
        foreach (VertexHandle vert in this._vertexPoints)
            vert.FadeColor(this._selectedVertices.Contains(vert) ? Color.LightBlue : Color.White, 100);
    }

    private class VertexHandle : TexturedDrawable {
        public VertexHandle(Vector2 position) : base(FurballGame.WhitePixel, position) {
            this.RegisterForInput();
        }
    }

    private void Recalc() {
        //Clear the selected vertices
        this._selectedVertices.Clear();

        //Remove all the vertex points from the manager
        this._vertexPoints.ForEach(x => this.Manager.Remove(x));
        this._vertexPoints.Clear();

        for (int i = 0; i < this._mesh.Vertices.Count; i++) {
            Vertex       vert    = this._mesh.Vertices[i];
            VertexHandle vertTex = new(vert.Position);
            vertTex.Depth      = -1;
            vertTex.Scale      = new Vector2(10);
            vertTex.OriginType = OriginType.Center;

            int i1 = i;
            vertTex.OnDrag += (_, e) => {
                Vertex meshVertex = this._mesh.Vertices[i1];
                meshVertex.Position     = e.Position;
                this._mesh.Vertices[i1] = meshVertex;

                this._recalcNeeded = true;
                vertTex.Position   = e.Position;
            };

            vertTex.OnDragBegin += delegate {
                if (!this._selectedVertices.Contains(vertTex))
                    this._selectedVertices.Add(vertTex);
                else
                    this._selectedVertices.Remove(vertTex);
            };

            vertTex.OnClick += delegate(object _, MouseButtonEventArgs e) {
                switch (e.Button) {
                    case MouseButton.Left: {
                        //If we are already selecting the vertex, deselect it, else select it
                        if (!this._selectedVertices.Contains(vertTex))
                            this._selectedVertices.Add(vertTex);
                        else
                            this._selectedVertices.Remove(vertTex);

                        break;
                    }
                    case MouseButton.Right: {
                        List<int> foundIndexes = this.GetIndexOfIndicesForVertex(vertTex);

                        foreach (int foundIndex in foundIndexes) {
                            for (int j = 2; j >= 0; j--) {
                                //Remove all the indices
                                this._mesh.Indices.RemoveAt(foundIndex + j);
                            }
                        }

                        for (int j = 0; j < this._mesh.Indices.Count; j++) {
                            ushort meshIndex = this._mesh.Indices[j];

                            if (meshIndex >= this._vertexPoints.IndexOf(vertTex))
                                this._mesh.Indices[j]--;
                        }

                        this._mesh.Vertices.RemoveAt(i1);

                        this._recalcNeeded = true;

                        break;
                    }
                }
            };

            this._vertexPoints.Add(vertTex);
        }

        this._vertexPoints.ForEach(x => this.Manager.Add(x));
    }
}
