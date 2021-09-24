using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Furball.Engine.Engine.Graphics {
    public static class ContentReader {
        private static readonly Dictionary<string, byte[]> CONTENT_CACHE = new();

        public static int CacheSizeLimit = 4 * 1024 * 1024;//4 MB

        /// <summary>
        /// Clears the content cache, allowing changed assets to reload
        /// </summary>
        public static void ClearCache() {
            CONTENT_CACHE.Clear();
        }
        
        public static pContentType LoadMonogameAsset <pContentType>(string filename, ContentSource source = ContentSource.Game) {
            ContentManager tempManager = new(FurballGame.Instance.Content.ServiceProvider);

            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");

            if (source <= ContentSource.User) {
                tempManager.RootDirectory = Path.Combine(executablePath, "/UserContent/");

                try { return tempManager.Load<pContentType>(filename); }
                catch {/* */
                }
            }
            if (source <= ContentSource.Game) {
                tempManager.RootDirectory = FurballGame.Instance.Content.RootDirectory;

                try { return tempManager.Load<pContentType>(filename); }
                catch {/* */
                }
            }
            if (source <= ContentSource.Engine) {
                tempManager.RootDirectory = Path.Combine(executablePath, "/EngineContent/");

                try { return tempManager.Load<pContentType>(filename); }
                catch {/* */
                }
            }

            throw new FileNotFoundException();
        }

        public static byte[] LoadRawAsset(string filename, ContentSource source = ContentSource.Game) {
            if (CONTENT_CACHE.TryGetValue(filename, out byte[] cacheData))
                return cacheData;

            byte[] data = Array.Empty<byte>();

            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("shits fucked man");

            string path;
            if (source <= ContentSource.User) {
                path = Path.Combine(executablePath, "/UserContent/", filename);
                if (File.Exists(path))
                    data = File.ReadAllBytes(path);
            }
            if (source <= ContentSource.Game && data.Length == 0) {
                path = Path.Combine(FurballGame.Instance.Content.RootDirectory, filename);
                if (File.Exists(path))
                    data = File.ReadAllBytes(path);
            }
            if (source <= ContentSource.Engine && data.Length == 0) {
                path = Path.Combine(executablePath, "/EngineContent/", filename);
                if (File.Exists(path))
                    data = File.ReadAllBytes(path);
            }

            if (data.Length == 0)
                throw new FileNotFoundException("The specified content file was not found.", filename);

            //We dont want to be caching anything huge as that could cause unnessesarily high memory usage
            if (data.Length < CacheSizeLimit)
                CONTENT_CACHE.Add(filename, data);

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
        User = 2
    }
}
