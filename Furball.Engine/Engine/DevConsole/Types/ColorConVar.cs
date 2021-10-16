using System;
using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class ColorConVar : ConVar {
        public Bindable<Color> Value;

        public ColorConVar(string conVarName, Color initialValue, Action onChange = null) : base(conVarName, onChange) => this.Value = new Bindable<Color>(initialValue);

        public override (ExecutionResult result, string message) Set(string consoleInput) {
            string[] split = consoleInput.Split(" ");

            try {
                switch (split.Length) {
                    case 1:
                        this.Value.Value.FromHexString(consoleInput);
                        break;
                    case 3: {
                        Color tempColor = new() {
                            R = byte.Parse(split[0]),
                            G = byte.Parse(split[1]),
                            B = byte.Parse(split[2]),
                            A = 255
                        };

                        this.Value.Value = tempColor;
                        break;
                    }
                    case 4: {
                        Color tempColor = new() {
                            R = byte.Parse(split[0]),
                            G = byte.Parse(split[1]),
                            B = byte.Parse(split[2]),
                            A = byte.Parse(split[3])
                        };

                        this.Value.Value = tempColor;
                        break;
                    }
                    default:
                        return (ExecutionResult.Error, "Invalid Syntax.");
                }
            } catch (ArgumentException) {
                return (ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return (ExecutionResult.Error, "Failed to parse input into a +color");
            } catch (OverflowException) {
                return (ExecutionResult.Error, "Number parsed is too big to fit into a +color+byte");
            }

            base.Set(string.Empty);

            return (ExecutionResult.Success, $"{this.Name} set to {this.Value.Value.ToHexString()}!");
        }

        public override string ToString() => this.Value.Value.ToHexString();
    }
}
