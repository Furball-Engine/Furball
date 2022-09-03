using System.Linq;
using Furball.Engine.Engine.Helpers;
using Xunit;

namespace Furball.Tests.Helpers {
    public class ArrayHelperTests {
        private static readonly string[] BeginArray = new[] {
            "A", "B"
        };
        private static readonly string[] EndArray = new[] {
            "A", "A", "B", "B"
        };

        private static readonly string[] EndArraySurplusTest = new[] {
            "A", "A", "A", "B", "B"
        };

        [Fact]
        public void FitElementsInANewArray() {
            string[] final = ArrayHelper.FitElementsInANewArray(BeginArray, EndArray.Length);

            Assert.True(final.SequenceEqual(EndArray), "FitElementsInANewArray produced a wrong array!");
        }

        [Fact]
        public void FitElementsInANewArraySurplusTest() {
            string[] final = ArrayHelper.FitElementsInANewArray(BeginArray, EndArraySurplusTest.Length);

            Assert.True(final.SequenceEqual(EndArraySurplusTest), "FitElementsInANewArray produced a wrong array!");
        }
    }
}
