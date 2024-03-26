using System;
using TMPro;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Demo.Scripts
{
    /// <summary>
    /// Localization Switcher Dropdown
    /// </summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    internal class LocalizationSwitcher : MonoBehaviour
    {

        private TMP_Dropdown dropdown;
        
        /// <summary>
        /// On Dropdown Awake
        /// </summary>
        private void Awake() {
            dropdown = GetComponent<TMP_Dropdown>();
            BindEvents();
        }

        /// <summary>
        /// On Dropdown Destroy
        /// </summary>
        private void OnDestroy() {
            UnbindEvents();
        }

        /// <summary>
        /// Bind Events
        /// </summary>
        private void BindEvents() {
            Localization.Main.OnInitialized?.AddListener(OnLocalizationInitialized);
            Localization.Main.OnLanguageSwitched?.AddListener(OnLocalizationSwitched);
            dropdown.onValueChanged?.AddListener(OnSwitchedChanged);
        }
        
        /// <summary>
        /// Unbind Events
        /// </summary>
        private void UnbindEvents() {
            Localization.Main.OnInitialized?.RemoveListener(OnLocalizationInitialized);
            Localization.Main.OnLanguageSwitched?.RemoveListener(OnLocalizationSwitched);
            dropdown.onValueChanged?.RemoveListener(OnSwitchedChanged);
        }

        /// <summary>
        /// On Localization Initialized
        /// </summary>
        private void OnLocalizationInitialized() {
            dropdown.ClearOptions();
            foreach (var localeTable in Localization.Main.GetLocaleTables()) {
                dropdown.options.Add(new TMP_Dropdown.OptionData {
                    image = null,
                    text = localeTable.LocaleName
                });
            }

            OnLocalizationSwitched();
        }

        /// <summary>
        /// On Localization Switched
        /// </summary>
        private void OnLocalizationSwitched() {
            dropdown.SetValueWithoutNotify(Localization.Main.GetCurrentLocaleIndex());
        }

        /// <summary>
        /// On Localization Dropdown Changed
        /// </summary>
        /// <param name="index"></param>
        private void OnSwitchedChanged(int index) {
            Localization.Main.SwitchLanguage(index);
        }
    }
}