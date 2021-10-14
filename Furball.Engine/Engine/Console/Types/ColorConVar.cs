using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Console.Types {
    public class ColorConVar : ConVar {
        public Bindable<Color> Value;

        public ColorConVar(string conVarName, Color initialValue) : base(conVarName) => this.Value = new Bindable<Color>(initialValue);

        public override string Set(string consoleInput) {
            string[] split = consoleInput.Split(" ");

            if (split.Length == 1) {
                this.Value.Value.FromHexString(consoleInput);
            } else if (split.Length == 3) {
                Color tempColor = new() {
                    R = byte.Parse(split[0]),
                    G = byte.Parse(split[1]),
                    B = byte.Parse(split[2]),
                    A = 255
                };

                this.Value.Value = tempColor;
            } else if (split.Length == 4) {
                Color tempColor = new() {
                    R = byte.Parse(split[0]),
                    G = byte.Parse(split[1]),
                    B = byte.Parse(split[2]),
                    A = byte.Parse(split[3])
                };

                this.Value.Value = tempColor;
            }

            base.Set(string.Empty);

            return $"{this.Name} set to {this.Value.Value.ToHexString()}!";
        }

        public override string ToString() => this.Value.Value.ToHexString();
    }
}
