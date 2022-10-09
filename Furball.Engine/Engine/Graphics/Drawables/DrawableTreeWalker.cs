using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class DrawableTreeWalker {
    public delegate void WalkAction(Drawable drawable, Drawable parent, int depth);
    
    public static void Walk(DrawableManager manager, WalkAction action, int depth = 0) {
        IReadOnlyList<Drawable> drawables = manager.Drawables;
        
        for (int i = 0; i < drawables.Count; i++) {
            Walk(drawables[i], action, null, depth);
        }
    }
    
    public static void Walk(Drawable drawable, WalkAction action, Drawable parent = null, int depth = 0) {
        action(drawable, parent, depth);

        //Walk the screens of ScreenDrawables
        if(drawable is ScreenDrawable screenDrawable) {
            Walk(screenDrawable.Screen.Manager, action, depth + 1);
        }
        
        if(drawable.Children != null) {
            foreach(Drawable child in drawable.Children) {
                Walk(child, action, drawable, depth + 1);
            }
        }
    }
    
    public static void PrintWalkedTree(DrawableManager manager) {
        StringBuilder builder = new StringBuilder();
        
        Walk(manager, (drawable, parent, depth) => {
            string indent = new string(' ', depth * 2);
            builder.Append($"{indent}{drawable.GetType().Name} size: {drawable.RealSize} pos: {drawable.RealPosition} time: {drawable.DrawableTime} ");
            //If the drawable is a TextDrawable, print the text
            if (drawable is TextDrawable textDrawable) {
                builder.Append($"text: \"{textDrawable.Text}\" ");
            }
            //If the drawable rotation is not 0, print the rotation
            if (drawable.Rotation != 0) {
                builder.Append($"rotation: {drawable.Rotation}rad ");
            }
            //If the drawable color is not Color.White, print the color
            if (drawable.ColorOverride != Color.White) {
                builder.Append($"color: {drawable.ColorOverride} ");
            }
            //If the drawable scale is not Vector2.One, print the scale
            if (drawable.Scale != Vector2.One) {
                builder.Append($"scale: {drawable.Scale} ");
            }
            //If the drawable depth is not 0, print the depth
            if (drawable.Depth != 0) {
                builder.Append($"depth: {drawable.Depth:N2} ");
            }
            //If the drawable tooltip is not null or empty, print the tooltip
            if (!string.IsNullOrEmpty(drawable.ToolTip)) {
                builder.Append($"tooltip: \"{drawable.ToolTip}\" ");
            }
            //If the drawable has more than 0 tweens, print the tween count
            if (drawable.Tweens.Count > 0) {
                builder.Append($"tweens: {drawable.Tweens.Count} ");
            }
            //If the drawable is not visible, print the visibility
            if (!drawable.Visible) {
                builder.Append($"visible: {drawable.Visible}");
            }

            //Write the newline
            builder.Append(Environment.NewLine);
        });
        
        File.WriteAllText("drawabletreedump.txt", builder.ToString());
        
        Console.WriteLine(builder.ToString());
    }
}
