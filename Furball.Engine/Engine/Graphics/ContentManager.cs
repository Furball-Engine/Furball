using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Kettu;
using SixLabors.Fonts;

namespace Furball.Engine.Engine.Graphics {
    public static class ContentManager {
        private static readonly Dictionary<string, WeakReference<byte[]>>        CONTENT_CACHE = new();
        public static readonly  Dictionary<(FontSystem, int), DynamicSpriteFont> FSS_CACHE     = new();
        public static           string                                           ContentPath   = "Content";

        public static int CacheSizeLimit = 100000000;// 8 MB

        public static int ContentCacheItems => CONTENT_CACHE.Count;
        public static int FSSCacheItems => FSS_CACHE.Count;

        /// <summary>
        /// Clears the content cache, allowing changed assets to reload
        /// </summary>
        public static void ClearCache() {
            CONTENT_CACHE.Clear();
            FSS_CACHE.Clear();

            Logger.Log("Content cache cleared!", LoggerLevelCacheEvent.Instance);
        }

        public static FontSystem LoadSystemFont(string familyName, FontSystemSettings settings = null) {
            SystemFonts.TryGet(familyName, out FontFamily font);

            font.TryGetPaths(out IEnumerable<string> paths);

            FontSystem system;
            if (settings != null)
                system = new FontSystem(settings);
            else
                system = new FontSystem();

            foreach (string path in paths)
                system.AddFont(File.OpenRead(path));

            return system;
        }

        public static Texture LoadTextureFromFile(string filename, ContentSource source = ContentSource.Game, bool bypassCache = false)
            => Resources.CreateTexture(new MemoryStream(LoadRawAsset(filename, source, bypassCache)));

        public static byte[] LoadRawAsset(string filename, ContentSource source = ContentSource.Game, bool bypassCache = false) {
            if (CONTENT_CACHE.TryGetValue(filename, out WeakReference<byte[]> cacheReference) && !bypassCache) {
                if (cacheReference.TryGetTarget(out byte[] cacheData))
                    return cacheData;

                //If we fail to get the data from the weak reference, then remove the reference from the cache
                CONTENT_CACHE.Remove(filename);
            }

            byte[] data = Array.Empty<byte>();

            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");

            string path;
            if(source != ContentSource.External) {
                if ((int)source >= (int)ContentSource.User) {
                    path = Path.Combine(executablePath, "UserContent/", filename);
                    if (File.Exists(path))
                        data = File.ReadAllBytes(path);
                }
                if ((int)source >= (int)ContentSource.Game && data.Length == 0) {
                    path = Path.Combine(ContentPath, filename);
                    if (File.Exists(path))
                        data = File.ReadAllBytes(path);
                }
                if ((int)source >= (int)ContentSource.Engine && data.Length == 0) {
                    path = Path.Combine(executablePath, "EngineContent/", filename);
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
                CONTENT_CACHE.Add(filename, new WeakReference<byte[]>(data));
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
}
