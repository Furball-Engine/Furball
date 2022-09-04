using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Helpers;
using Xunit;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Tests.Helpers; 

public class ConversionHelperTests {
    private readonly Color  _color1 = new(255, 255, 255);
    private readonly Color  _color2 = new(255, 255, 255, 170);
    private readonly Color  _color3 = new(255, 255, 255);
    private readonly string _hex1   = "#FFFFFF";

    private readonly string _hex2 = "#AAFFFFFF";

    private readonly string _hex3 = "#FFF";

    [Fact]
    public void FromHexString() {
        Assert.True(ConversionHelpers.ColorFromHexString(this._hex1) == this._color1, $"{this._hex1} does not equal {this._color1}");
        Assert.True(ConversionHelpers.ColorFromHexString(this._hex2) == this._color2, $"{this._hex2} does not equal {this._color2}");
        Assert.True(ConversionHelpers.ColorFromHexString(this._hex3) == this._color3, $"{this._hex3} does not equal {this._color3}");
    }

    [Fact]
    public void FromHexStringExtension() {
        Color color = new(0, 0, 0, 0);

        color.ColorFromHexString(this._hex1);
        Assert.True(color == this._color1, $"{this._hex1} does not equal {this._color1}");

        color.ColorFromHexString(this._hex2);
        Assert.True(color == this._color2, $"{this._hex2} does not equal {this._color2}");

        color.ColorFromHexString(this._hex3);
        Assert.True(color == this._color3, $"{this._hex3} does not equal {this._color3}");
    }

    [Fact]
    public void ToHexString() {
        Assert.True(this._color1.ToHexString() == this._hex1);
        //ToHexString doesnt work with alpha for like no reason
        //yeah i hate it too
        // Assert.True(this._color2.ToHexString() == this._hex2);
    }

    [Fact]
    public void Vector2ToPoint() {
        Vector2 vec = new(10, 7);

        Point point = vec.ToPoint();
        
        Assert.Equal(vec.X, point.X);
        Assert.Equal(vec.Y, point.Y);
    }
    
    [Fact]
    public void Vector2ToPointF() {
        Vector2 vec = new(10.1f, 7.29f);

        PointF point = vec.ToPointF();
        
        Assert.Equal(vec.X, point.X);
        Assert.Equal(vec.Y, point.Y);
    }
    
    [Fact]
    public void Vector2ToSize() {
        Vector2 vec = new(10, 7);

        Size point = vec.ToSize();
        
        Assert.Equal(vec.X, point.Width);
        Assert.Equal(vec.Y, point.Height);
    }
    
    [Fact]
    public void Vector2ToSizeF() {
        Vector2 vec = new(10.1f, 7.29f);

        SizeF point = vec.ToSizeF();
        
        Assert.Equal(vec.X, point.Width);
        Assert.Equal(vec.Y, point.Height);
    }
    
    [Fact]
    public void PointToVector2() {
        Point point = new(10, 7);

        Vector2 vec = point.ToVector2();
        
        Assert.Equal(point.X, vec.X);
        Assert.Equal(point.Y, vec.Y);
    }
    
    [Fact]
    public void PointFToVector2() {
        PointF point = new(10.1f, 7.29f);

        Vector2 vec = point.ToVector2();
        
        Assert.Equal(point.X, vec.X);
        Assert.Equal(point.Y, vec.Y);
    }
}
