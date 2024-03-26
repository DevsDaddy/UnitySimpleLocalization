using TMPro;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Core.Components
{
    /// <summary>
    /// Localized Text Mesh Pro UGUI
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTMProUGUI : MonoBehaviour
    {
        [Header("Locale Code from Table")]
        public string LocaleCode = "";
        public bool UseFallbackFromOriginal = true;

        private TextMeshProUGUI component;

        /// <summary>
        /// On Object Awake
        /// </summary>
        private void Awake() {
            component = GetComponent<TextMeshProUGUI>();
            BindEvents();
        }

        /// <summary>
        /// On Object Destroy
        /// </summary>
        private void OnDestroy() {
            UnbindEvents();
        }

        /// <summary>
        /// Bind Events
        /// </summary>
        private void BindEvents() {
            Localization.Main.OnLanguageSwitched?.AddListener(OnLocalizationSwitched);
        }

        /// <summary>
        /// Unbind Events
        /// </summary>
        private void UnbindEvents() {
            Localization.Main.OnLanguageSwitched?.RemoveListener(OnLocalizationSwitched);
        }

        /// <summary>
        /// On Localization Switched
        /// </summary>
        private void OnLocalizationSwitched() {
            component.SetText(Localization.Main.GetLocalized(LocaleCode, UseFallbackFromOriginal ? component.text : ""));
        }
    }
}