using System.Collections.Generic;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Core.Locales
{
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