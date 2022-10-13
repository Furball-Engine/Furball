using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Input;
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Volpe.Evaluation;
using Furball.Volpe.Exceptions;

namespace Furball.Engine.Engine.DevConsole; 

public class Builtins {
    public static readonly BuiltinFunction[] Collection = new BuiltinFunction[] {
        new("println", paramCount: 1, 
            (_, parameters) => {
                string output = parameters[0] switch {
                    Value.String str => str.Value,
                    Value.Number n   => n.Value.ToString(CultureInfo.InvariantCulture),
                    var v            => v.Representation
                };
                        
                DevConsole.AddMessage(output);
                        
                return Value.DefaultVoid;
            }),
        
        new("cl_clear_log", paramCount: 0,
            (_, _) => {
                if (DevConsole.ConsoleLog.Count == 0) {
                    DevConsole.AddMessage("Console log is already empty!", ExecutionResult.Warning);
                    return Value.DefaultVoid;
                }
                    
                DevConsole.ConsoleLog.Clear();
                DevConsole.AddMessage("Console log cleared", ExecutionResult.Success);
                    
                return Value.DefaultVoid;
            }),
            
        new ("cl_clear_log_folder", paramCount: 0,
             (_, _) => {
                 string[] directories = Directory.GetFiles(DevConsole.LogPath);

                 try {
                     for (int i = 0; i != directories.Length; i++) {
                         string currentFile = directories[i];

                         File.Delete(currentFile);
                     }
                 }
                 catch {
                     DevConsole.AddMessage("Something went wrong. Make sure the `log` directory exists, and that the `log` directory and its files aren't write protected.", ExecutionResult.Error);
                 }

                 DevConsole.AddMessage("Successfully cleared the log directory.", ExecutionResult.Success);
                 return Value.DefaultVoid;
             }),
            
        new ("cl_flush_log", paramCount: 0,
             (_, _) => {
                 if (DevConsole.ConsoleLog.Count == 0) {
                     DevConsole.AddMessage("No log to flush!", ExecutionResult.Warning);
                     return Value.DefaultVoid;
                 }

                 try {
                     string filename = DevConsole.WriteLog();
                     DevConsole.AddMessage( $"Log successfully written to `{filename}`", ExecutionResult.Success);
                 }
                 catch (Exception e) {
                     DevConsole.AddMessage("Could not write the log. " + e.Message, ExecutionResult.Error);
                 }
                     
                 return Value.DefaultVoid;
             }), 
            
        new ("cl_get_game_time", paramCount: 0,
             (_, _) => new Value.Number(FurballGame.GameTimeSource.GetCurrentTime())
        ),

        new ("cl_set_target_fps", paramCount: 1,
             (context, parameters) => {
                 if (parameters[0] is not Value.Number(var value))
                     throw new InvalidValueTypeException(typeof(Value.Number), parameters[0].GetType(), context.Expression.PositionInText);
                     
                 FurballGame.Instance.SetTargetFps(value);
                 DevConsole.AddMessage($"Set Target FPS to {value}", ExecutionResult.Success);
                     
                 return Value.DefaultVoid;
             }),
            
        new ("cl_set_screen_resolution", paramCount: 2, 
             (context, parameters) => {
                 if (parameters[0] is not Value.Number(var width))
                     throw new InvalidValueTypeException(typeof(Value.Number), parameters[0].GetType(), context.Expression.PositionInText);
                     
                 if (parameters[1] is not Value.Number(var height))
                     throw new InvalidValueTypeException(typeof(Value.Number), parameters[1].GetType(), context.Expression.PositionInText);
                     
                 FurballGame.Instance.ChangeScreenSize((int)width, (int)height);
                 DevConsole.AddMessage($"Resolution has been set to {width}x{height}", ExecutionResult.Success);
                     
                 return Value.DefaultVoid;
             }),
            
        new ("cl_set_debug_overlay_visibility", paramCount: 1, 
             (context, parameters) => {
                 if (parameters[0] is not Value.Boolean(var truthValue))
                     throw new InvalidValueTypeException(typeof(Value.Boolean), parameters[0].GetType(), context.Expression.PositionInText);

                 ConVars.DebugOverlay = truthValue;
                     
                 return Value.DefaultVoid;
             }),
            
        new ("cl_get_debug_overlay_visibility", paramCount: 0, 
             (_, _) => ConVars.DebugOverlay ? Value.DefaultTrue : Value.DefaultFalse),

        new ("cl_toggle_debug_overlay", paramCount: 0, 
             (_, _) => {
                 ConVars.DebugOverlay = !ConVars.DebugOverlay;
                     
                 return Value.DefaultVoid;
             }),
            
        new ("cl_set_write_log", paramCount: 0, 
             (context, parameters) => {
                 if (parameters[0] is not Value.Boolean(var truthValue))
                     throw new InvalidValueTypeException(typeof(Value.Boolean), parameters[0].GetType(), context.Expression.PositionInText);

                 ConVars.WriteLog = truthValue;
                     
                 return Value.DefaultVoid;
             }),
            
        new ("cl_get_write_log", paramCount: 0, 
             (_, _) => ConVars.WriteLog ? Value.DefaultTrue : Value.DefaultFalse),

        new ("cl_set_enable_tooltips", paramCount: 0, 
             (context, parameters) => {
                 if (parameters[0] is not Value.Boolean(var truthValue))
                     throw new InvalidValueTypeException(typeof(Value.Boolean), parameters[0].GetType(), context.Expression.PositionInText);

                 ConVars.ToolTips = truthValue;
                     
                 return Value.DefaultVoid;
             }),
            
        new ("cl_get_enable_tooltips", paramCount: 0, 
             (_, _) => ConVars.ToolTips ? Value.DefaultTrue : Value.DefaultFalse),
            
        new ("quit", paramCount: 0, 
             (_, _) => {
                 DevConsole.AddMessage("Bye!", ExecutionResult.Success);
                 FurballGame.Instance.WindowManager.Close();
                     
                 return Value.DefaultVoid;
             }),
            
        new ("cmr_clear_cache", paramCount: 0,
             (_, _) => {
                 ContentManager.ClearCache();

                 DevConsole.AddMessage("ContentManager cache has been cleared.", ExecutionResult.Success);
                 return Value.DefaultVoid;
             }),
            
        new ("im_input_methods", paramCount: 0,
             (_, _) => {
                 List<Value> arrayList = new();
                     
                 for (int i = 0; i != FurballGame.InputManager.RegisteredInputMethods.Count; i++) {
                     InputMethod currentMethod = FurballGame.InputManager.RegisteredInputMethods[i];

                     arrayList.Add(new Value.String(currentMethod.GetType().Name));
                 }

                 return new Value.Array(arrayList);
             }),

        new ("cl_clear", paramCount: 0,
             (_, _) => {
                 DevConsole.ConsoleLog.Clear();
                 return Value.DefaultVoid;
             }),
    };
}