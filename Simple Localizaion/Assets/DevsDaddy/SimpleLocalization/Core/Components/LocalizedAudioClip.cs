using System.Collections.Generic;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Core.Components
{
    /// <summary>
    /// Localized Audio Clip
    /// </summary>
    public class LocalizedAudioClip : MonoBehaviour
    {
        [Header("Localized Audio Clips")]
        public List<LocalizedAudioClipData> LocalizedSprites = new List<LocalizedAudioClipData>();
        public AudioClip fallbackClip;

        // Current Clip
        private AudioClip currentClip;
        
        // Localized Audio Data
        [System.Serializable]
        public class LocalizedAudioClipData
        {
            public SystemLanguage Language;
            public AudioClip LocalizedClip;
        }

        /// <summary>
        /// Get Current Clip
        /// </summary>
        /// <returns></returns>
        public AudioClip GetCurrentClip() {
            return currentClip ? currentClip : fallbackClip;
        }
        
        /// <summary>
        /// On Object Awake
        /// </summary>
        private void Awake() {
            currentClip = fallbackClip;
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
                    currentClip = localizedData.LocalizedClip;
                    return;
                }
            }

            currentClip = fallbackClip;
        }
    }
}