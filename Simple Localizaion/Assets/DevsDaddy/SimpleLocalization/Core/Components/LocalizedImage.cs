using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevsDaddy.SimpleLocalization.Core.Components
{
    /// <summary>
    /// Localized uGUI Image
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class LocalizedImage : MonoBehaviour
    {
        [Header("Localized UI Image Settings")]
        public List<LocalizedImageData> LocalizedSprites = new List<LocalizedImageData>();
        public Sprite fallbackSprite;

        // Component Reference
        private Image component;
        
        // Localized Image Data
        [System.Serializable]
        public class LocalizedImageData
        {
            public SystemLanguage Language;
            public Sprite LocalizedSprite;
        }
        
        /// <summary>
        /// On Object Awake
        /// </summary>
        private void Awake() {
            component = GetComponent<Image>();
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
            foreach (var localizedData in LocalizedSprites) {
                if (localizedData.Language == Localization.Main.GetCurrentLanguage().LocaleCode) {
                    component.sprite = localizedData.LocalizedSprite;
                    return;
                }
            }

            component.sprite = fallbackSprite;
        }
    }
}