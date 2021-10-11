using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Furball.Engine.Engine.Configuration {
    public static class Config {
        private static readonly Dictionary<string, object> _DefaultConfigs = new();
        private static readonly Dictionary<string, object> _UserConfigs    = new();

        public static string Filename = "furball.conf";

        public static void AddDefaultConfig(string key, object config) {
            if (_DefaultConfigs.ContainsKey(key))
                _DefaultConfigs.Remove(key);

            _DefaultConfigs.Add(key, config);
        }

        public static void SetConfig(string key, object config) {
            if (!_DefaultConfigs.ContainsKey(key)) throw new KeyNotFoundException("There is no default set for that key!");

            if (_UserConfigs.ContainsKey(key))
                _UserConfigs.Remove(key);

            _UserConfigs.Add(key, config);
        }

        public static void ResetConfig(string key) {
            _UserConfigs.Remove(key);
        }

        public static T GetConfig <T>(string key) {
            if (!_UserConfigs.TryGetValue(key, out object config))
                if (!_DefaultConfigs.TryGetValue(key, out config))
                    throw new KeyNotFoundException("There is no default set for that key!");

            config = (T)config;
            if (config is null)
                throw new TypeAccessException();

            return (T)config;
        }

        public static void Load() {
            if (!File.Exists(Filename))
                Save();

            Dictionary<string, object> tempConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(Filename));

            if (tempConfig is null)
                throw new ConfigInvalidException();

            lock (_UserConfigs) {
                _UserConfigs.Clear();

                foreach (KeyValuePair<string, object> configOption in tempConfig)
                    _UserConfigs.Add(configOption.Key, configOption.Value);
            }
        }

        public static void Save() {
            string json = JsonConvert.SerializeObject(_UserConfigs);

            File.WriteAllText(Filename, json);
        }
    }

    internal class ConfigInvalidException : Exception {
        public override string Message => "The configuration file is invalid!";
    }
}
