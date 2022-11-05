using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens.Tests;

public class GraphTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        GraphDrawable graph = new GraphDrawable(
        GraphFunction,
        new GraphAxis {
            Start = 0,
            End   = 100,
            Label = "X"
        },
        new GraphAxis {
            Start = 0,
            End   = 100,
            Label = "Y"
        }
        );

        graph.Position = new Vector2(100);
        graph.Scale    = new Vector2(5, 2.5f);

        graph.Resolution = 5;

        graph.Recalculate();
        
        this.Manager.Add(graph);
    }
    
    private double GraphFunction(double arg) {
        return Math.Sin(arg) * 20 + 20;
    }
}
