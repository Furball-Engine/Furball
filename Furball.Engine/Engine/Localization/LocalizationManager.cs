using System;
using System.IO;
using System.Collections.Generic;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Localization.Exceptions;
using Furball.Engine.Engine.Localization.Languages;

namespace Furball.Engine.Engine.Localization {
    public class LocalizationManager {
        private static readonly Dictionary<(string translationKey, ISO639_2Code code), string> _translations = new();
        
        public static readonly Dictionary<ISO639_2Code, Type> Languages = new();

        public static readonly Language DefaultLanguage = new EnglishLanguage();
        public static          Language CurrentLanguage = DefaultLanguage;

        public static string GetLocalizedString(string key, ISO639_2Code code = ISO639_2Code.und) {
            if (code == ISO639_2Code.und)
                code = CurrentLanguage.Iso6392Code();

            if (_translations.TryGetValue((key, code), out string localization)) {
                return localization;
            }
            
            if(_translations.TryGetValue((key, DefaultLanguage.Iso6392Code()), out localization)) {
                return localization;
            }

            throw new NoTranslationException();
        }

        public static List<ISO639_2Code> GetSupportedLanguages() {
            List<ISO639_2Code> languages = new();
            
            foreach (KeyValuePair<(string translationKey, ISO639_2Code code), string> translation in _translations) {
                if (languages.Contains(translation.Key.code)) continue;
                
                languages.Add(translation.Key.code);
            }

            return languages;
        }

        public static Language GetLanguageFromCode(ISO639_2Code code) {
            if (Languages.TryGetValue(code, out Type type)) {
                return (Language)Activator.CreateInstance(type);
            }

            return null;
        }
        
        public static void AddDefaultTranslation(string key, string contents) {
            _translations.Add((key, DefaultLanguage.Iso6392Code()), contents);
        }
        
        public static void ReadTranslations() {
            Languages.Add(ISO639_2Code.eng, typeof(EnglishLanguage));
            Languages.Add(ISO639_2Code.jbo, typeof(LojbanLanguage));
            
            string localizationFolder = Path.Combine(FurballGame.AssemblyPath, FurballGame.LocalizationFolder);
            
            DirectoryInfo dirInfo = null;
            
            if (!Directory.Exists(localizationFolder)) {
                dirInfo = Directory.CreateDirectory(localizationFolder);
            }

            dirInfo ??= new DirectoryInfo(localizationFolder);
            
            IEnumerable<FileInfo> langFiles = dirInfo.EnumerateFiles("*.lang", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in langFiles) {
                StreamReader stream = file.OpenText();

                ISO639_2Code code = ISO639_2Code.und;
                
                string line;
                //Iterate through all lines of file
                while ((line = stream.ReadLine()) != null) {
                    if(line.Trim().Length == 0) continue;
                    
                    string[] splitLine = line.Split("=");

                    //Checks if the first section is LanguageCode, which defines the language of the file
                    if (splitLine[0] == "LanguageCode") {
                        try {
                            //Parse the language code
                            Enum.TryParse(splitLine[1], true, out code);
                        } catch {
                            break;
                        }
                    } else {
                        (string translationKey, ISO639_2Code languageCode) key = (splitLine[0], code);
                        
                        _translations.Add(key, splitLine[1].Trim());
                    }
                }

                Logger.Log($"Parsed localization for {GetLanguageFromCode(code)}", new LoggerLevelLocalizationInfo());
            }
        }
    }
}
