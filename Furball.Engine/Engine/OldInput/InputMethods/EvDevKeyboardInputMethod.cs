using System;
using System.Collections.Generic;
using System.Linq;
using EvDevSharp;
using EvDevSharp.Enums;
using EvDevSharp.EventArgs;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input;
using Silk.NET.Input;

namespace Furball.Engine.Engine.OldInput.InputMethods;

public class EvDevKeyboardInputMethod : InputMethod {
    private readonly List<EvDevDevice> _trackedDevices = new();

    public override void Initialize() {
        List<EvDevDevice> devices = EvDevDevice.GetDevices().Where(x => x.GuessedDeviceType == EvDevGuessedDeviceType.Keyboard).ToList();

        for (int i = 0; i < devices.Count; i++) {
            EvDevDevice device = devices[i];
            
            FurballKeyboard keyboard = new() {
                Name         = device.Name,
                BeginInput   = () => {},
                EndInput     = () => {},
                GetClipboard = () => "",
                SetClipboard = _ => {}
            };

            this.Keyboards.Add(keyboard);
            
            device.StartMonitoring();
            this._trackedDevices.Add(device);

            int j = i;
            // device.OnKeyEvent += (_, args) => this.DeviceOnOnKeyEvent(j, args);
        }
    }
    
    // private void DeviceOnOnKeyEvent(int i, OnKeyEventArgs e) {
    //     FurballGame.GameTimeScheduler.ScheduleMethod(
    //     _ => {
    //         Key k = e.Key.ToSilk();
    //         switch (e.Type) {
    //             case EvDevKeyValue.KeyDown:
    //                 if (!FurballGame.Instance.WindowManager.Focused)
    //                     break;
    //             //         
    //             //     this.Keyboards[i].PressedKeys.Add(k);
    //             //     this.Keyboards[i].QueuedKeyPresses.Add(k);
    //             //
    //             //     //Warning: hacky shit ahead
    //             //
    //             //     if (k == Key.Space) {
    //             //         this.Keyboards[i].QueuedTextInputs.Add(' ');
    //             //         
    //             //         break;
    //             //     }
    //             //     
    //             //     string kStr  = k.ToString();
    //             //     char   kChar = char.ToLower(Convert.ToChar(kStr.Substring(0, 1)));
    //             //     if (kStr.Length == 1 && char.IsLetter(kChar)) {
    //             //         if(this.Keyboards[i].IsKeyPressed(Key.ShiftRight) || this.Keyboards[i].IsKeyPressed(Key.ShiftLeft))
    //             //             kChar = char.ToUpper(kChar);
    //             //         this.Keyboards[i].QueuedTextInputs.Add(kChar);
    //             //
    //             //         break;
    //             //     }
    //             //
    //             //     if (kStr.Length == "NumberX".Length && kStr.StartsWith("Number")) {
    //             //         this.Keyboards[i].QueuedTextInputs.Add(Convert.ToChar(kStr.TrimStart("Number".ToCharArray()).Substring(0, 1)));
    //             //
    //             //     }
    //             //     break;
    //             // case EvDevKeyValue.KeyUp:
    //             //     this.Keyboards[i].PressedKeys.Remove(k);
    //             //     this.Keyboards[i].QueuedKeyReleases.Add(k);
    //             //     break;
    //         // }
    //     });
    // }

    public override void Update() {
        
    }
    
    public override void Dispose() {
        foreach (EvDevDevice device in this._trackedDevices) {
            device.StopMonitoring();
        }
    }
}
