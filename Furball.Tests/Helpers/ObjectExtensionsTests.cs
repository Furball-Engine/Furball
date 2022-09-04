using System.Collections.Generic;
using Furball.Engine.Engine.Helpers;
using Xunit;

namespace Furball.Tests.Helpers;

internal class TestClass {
    public string String;
}

public class ObjectExtensionsTests {
    [Fact]
    public void IsPrimitiveTest() {
        int       primitive    = 0;
        TestClass notPrimitive = new();
        string    str          = "";

        Assert.True(primitive.GetType().IsPrimitive(), "int should be considered primitive!");
        Assert.False(notPrimitive.GetType().IsPrimitive(), "TestClass should be not considered primitive!");
        Assert.True(str.GetType().IsPrimitive(), "string should be primitive!");
    }

    [Fact]
    public void DictionaryRemoveAllTest() {
        Dictionary<string, string> dict = new();
        
        dict.Add("foo", "bar");
        dict.Add("baz", "bar");
        dict.Add("bam", "pow");
        dict.Add("pow", "baz");
        
        //Remove all elements with a value of 'bar'
        dict.RemoveAll((key, value) => value == "bar");
        
        Assert.True(dict.Count == 2, "We should end with 2 items!");
        
        Assert.DoesNotContain(dict, pair => pair.Value == "bar");
    }

    [Fact]
    public void CopyObjectTest() {
        TestClass instance = new() {String = "str"};

        TestClass instanceCopy = instance.Copy();
        
        Assert.NotEqual(instance, instanceCopy);
        Assert.Equal(instance.String, instanceCopy.String);

        instance.String = "baz";
        
        Assert.Equal("str", instanceCopy.String);
    }
}
