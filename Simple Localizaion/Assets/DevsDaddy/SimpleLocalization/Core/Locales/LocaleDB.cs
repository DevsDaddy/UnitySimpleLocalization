using System.Collections.Generic;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Core.Locales
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Simple Localization/Create Localization Database", fileName = "LocalizationDatabase", order = 1)]
    public class LocaleDB : ScriptableObject
    {
        public int ConfigRevision = 0;
        public bool InitAtStartup = true;
        public string FallbackString = "Locale {0} is not found in Table {1}";
        public LocaleTable DefaultLocale = null;
        public List<LocaleTable> AvailableLocales = new List<LocaleTable>();
    }

    [System.Serializable]
    [CreateAssetMenu(menuName = "Simple Localization/Create Localization Table", fileName = "LocalizationTable",
        order = 2)]
    public class LocaleTable : ScriptableObject
    {
        public SystemLanguage LocaleCode = SystemLanguage.English;
        public string LocaleName = "English";
        public List<LocaleString> Strings = new List<LocaleString>();
    }
}