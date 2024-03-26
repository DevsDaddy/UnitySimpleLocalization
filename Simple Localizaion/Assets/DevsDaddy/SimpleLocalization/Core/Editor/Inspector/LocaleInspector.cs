#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Core.Editor.Inspector
{
    [CustomEditor(typeof(Localization))]
    public class LocaleInspector : UnityEditor.Editor
    {
        private WizzardStyles styles = new WizzardStyles();
        
        public override void OnInspectorGUI() {
            if(GUILayout.Button("Open Setup Wizzard", styles.GetFooterButtonStyle(false), GUILayout.ExpandWidth(true))) {
                LocalizationWizzard dialogue = CreateLocaleTableWindow.CreateInstance<LocalizationWizzard>();
                dialogue.Show();
            }
        }
    }
}
#endif