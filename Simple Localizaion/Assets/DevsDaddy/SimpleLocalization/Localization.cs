using System.Collections.Generic;
using DevsDaddy.SimpleLocalization.Core.Constants;
using DevsDaddy.SimpleLocalization.Core.Locales;
using UnityEngine;
using UnityEngine.Events;

namespace DevsDaddy.SimpleLocalization
{
    /// <summary>
    /// Simple Localization System
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public sealed class Localization : MonoBehaviour
    {
        // Constructor
        private Localization(){}
        private static Localization instance;
        public static Localization Main => instance;
        
        // Utils Parameters
        private LocaleDB currentConfig;
        private LocaleTable currentTable;
        private bool isQuitting = false;
        
        // Events
        [Header("Localization Events")] 
        public UnityEvent OnInitialized = new UnityEvent();
        public UnityEvent OnConfigChanged = new UnityEvent();
        public UnityEvent OnLanguageSwitched = new UnityEvent();
        
        // Temporary Items
        private readonly Dictionary<string, string> locales = new Dictionary<string, string>();
        private int currentLocaleIndex = -1;

        /// <summary>
        /// On Localization Worker Awake
        /// </summary>
        private void Awake() {
            if (instance == null) {
                instance = this;
                transform.SetParent(null);
                transform.SetAsFirstSibling();
                DontDestroyOnLoad(this);
            }
            else {
                Destroy(this);
                return;
            }
            
            FindConfiguration();
            BindEvents();
        }

        /// <summary>
        /// On Localization Worker Started
        /// </summary>
        private void Start() {
            if(currentConfig.InitAtStartup)
                Initialize();
        }

        /// <summary>
        /// Initialize Localization Worker
        /// </summary>
        /// <param name="configuration"></param>
        public void Initialize(LocaleDB configuration = null) {
            // Change Config if is Provided
            if (configuration != null && configuration != currentConfig) {
                currentConfig = configuration;
                OnConfigChanged?.Invoke();
            }

            currentTable = GetUserLanguage();
            if (currentTable == null) DetectCurrentLanguage();
            if (currentTable == null) currentTable = currentConfig.DefaultLocale;
            LoadTempLocales();
            GetCurrentLocaleIndex();
            OnInitialized?.Invoke();
            OnLanguageSwitched?.Invoke();
        }

        /// <summary>
        /// Switch Language
        /// </summary>
        /// <param name="table"></param>
        public void SwitchLanguage(LocaleTable table) {
            for (int i = 0; i < currentConfig.AvailableLocales.Count; i++) {
                if (currentConfig.AvailableLocales[i] == table) {
                    currentTable = table;
                    currentLocaleIndex = i;
                    OnLanguageSwitched?.Invoke();
                    SaveUserLanguage(i);
                    return;
                }
            }

            Debug.LogWarning($"{GeneralStrings.LOG_PREFIX} Failed to switch language. Language table is not found in Available locales list.");
        }

        /// <summary>
        /// Switch Language
        /// </summary>
        /// <param name="localeTableIndex"></param>
        public void SwitchLanguage(int localeTableIndex) {
            if (localeTableIndex > currentConfig.AvailableLocales.Count - 1) {
                Debug.LogWarning($"{GeneralStrings.LOG_PREFIX} Failed to switch language. Language table with this index is not found.");
                return;
            }
            
            currentTable = currentConfig.AvailableLocales[localeTableIndex];
            LoadTempLocales();
            currentLocaleIndex = localeTableIndex;
            OnLanguageSwitched?.Invoke();
            SaveUserLanguage(localeTableIndex);
        }

        /// <summary>
        /// Get Locale by Index
        /// </summary>
        /// <param name="localeTableIndex"></param>
        /// <returns></returns>
        public LocaleTable GetLocaleByIndex(int localeTableIndex) {
            return currentConfig.AvailableLocales?[localeTableIndex] ?? null;
        }

        /// <summary>
        /// Get Current Locale Index
        /// </summary>
        /// <returns></returns>
        public int GetCurrentLocaleIndex() {
            currentLocaleIndex = -1;
            for (int i = 0; i < currentConfig.AvailableLocales.Count; i++) {
                if (currentConfig.AvailableLocales[i] == currentTable) {
                    currentLocaleIndex = i;
                    return currentLocaleIndex;
                }
            }

            return currentLocaleIndex;
        }

        /// <summary>
        /// Get Available Locale Tables
        /// </summary>
        /// <returns></returns>
        public List<LocaleTable> GetLocaleTables() {
            return currentConfig.AvailableLocales;
        }

        /// <summary>
        /// Get Current Language Table
        /// </summary>
        /// <returns></returns>
        public LocaleTable GetCurrentLanguage() {
            return currentTable;
        }
        
        /// <summary>
        /// Get Localized Item
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public string GetLocalized(string code, string fallback = "") {
            if (locales.TryGetValue(code, out var localized)) {
                return localized;
            }

            return (string.IsNullOrEmpty(fallback))
                ? string.Format(currentConfig.FallbackString, code, currentTable.LocaleName)
                : string.Format(fallback, code, currentTable.LocaleName);
        }

        /// <summary>
        /// On Localization Worker Destroy
        /// </summary>
        private void OnDestroy() {
            if(isQuitting)
                UnbindEvents();
        }

        /// <summary>
        /// On Application Quit
        /// </summary>
        private void OnApplicationQuit() {
            isQuitting = true;
        }

        /// <summary>
        /// Bind Localization Worker Events
        /// </summary>
        private void BindEvents() {
        }

        /// <summary>
        /// Unbind Localization Worker Events
        /// </summary>
        private void UnbindEvents() {
            OnInitialized?.RemoveAllListeners();
            OnConfigChanged?.RemoveAllListeners();
            OnLanguageSwitched?.RemoveAllListeners();
        }

        /// <summary>
        /// Find Configuration
        /// </summary>
        private void FindConfiguration() {
            currentConfig = Resources.Load<LocaleDB>(GeneralConstants.CONFIG_PATH);
            if (currentConfig == null) {
                Debug.LogWarning($"{GeneralStrings.LOG_PREFIX} Localization Configuration is not found. Please, run Setup Wizzard or Initialize Localization manual using {nameof(Initialize)} method.");
                return;
            }
            
            OnConfigChanged?.Invoke();
        }

        /// <summary>
        /// Get User Language
        /// </summary>
        /// <returns></returns>
        private LocaleTable GetUserLanguage() {
            int currentIndex = PlayerPrefs.GetInt("SLS_USER_LANGUAGE", -1);
            if (currentIndex == -1) return null;
            return GetLocaleByIndex(currentIndex);
        }

        /// <summary>
        /// Save User Language
        /// </summary>
        /// <param name="index"></param>
        private void SaveUserLanguage(int index) {
            PlayerPrefs.SetInt("SLS_USER_LANGUAGE", index);
        }

        /// <summary>
        /// Detect Current Language
        /// </summary>
        /// <returns></returns>
        private LocaleTable DetectCurrentLanguage() {
            SystemLanguage deviceLanguage = Application.systemLanguage;
            foreach (var locale in currentConfig.AvailableLocales) {
                if (locale.LocaleCode == deviceLanguage) {
                    return locale;
                }
            }

            return null;
        }

        /// <summary>
        /// Load Temporary Locales
        /// </summary>
        private void LoadTempLocales() {
            locales.Clear();
            foreach (var localeString in currentTable.Strings)
                locales.Add(localeString.Code, localeString.Value);
        }
    }
}