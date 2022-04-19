using Furball.Engine.Engine.Helpers;
using Xunit;

namespace Furball.Tests.Helpers {
    internal class TestClass {
        
    }
    
    public class ObjectExtensionsTests {
        [Fact]
        public void IsPrimitiveTest() {
            int       primitive    = 0;
            TestClass notPrimitive = new();

            Assert.True(primitive.GetType().IsPrimitive(), "int should be considered primitive!");
            Assert.False(notPrimitive.GetType().IsPrimitive(), "TestClass should be not considered primitive!");
        }
    }
}
