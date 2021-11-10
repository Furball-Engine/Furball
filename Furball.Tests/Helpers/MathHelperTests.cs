using System;
using System.Numerics;
using Xunit;
using MathHelper=Furball.Engine.Engine.Helpers.MathHelper;

namespace Furball.Tests.Helpers {
    public class MathHelperTests {
        private const double START_VALUE         = 0d;
        private const double END_VALUE           = 1d;
        private const double MIDDLE_VALUE        = 0.5d;
        private const double THREE_FOURTHS_VALUE = 0.75d;
        private const double ONE_FOURTHS_VALUE   = 0.25d;

        [Fact]
        public void LerpDoubleTest() {
            Assert.True(MathHelper.Lerp(START_VALUE, END_VALUE, MIDDLE_VALUE) == MIDDLE_VALUE,               "Middle lerp returned wrong value!");
            Assert.True(MathHelper.Lerp(START_VALUE, END_VALUE, THREE_FOURTHS_VALUE) == THREE_FOURTHS_VALUE, "3/4th lerp returned wrong value!");
            Assert.True(MathHelper.Lerp(START_VALUE, END_VALUE, ONE_FOURTHS_VALUE) == ONE_FOURTHS_VALUE,     "1/4th lerp returned wrong value!");
        }

        [Fact]
        public void LerpVector2Test() {
            Assert.True(
            MathHelper.Lerp(new Vector2((float)START_VALUE), new Vector2((float)END_VALUE), (float)MIDDLE_VALUE) == new Vector2((float)MIDDLE_VALUE),
            "Middle lerp returned wrong value!"
            );
            Assert.True(
            MathHelper.Lerp(new Vector2((float)START_VALUE), new Vector2((float)END_VALUE), (float)THREE_FOURTHS_VALUE) == new Vector2((float)THREE_FOURTHS_VALUE),
            "Middle lerp returned wrong value!"
            );
            Assert.True(
            MathHelper.Lerp(new Vector2((float)START_VALUE), new Vector2((float)END_VALUE), (float)ONE_FOURTHS_VALUE) == new Vector2((float)ONE_FOURTHS_VALUE),
            "Middle lerp returned wrong value!"
            );
        }

        [Fact]
        public void DegreesToRadiansTest() {
            Assert.True(MathHelper.DegreesToRadians(180d) == Math.PI,      "180 degrees is not PI radians!");
            Assert.True(MathHelper.DegreesToRadians(360d) == Math.PI * 2d, "360 degrees is not 2PI radians!");
        }
    }
}
