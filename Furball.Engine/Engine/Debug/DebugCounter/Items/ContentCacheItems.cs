using Furball.Engine.Engine.Graphics;


namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    /// <summary>
    /// Displays the total amount of cached items in the Content Cache
    /// </summary>
    public class ContentCacheItems : DebugCounterItem {
        public override string GetAsString(GameTime time) => $"cci: {ContentManager.ContentCacheItems + ContentManager.FSSCacheItems}";
    }
}
