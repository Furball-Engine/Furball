using System.Linq;
using Furball.Engine.Engine.Helpers;
using Xunit;

namespace Furball.Tests.Helpers {
    public class ArrayHelperTests {
        private static readonly string[] BEGIN_ARRAY = new[] {
            "A", "B"
        };
        private static readonly string[] END_ARRAY = new[] {
            "A", "A", "B", "B"
        };

        private static readonly string[] END_ARRAY_SURPLUS_TEST = new[] {
            "A", "A", "A", "B", "B"
        };

        [Fact]
        public void FitElementsInANewArray() {
            string[] final = ArrayHelper.FitElementsInANewArray(BEGIN_ARRAY, END_ARRAY.Length);

            Assert.True(final.SequenceEqual(END_ARRAY), "FitElementsInANewArray produced a wrong array!");
        }

        [Fact]
        public void FitElementsInANewArraySurplusTest() {
            string[] final = ArrayHelper.FitElementsInANewArray(BEGIN_ARRAY, END_ARRAY_SURPLUS_TEST.Length);

            Assert.True(final.SequenceEqual(END_ARRAY_SURPLUS_TEST), "FitElementsInANewArray produced a wrong array!");
        }
    }
}
