using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Kettu;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics {
    public static class ContentManager {
        private static readonly Dictionary<string, byte[]>                         CONTENT_CACHE = new();
        public static readonly  Dictionary<(FontSystem, int), DynamicSpriteFont> FSS_CACHE     = new();
        
        public static int CacheSizeLimit = 40000000;//4 MB

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
        /// <summary>
        /// Looks for `filename` and returns it if it finds it at `source`
        /// </summary>
        /// <param name="filename">Asset Name</param>
        /// <param name="source">Where to look for</param>
        /// <typeparam name="pContentType">What type is the Asset</typeparam>
        /// <returns>Asset Requested</returns>
        /// <exception cref="Exception">other stuff</exception>
        /// <exception cref="FileNotFoundException">Asset not Found</exception>
        public static pContentType LoadMonogameAsset<pContentType>(string filename, ContentSource source = ContentSource.Game) {
            Microsoft.Xna.Framework.Content.ContentManager tempManager = new(FurballGame.Instance.Content.ServiceProvider);

            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");

            if ((int)source >= (int)ContentSource.User) {
                tempManager.RootDirectory = Path.Combine(executablePath, "/UserContent/");

                try { return tempManager.Load<pContentType>(filename); }
                catch {/* */
                }
            }
            if ((int)source >= (int)ContentSource.Game) {
                tempManager.RootDirectory = FurballGame.Instance.Content.RootDirectory;

                try { return tempManager.Load<pContentType>(filename); }
                catch {/* */
                }
            }
            if ((int)source >= (int)ContentSource.Engine) {
                tempManager.RootDirectory = Path.Combine(executablePath, "/EngineContent/");

                try { return tempManager.Load<pContentType>(filename); }
                catch {/* */
                }
            }

            throw new FileNotFoundException();
        }

        public static Texture2D LoadTextureFromFile(string filename, ContentSource source = ContentSource.Game, bool bypassCache = false) 
            => Texture2D.FromStream(FurballGame.Instance.GraphicsDevice, new MemoryStream(LoadRawAsset(filename, source, bypassCache)));

        public static byte[] LoadRawAsset(string filename, ContentSource source = ContentSource.Game, bool bypassCache = false) {
            if (CONTENT_CACHE.TryGetValue(filename, out byte[] cacheData) && !bypassCache)
                return cacheData;

            byte[] data = Array.Empty<byte>();

            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");

            string path;
            if(source != ContentSource.External) {
                if ((int)source >= (int)ContentSource.User) {
                    path = Path.Combine(executablePath, "/UserContent/", filename);
                    if (File.Exists(path))
                        data = File.ReadAllBytes(path);
                }
                if ((int)source >= (int)ContentSource.Game && data.Length == 0) {
                    path = Path.Combine(FurballGame.Instance.Content.RootDirectory, filename);
                    if (File.Exists(path))
                        data = File.ReadAllBytes(path);
                }
                if ((int)source >= (int)ContentSource.Engine && data.Length == 0) {
                    path = Path.Combine(executablePath, "/EngineContent/", filename);
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
                CONTENT_CACHE.Add(filename, data);
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
