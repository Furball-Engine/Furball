using Furball.Engine.Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public class ContentCacheItems : DebugCounterItem {
        public override string GetAsString(GameTime time) => $"cci: {ContentManager.ContentCacheItems + ContentManager.FSSCacheItems}";
    }
}
