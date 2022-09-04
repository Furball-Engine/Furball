using System;
using Furball.Engine.Engine.Helpers;
using Xunit;

namespace Furball.Tests.Helpers; 

public class BindableTests {
    private const string STRING_START_VALUE = "begin";
    private const string STRING_END_VALUE   = "end";

    private const double DOUBLE_START_VALUE = Math.PI;

    private Bindable<string> CreateStringBindable() {
        Bindable<string> bindable = new(STRING_START_VALUE);

        //Make sure that the bindable actually starts with its intended value
        Assert.True(bindable.Value == STRING_START_VALUE, "The bindable did not start with the correct value!");

        return bindable;
    }

    private Bindable<double> CreateDoubleBindable() {
        Bindable<double> bindable = new(DOUBLE_START_VALUE);

        //Make sure that the bindable actually starts with its intended value
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        Assert.True(bindable.Value == DOUBLE_START_VALUE, "The bindable did not start with the correct value!");

        return bindable;
    }

    [Fact]
    public void BindableTestOnChange() {
        Bindable<string> bindable = this.CreateStringBindable();

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        bindable.OnChange += delegate(object sender, string args) {
            Assert.True(sender == bindable,       "sender == bindable");
            Assert.True(args == STRING_END_VALUE,    "The value in the OnChange event is not what it should be!");
        };

        bindable.Value = STRING_END_VALUE;

        Assert.True(bindable.Value == STRING_END_VALUE, "The value did not change to the intended value!");
    }

    [Fact]
    public void BindableTestSameValueCheck() {
        Bindable<string> bindable = this.CreateStringBindable();

        bindable.OnChange += (sender, args) => {
            Assert.True(sender == bindable,       "sender == bindable");
            Assert.True(args == STRING_END_VALUE, "args == STRING_END_VALUE");
            Assert.True(false, "This should not be called the bindable does not change value!");
        };

        bindable.Value = STRING_START_VALUE;
    }

    [Fact]
    public void BindableTestDispose() {
        Bindable<string> bindable = this.CreateStringBindable();

        bindable.OnChange += (_, _) => {
            Assert.True(false, "Bindable.OnChange should not be called after its disposed!");
        };

        bindable.Dispose();

        bindable.Value = STRING_END_VALUE;
    }

    [Fact]
    public void BindableTestToString() {
        Bindable<double> bindable = this.CreateDoubleBindable();

        // We disable this analysis here because Bindable.ToString calls double.ToString implicity with no parameters
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        Assert.True(bindable.ToString() == DOUBLE_START_VALUE.ToString(), "Bindable.ToString returned the wrong value!");
    }

    [Fact]
    public void BindableTestImplicitConversion() {
        Bindable<double> bindableDouble = this.CreateDoubleBindable();

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        Assert.True(bindableDouble == DOUBLE_START_VALUE, "The double implicit conversion failed!");

        Bindable<string> bindableString = this.CreateStringBindable();

        Assert.True(bindableString == STRING_START_VALUE, "The string implicit conversion failed!");
    }
}
