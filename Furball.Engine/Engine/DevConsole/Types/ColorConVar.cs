using System;
using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class ColorConVar : ConVar {
        public Bindable<Color> BindableValue;
        public Color Value {
            get {
                return this.BindableValue.Value;
            }
            set {
                this.BindableValue.Value = value;
            }
        }

        public ColorConVar(string conVarName, Color initialValue, Action onChange = null) : base(conVarName, onChange) => this.BindableValue = new Bindable<Color>(initialValue);

        public override ConsoleResult Set(string consoleInput) {
            string[] split = consoleInput.Split(" ");

            try {
                switch (split.Length) {
                    case 1:
                        this.Value.FromHexString(consoleInput);
                        break;
                    case 3: {
                        Color tempColor = new() {
                            R = byte.Parse(split[0]),
                            G = byte.Parse(split[1]),
                            B = byte.Parse(split[2]),
                            A = 255
                        };

                        this.Value = tempColor;
                        break;
                    }
                    case 4: {
                        Color tempColor = new() {
                            R = byte.Parse(split[0]),
                            G = byte.Parse(split[1]),
                            B = byte.Parse(split[2]),
                            A = byte.Parse(split[3])
                        };

                        this.Value = tempColor;
                        break;
                    }
                    default:
                        return new ConsoleResult(ExecutionResult.Error, "Invalid Syntax.");
                }
            } catch (ArgumentException) {
                return new ConsoleResult(ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return new ConsoleResult(ExecutionResult.Error, "Failed to parse input into a +color");
            } catch (OverflowException) {
                return new ConsoleResult(ExecutionResult.Error, "Number parsed is too big to fit into a +color+byte");
            }

            base.Set(string.Empty);

            return new ConsoleResult(ExecutionResult.Success, $"{this.Name} set to {this.Value.ToHexString()}!");
        }

        public override string ToString() => this.Value.ToHexString();
    }
}
