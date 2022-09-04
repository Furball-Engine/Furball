using Furball.Engine.Engine.Graphics;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

/// <summary>
/// Displays the total amount of cached items in the Content Cache
/// </summary>
public class ContentCacheItems : DebugCounterItem {
    public override string GetAsString(double time) => $"cci: {ContentManager.ContentCacheItems:N0} tci: {ContentManager.TextureCacheItems:N0} fci: {ContentManager.FssCacheItems:N0}";
}