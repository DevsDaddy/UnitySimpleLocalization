using UnityEngine;
using UnityEngine.UI;

namespace DevsDaddy.SimpleLocalization.Core.Components
{
    /// <summary>
    /// Localized uGUI Text Component
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [Header("Locale Code from Table")]
        public string LocaleCode = "";
        public bool UseFallbackFromOriginal = true;

        private Text component;

        /// <summary>
        /// On Object Awake
        /// </summary>
        private void Awake() {
            component = GetComponent<Text>();
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
            component.text = Localization.Main.GetLocalized(LocaleCode, UseFallbackFromOriginal ? component.text : "");
        }
    }
}