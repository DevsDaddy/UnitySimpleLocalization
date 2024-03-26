using System;
using DevsDaddy.SimpleLocalization.Core.Locales;
using UnityEditor;
using UnityEngine;

namespace DevsDaddy.SimpleLocalization.Core.Editor
{
    /// <summary>
    /// Create Locale Table Window
    /// </summary>
    public class CreateLocaleTableWindow : EditorWindow
    {
        // Locale Table Name
        private string localeTableName = "";
        private LocaleTable table;
        
        // Window Sizes
        private static readonly Vector2 windowSize = new Vector2(400, 228);
        private WizzardStyles styles = new WizzardStyles();

        public Action<LocaleTable> OnComplete;
        
        // Scroll Positions
        Vector2 scrollPos;
        
        /// <summary>
        /// Show Create Locale Wizzard
        /// </summary>
        [MenuItem("Simple Localization/Tables/Create Locale Table", false, 10)]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            CreateLocaleTableWindow window = (CreateLocaleTableWindow)EditorWindow.GetWindow(typeof(CreateLocaleTableWindow));
            window.titleContent = new GUIContent("Create Locale Table", "Create a New Locale Table Window");
            window.maxSize = windowSize;
            window.minSize = windowSize;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, windowSize.x, windowSize.y);
            window.Show();
            window.CenterOnMainWin();
        }
        
        /// <summary>
        /// GUI Updates
        /// </summary>
        private void OnGUI(){
            DrawHeader();
            DrawBody();
            DrawFooter();
        }
        
        /// <summary>
        /// Draw Wizzard Header
        /// </summary>
        private void DrawHeader() {
            // Header
            GUILayout.BeginHorizontal();
            GUILayout.Label("Create Locale Table", styles.GetHeaderStyle());
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Draw Wizzard Body
        /// </summary>
        private void DrawBody() {
            GUILayout.BeginVertical(styles.GetBodyAreaStyle(), GUILayout.ExpandHeight(true));
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            DrawInputField("Please, enter New Locale Table Name", "Locale Table Name:", localeTableName, (newName) => {
                localeTableName = newName;
            });
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw Wizzard Footer
        /// </summary>
        private void DrawFooter() {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(styles.GetFooterAreaStyle());
            if (GUILayout.Button("Create Table", styles.GetFooterButtonStyle(false)))
                CompleteSetup();
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Draw Input Field
        /// </summary>
        /// <param name="placeholder"></param>
        /// <param name="label"></param>
        /// <param name="variable"></param>
        /// <param name="onComplete"></param>
        private void DrawInputField(string placeholder, string label, string variable, Action<string> onComplete = null) {
            string newText = "";
            GUILayout.BeginHorizontal(styles.GetListElementStyle());
            GUILayout.BeginVertical();
            GUILayout.Label($"<b>{label}</b>", styles.GetRegularTextStyle(TextAnchor.UpperLeft));
            newText = GUILayout.TextField(variable, 64, styles.GetBasicFieldStyle());
            GUILayout.Label($"<color=#2f2f2f>{placeholder}</color>", styles.GetRegularTextStyle(TextAnchor.UpperLeft));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            if(newText != variable)
                onComplete?.Invoke(newText);
        }

        /// <summary>
        /// On Click Save Table
        /// </summary>
        void CompleteSetup() {
            localeTableName = localeTableName.Trim();
		
            if (string.IsNullOrEmpty(localeTableName)) {
                EditorUtility.DisplayDialog("Unable to save Locale Table", "Please specify a valid Locale Table Name name.", "Close");
                return;
            }
            
            string rootPath = LocalizationWizzard.GetRoot();
            table = Resources.Load<LocaleTable>($"Locales/{localeTableName}");
            if (table != null) {
                EditorUtility.DisplayDialog("Locale Table Already Exists", "Locale Table with this name is already exists. Please specify a another Locale Table Name name.", "Close");
                return;
            }

            table = null;
            table = ScriptableObject.CreateInstance<LocaleTable>();
            table.LocaleName = localeTableName;
            LocalizationWizzard.CheckFolders();
            string pathToConfig = $"{rootPath}Resources/Locales/{localeTableName}.asset";
            AssetDatabase.CreateAsset(table, pathToConfig);
            AssetDatabase.Refresh();
            AssetDatabase.OpenAsset(table);
            OnComplete?.Invoke(table);
            Close();
        }
    }
}