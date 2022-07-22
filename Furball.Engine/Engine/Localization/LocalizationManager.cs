using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Furball.Engine.Engine.Config;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Localization.Exceptions;
using Furball.Engine.Engine.Localization.Languages;
using Furball.Volpe.Evaluation;
using JetBrains.Annotations;
using Kettu;

namespace Furball.Engine.Engine.Localization; 

public class LocalizationManager {
    private static readonly Dictionary<(string translationKey, ISO639_2Code code), string> TRANSLATIONS = new();

    public static Dictionary<ISO639_2Code, Type> Languages = new();

    public static  Language DefaultLanguage  = new EnglishLanguage();
    private static Language _CurrentLanguage = DefaultLanguage;

    [NotNull]
    public static Language CurrentLanguage {
        get => _CurrentLanguage;
        set {
            FurballConfig.Instance.Values["language"] = new Value.String(value.Iso6392Code().ToString());

            CultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(value.IetfLanguageTag());
            
            CultureInfo.CurrentUICulture = CultureInfo;
            CultureInfo.CurrentCulture   = CultureInfo;
            
            _CurrentLanguage = value;
            
            LanguageChanged?.Invoke(null, value);
        }
    }

    public static event EventHandler<Language> LanguageChanged;
    
    public static CultureInfo CultureInfo {
        get;
        private set;
    }

    [Pure]
    public static string GetLocalizedString(object key, Language language = null) {
        if (language == null)
            language = DefaultLanguage;

        return GetLocalizedString(key, language.Iso6392Code());
    }

    [Pure]
    public static string GetLocalizedString(object key) => GetLocalizedString(key, ISO639_2Code.und);

    [Pure]
    public static string GetLocalizedString(object key, ISO639_2Code code = ISO639_2Code.und) {
        if (code == ISO639_2Code.und)
            code = CurrentLanguage.Iso6392Code();

        if (TRANSLATIONS.TryGetValue((key.ToString(), code), out string localization))
            return localization;

        if (TRANSLATIONS.TryGetValue((key.ToString(), DefaultLanguage.Iso6392Code()), out localization))
            return localization;

        throw new NoTranslationException();
    }

    public static double GetLocalizationCompletion(ISO639_2Code code) {
        int total    = 0;
        int complete = 0;

        foreach (KeyValuePair<(string translationKey, ISO639_2Code code),string> pair in TRANSLATIONS) {
            if (pair.Key.code == DefaultLanguage.Iso6392Code())
                total++;
            if (pair.Key.code == code)
                complete++;
        }

        return (double)complete / total;
    }

    [Pure]
    public static List<ISO639_2Code> GetSupportedLanguages() {
        List<ISO639_2Code> languages = new();

        foreach (KeyValuePair<(string translationKey, ISO639_2Code code), string> translation in TRANSLATIONS) {
            if (languages.Contains(translation.Key.code)) continue;
                
            languages.Add(translation.Key.code);
        }

        return languages;
    }

    [Pure]
    [CanBeNull]
    public static Language GetLanguageFromCode(ISO639_2Code code) {
        if (Languages.TryGetValue(code, out Type type)) {
            return (Language)Activator.CreateInstance(type);
        }

        return null;
    }

    public static void AddDefaultTranslation(object key, string contents) {
        TRANSLATIONS.Add((key.ToString(), DefaultLanguage.Iso6392Code()), contents);
    }
        
    public static void ReadTranslations() {
        Languages.Add(ISO639_2Code.eng, typeof(EnglishLanguage));
        Languages.Add(ISO639_2Code.jbo, typeof(LojbanLanguage));
        Languages.Add(ISO639_2Code.epo, typeof(EsperantoLanguage));
        Languages.Add(ISO639_2Code.pol, typeof(PolishLanguage));
        Languages.Add(ISO639_2Code.deu, typeof(GermanLanguage));
        Languages.Add(ISO639_2Code.jpn, typeof(JapaneseLanguage));
        Languages.Add(ISO639_2Code.spa, typeof(SpanishLanguage));
        Languages.Add(ISO639_2Code.ara, typeof(ArabicLanguage));
        Languages.Add(ISO639_2Code.ita, typeof(ItalianLanguage));
        Languages.Add(ISO639_2Code.fra, typeof(FrenchLanguage));

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
                if(line.Trim().Length == 0 || line.StartsWith("#")) continue;
                    
                string[] splitLine = line.Split('=');

                //Checks if the first section is LanguageCode, which defines the language of the file
                if (splitLine[0] == "LanguageCode") {
                    try {
                        //Parse the language code
                        Enum.TryParse(splitLine[1], true, out code);
                        Logger.Log($"Reading language file for {code}", LoggerLevelLocalizationInfo.Instance);
                    } catch {
                        break;
                    }
                } else {
                    (string translationKey, ISO639_2Code languageCode) key = (splitLine[0], code);

                    TRANSLATIONS.Add(key, splitLine[1].Trim().Replace("\\n", "\n"));
                }
            }

            Logger.Log($"Parsed localization for {GetLanguageFromCode(code)}", LoggerLevelLocalizationInfo.Instance);
        }
    }
}