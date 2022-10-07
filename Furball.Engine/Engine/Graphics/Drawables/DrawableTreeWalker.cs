using System;
using System.Collections.Generic;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class DrawableTreeWalker {
    public delegate void WalkAction(Drawable drawable, Drawable parent, int depth);
    
    public static void Walk(DrawableManager manager, WalkAction action) {
        IReadOnlyList<Drawable> drawables = manager.Drawables;
        
        for (int i = 0; i < drawables.Count; i++) {
            Walk(drawables[i], action);
        }
    }
    
    public static void Walk(Drawable drawable, WalkAction action, Drawable parent = null, int depth = 0) {
        action(drawable, parent, depth);

        if(drawable.Children != null) {
            foreach(Drawable child in drawable.Children) {
                Walk(child, action, drawable, depth + 1);
            }
        }
    }
    
    public static void PrintWalkedTree(DrawableManager manager) {
        Walk(manager, (drawable, parent, depth) => {
            string indent = new string(' ', depth * 2);
            Console.Write($"{indent}{drawable.GetType().Name} size: {drawable.RealSize} pos: {drawable.RealPosition} time: {drawable.DrawableTime} ");
            //If the drawable is a TextDrawable, print the text
            if (drawable is TextDrawable textDrawable) {
                Console.Write($"text: \"{textDrawable.Text}\" ");
            }
            //If the drawable rotation is not 0, print the rotation
            if (drawable.Rotation != 0) {
                Console.Write($"rotation: {drawable.Rotation}rad ");
            }
            //If the drawable color is not Color.White, print the color
            if (drawable.ColorOverride != Color.White) {
                Console.Write($"color: {drawable.ColorOverride} ");
            }
            //If the drawable scale is not Vector2.One, print the scale
            if (drawable.Scale != Vector2.One) {
                Console.Write($"scale: {drawable.Scale} ");
            }
            //If the drawable depth is not 0, print the depth
            if (drawable.Depth != 0) {
                Console.Write($"depth: {drawable.Depth:N2} ");
            }
            //If the drawable tooltip is not null or empty, print the tooltip
            if (!string.IsNullOrEmpty(drawable.ToolTip)) {
                Console.Write($"tooltip: \"{drawable.ToolTip}\" ");
            }
            //If the drawable has more than 0 tweens, print the tween count
            if (drawable.Tweens.Count > 0) {
                Console.Write($"tweens: {drawable.Tweens.Count} ");
            }

            //Write the newline
            Console.Write(Environment.NewLine);
        });
    }
}
