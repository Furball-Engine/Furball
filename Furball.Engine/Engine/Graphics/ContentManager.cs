using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Vixie;
using Kettu;
using SixLabors.Fonts;

namespace Furball.Engine.Engine.Graphics; 

public static class ContentManager {
    private static readonly Dictionary<string, WeakReference<byte[]>>                                             ContentCache = new();
    private static readonly Dictionary<string, WeakReference<Texture>>                                            TextureCache = new();
    public static readonly  Dictionary<(FontSystem FontSystem, float fontSize), WeakReference<DynamicSpriteFont>> FssCache     = new();
    public static           string                                                                                ContentPath  = "Content";

    public static int CacheSizeLimit = 100000000;// 8 MB

    public static int ContentCacheItems => ContentCache.Count;
    public static int FssCacheItems => FssCache.Count;
    public static int TextureCacheItems => TextureCache.Count;

    /// <summary>
    /// Clears the content cache, allowing changed assets to reload
    /// </summary>
    public static void ClearCache() {
        ContentCache.Clear();
        FssCache.Clear();

        Logger.Log("Content cache cleared!", LoggerLevelCacheEvent.Instance);
    }

    internal static void Initialize() {
        FurballGame.TimeStepMethods.Add(
        new FixedTimeStepMethod(
        5000,
        () => {
            ContentCache.RemoveAll((key, value) => !value.TryGetTarget(out _));
            TextureCache.RemoveAll((key, value) => !value.TryGetTarget(out _));
        }
        )
        );
    }

    private static readonly Dictionary<string, FontSystem> SystemFontCache = new();
    public static FontSystem LoadSystemFont(string familyName, FontStyle style = FontStyle.Regular, FontSystemSettings settings = null, bool disableCache = false) {
        if (!disableCache && SystemFontCache.TryGetValue(familyName, out FontSystem sys))
            return sys;

        //If we cant find the font, just give them the default one, ittl work
        if (!SystemFonts.TryGet(familyName, out FontFamily font))
        {
            Logger.Log($"Could not load system font by family name \"{familyName}\", loading default font instead", LoggerLevelEngineInfo.Instance);
            return FurballGame.DefaultFont;
        }

        Font regularFont = font.CreateFont(30, style);

        //If we cant find the font *in the particualr style*, then just return the default font
        if (!regularFont.TryGetPath(out string path))
            return FurballGame.DefaultFont;

        FontSystem system = settings != null 
                                ? new FontSystem(settings) 
                                : new FontSystem();

        system.AddFont(File.ReadAllBytes(path));
        
        SystemFontCache.Add(familyName, system);

        return system;
    }

    public static Texture LoadTextureFromFileCached(string filename, ContentSource source = ContentSource.Game) {
        if (TextureCache.TryGetValue(filename, out WeakReference<Texture> reference)) {
            if (reference.TryGetTarget(out Texture refTex))
                return refTex;

            TextureCache.Remove(filename);
        }

        Texture tex = Texture.CreateTextureFromStream(new MemoryStream(LoadRawAsset(filename, source)));
        tex.Name = filename;
        
        TextureCache[filename] = new WeakReference<Texture>(tex);

        return tex;
    }
        
    public static Texture LoadTextureFromFile(string filename, ContentSource source = ContentSource.Game, bool bypassCache = false) {
        Texture tex = Texture.CreateTextureFromStream(new MemoryStream(LoadRawAsset(filename, source, bypassCache)));
        tex.Name = filename;
        
        return tex;
    }

    public static byte[] LoadRawAsset(string filename, ContentSource source = ContentSource.Game, bool bypassCache = false) {
        if (ContentCache.TryGetValue(filename, out WeakReference<byte[]> cacheReference) && !bypassCache) {
            if (cacheReference.TryGetTarget(out byte[] cacheData))
                return cacheData;

            //If we fail to get the data from the weak reference, then remove the reference from the cache
            ContentCache.Remove(filename);
        }

        byte[] data = Array.Empty<byte>();

        if(source != ContentSource.External) {
            string path;
            if ((int)source >= (int)ContentSource.User) {
                path = Path.Combine(FurballGame.AssemblyPath, "UserContent/", filename);
                if (File.Exists(path))
                    data = File.ReadAllBytes(path);
            }
            if ((int)source >= (int)ContentSource.Game && data.Length == 0) {
                path = Path.Combine(FurballGame.AssemblyPath, ContentPath, filename);
                if (File.Exists(path))
                    data = File.ReadAllBytes(path);
            }
            if ((int)source >= (int)ContentSource.Engine && data.Length == 0) {
                path = Path.Combine(FurballGame.AssemblyPath, "EngineContent/", filename);
                if (File.Exists(path))
                    data = File.ReadAllBytes(path);
            }
        } else {
            if (File.Exists(filename))
                data = File.ReadAllBytes(filename);
        }
            
        if (data.Length == 0)
            throw new FileNotFoundException("The specified content file was not found.", filename);

        //We dont want to be caching anything huge as that could cause unnessesarily high memory usage
        if (data.Length < CacheSizeLimit && !bypassCache) {
            Logger.Log($"Caching content with filepath: {filename}, hash:{CryptoHelper.GetMd5(data)}, dataSize:{data.LongLength}", LoggerLevelCacheEvent.Instance);
            ContentCache.Add(filename, new WeakReference<byte[]>(data));
        }

        return data;
    }
}

public enum ContentSource {
    /// <summary>
    /// Only load from the Engine's content
    /// </summary>
    Engine = 0,
    /// <summary>
    /// Load the Game's custom content, overrides the Engine
    /// </summary>
    Game = 1,
    /// <summary>
    /// Loads the User's custom content, overrides both Game and Engine
    /// </summary>
    User = 2,
    /// <summary>
    /// Loads data outside of the main content folders, the root is the working directory(?)
    /// </summary>
    External = 3,
}