using System;
using Furball.Engine.Engine.Helpers;
using Xunit;

namespace Furball.Tests.Helpers {
    public class ObjectExtensionsTests {
        [Fact]
        public void IsPrimitiveTest() {
            int  primitive    = 0;
            Half notPrimitive = Half.Epsilon;

            Assert.True(primitive.GetType().IsPrimitive(), "int should be considered primitive!");
            Assert.False(notPrimitive.GetType().IsPrimitive(), "Half should be not considered primitive!");
        }
    }
}
