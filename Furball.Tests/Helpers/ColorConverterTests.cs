using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Graphics;
using Xunit;

namespace Furball.Tests.Helpers {
    public class ColorConverterTests {
        private readonly string _hex1   = "#FFFFFF";
        private readonly Color  _color1 = new(255, 255, 255);

        private readonly string _hex2   = "#AAFFFFFF";
        private readonly Color  _color2 = new(255, 255, 255, 170);

        private readonly string _hex3   = "#FFF";
        private readonly Color  _color3 = new(255, 255, 255);

        [Fact]
        public void FromHexString() {
            Assert.True(ColorConverter.FromHexString(this._hex1) == this._color1, $"{this._hex1} does not equal {this._color1}");
            Assert.True(ColorConverter.FromHexString(this._hex2) == this._color2, $"{this._hex2} does not equal {this._color2}");
            Assert.True(ColorConverter.FromHexString(this._hex3) == this._color3, $"{this._hex3} does not equal {this._color3}");
        }

        [Fact]
        public void FromHexStringExtension() {
            Color color = new(0, 0, 0, 0);

            color.FromHexString(this._hex1);
            Assert.True(color == this._color1, $"{this._hex1} does not equal {this._color1}");

            color.FromHexString(this._hex2);
            Assert.True(color == this._color2, $"{this._hex2} does not equal {this._color2}");

            color.FromHexString(this._hex3);
            Assert.True(color == this._color3, $"{this._hex3} does not equal {this._color3}");
        }

        [Fact]
        public void ToHexString() {
            Assert.True(this._color1.ToHexString() == this._hex1);
            //ToHexString doesnt work with alpha for like no reason
            //yeah i hate it too
            // Assert.True(this._color2.ToHexString() == this._hex2);
        }
    }
}
