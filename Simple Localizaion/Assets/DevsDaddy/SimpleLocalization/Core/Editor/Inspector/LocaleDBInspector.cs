#if UNITY_EDITOR
using DevsDaddy.SimpleLocalization.Core.Locales;
using UnityEditor;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Core.Editor.Inspector
{
    [CustomEditor(typeof(LocaleDB))]
    public class LocaleDBInspector : UnityEditor.Editor
    {
        private WizzardStyles styles = new WizzardStyles();
        
        public override void OnInspectorGUI() {
            if(GUILayout.Button("Open Setup Wizzard", styles.GetFooterButtonStyle(false), GUILayout.ExpandWidth(true))) {
                LocalizationWizzard dialogue = CreateLocaleTableWindow.CreateInstance<LocalizationWizzard>();
                dialogue.Show();
            }
        }
    }
    
    [CustomEditor(typeof(LocaleTable))]
    public class LocaleTableInspector : UnityEditor.Editor
    {
        private WizzardStyles styles = new WizzardStyles();
        
        public override void OnInspectorGUI() {
            if(GUILayout.Button("Open Locale Table Editor", styles.GetFooterButtonStyle(false), GUILayout.ExpandWidth(true))) {
                EditLocaleTableWindow dialogue = CreateLocaleTableWindow.CreateInstance<EditLocaleTableWindow>();
                dialogue.SetActiveTable((LocaleTable)Selection.activeObject);
                dialogue.Show();
            }
        }
    }
}
#endif