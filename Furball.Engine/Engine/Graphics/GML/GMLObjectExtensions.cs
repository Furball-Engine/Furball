#nullable enable
using System;
using System.Linq;
using GMLSharp;
using Object=GMLSharp.Object;

namespace Furball.Engine.Engine.Graphics.GML; 

public static class GMLObjectExtensions {
    public static bool FillWithBackgroundColor(this Object obj) {
        return obj.Properties.LastOrDefault(
               x => x is KeyValuePair {
                   Key: "fill_with_background_color"
               }
               ) is KeyValuePair {
                   Value: JsonValueNode {
                       Value: {}
                   } fillWithBackgroundColor
               } && Convert.ToBoolean(fillWithBackgroundColor.Value);
    }

    public static float? FixedWidth(this Object obj) {
        if (obj.Properties.LastOrDefault(
            node => node is KeyValuePair {
                Key: "fixed_width"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } fixedWidth
            })
            return Convert.ToSingle(fixedWidth.Value);

        return null;
    }
    
    public static string? TextAlignment(this Object obj) {
        if (obj.Properties.LastOrDefault(
            node => node is KeyValuePair {
                Key: "text_alignment"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } originType
            })
            return Convert.ToString(originType.Value);

        return null;
    }
    
    public static float? FixedHeight(this Object obj) {
        if (obj.Properties.LastOrDefault(
            node => node is KeyValuePair {
                Key: "fixed_height"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } fixedWidth
            })
            return Convert.ToSingle(fixedWidth.Value);

        return null;
    }
}
