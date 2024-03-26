using System;
using DevsDaddy.SimpleLocalization.Core.Locales;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevsDaddy.SimpleLocalization.Core.Editor
{
    public class EditLocaleTableWindow : EditorWindow
    {
        private LocaleTable table;
        
        // Window Sizes
        private static readonly Vector2 minWindowSize = new Vector2(600, 450);
        private static readonly Vector2 maxWindowSize = new Vector2(1024, 550);
        private WizzardStyles styles = new WizzardStyles();
        
        public Action<LocaleTable> OnComplete;
        public Action<LocaleTable> OnTableChanged;
        
        // Scroll Positions
        Vector2 scrollPos;
        
        /// <summary>
        /// Show Locale Editor
        /// </summary>
        [MenuItem("Simple Localization/Tables/Edit Locale Table", false, 11)]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            EditLocaleTableWindow window = (EditLocaleTableWindow)EditorWindow.GetWindow(typeof(EditLocaleTableWindow));
            window.titleContent = new GUIContent("Edit Locale Table", "Edit Exists Localization Table or Create a new locale table");
            window.maxSize = maxWindowSize;
            window.minSize = minWindowSize;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, minWindowSize.x, minWindowSize.y);
            window.Show();
            window.CenterOnMainWin();
        }
        
        /// <summary>
        /// GUI Updates
        /// </summary>
        private void OnGUI(){
            DrawHeader();
            DrawSubHeader();
            DrawBody();
            DrawFooter();
        }

        /// <summary>
        /// Draw Header
        /// </summary>
        private void DrawHeader() {
            GUILayout.BeginHorizontal(styles.GetSubHeaderStyle(), GUILayout.ExpandWidth(true));
            GUILayout.Label("<b>Current Table:</b>", styles.GetRegularTextStyle(TextAnchor.MiddleLeft, new RectOffset(0,0,10,10)), GUILayout.Height(25));
            GUILayout.Space(10);
            DrawObjectField(table, SetActiveTable);
            GUILayout.Space(20);
            if (GUILayout.Button("Create New", styles.GetBasicButtonSyle(true), GUILayout.ExpandWidth(false))) {
                CreateLocaleTableWindow dialogue = CreateLocaleTableWindow.CreateInstance<CreateLocaleTableWindow>();
                dialogue.Show();
                dialogue.OnComplete = SetActiveTable;
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw Subheader
        /// </summary>
        private void DrawSubHeader() {
            if (table != null) {
                GUILayout.BeginHorizontal(styles.GetBodyAreaStyle(), GUILayout.ExpandWidth(true));
                GUILayout.Label("<b>Locale Name:</b>", styles.GetRegularTextStyle(TextAnchor.MiddleLeft, new RectOffset(0,0,10,10)), GUILayout.Height(25));
                GUILayout.Space(10);
                DrawInputField(table.LocaleName, -1, newName => table.LocaleName = newName);
                GUILayout.Space(20);
                GUILayout.Label("<b>Locale Code:</b>", styles.GetRegularTextStyle(TextAnchor.MiddleLeft, new RectOffset(0,0,10,10)), GUILayout.Height(25));
                GUILayout.Space(10);
                DrawSelectorField(table.LocaleCode, newCode => table.LocaleCode = newCode);
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draw Body
        /// </summary>
        private void DrawBody() {
            GUILayout.BeginVertical(styles.GetBodyAreaStyle(), GUILayout.ExpandHeight(true));
            if (table == null) {
                GUILayout.Space(20);
                GUILayout.Label("No Localization Table Asset is selected.\nSelect Localization Table Asset in the field above or in your project folder, or create a new Localization Table Asset.", styles.GetWarningTextStyle(TextAnchor.MiddleCenter));
                GUILayout.Space(20);
                if(GUILayout.Button("Create New Table", styles.GetBasicButtonSyle())) {
                    CreateLocaleTableWindow dialogue = CreateLocaleTableWindow.CreateInstance<CreateLocaleTableWindow>();
                    dialogue.Show();
                    dialogue.OnComplete = SetActiveTable;
                }
            }
            else {
                GUILayout.Label($"LOCALES EDITOR ({table.Strings.Count} Items)", styles.GetSubHeaderStyle());
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                DrawEditor();
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draw Locale Editor
        /// </summary>
        private void DrawEditor() {
            if (table.Strings.Count < 1) {
                GUILayout.BeginVertical(styles.GetBodyAreaStyle());
                GUILayout.Label("No Localization Items available for this Language.\nAdd New Item to this Localization Table", styles.GetWarningTextStyle(TextAnchor.MiddleCenter));
                GUILayout.EndVertical();
            }
            for (int i = 0; i < table.Strings.Count; i++) {
                DrawElementEditor(i);
            }
        }

        /// <summary>
        /// Draw Element Editor
        /// </summary>
        /// <param name="index"></param>
        private void DrawElementEditor(int index) {
            GUILayout.BeginHorizontal(styles.GetTableStyle(index % 2 == 0), GUILayout.ExpandWidth(false));
            GUILayout.BeginVertical();
            GUILayout.Label("<b>Code:</b>", styles.GetRegularTextStyle(TextAnchor.MiddleLeft), GUILayout.Width(50));
            if (GUILayout.Button("Remove", styles.GetBasicButtonSyle(true), GUILayout.ExpandWidth(false))) {
                table.Strings.RemoveAt(index);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUILayout.EndVertical();
            GUILayout.Space(10);
            DrawInputField(table.Strings[index].Code, 200, newValue => {
                table.Strings[index].Code = newValue;
            });
            GUILayout.Space(20);
            GUILayout.Label("<b>Value:</b>", styles.GetRegularTextStyle(TextAnchor.MiddleLeft), GUILayout.Width(50));
            GUILayout.Space(10);
            DrawInputField(table.Strings[index].Value, -1, newValue => {
                table.Strings[index].Value = newValue;
            });

            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Draw Wizzard Footer
        /// </summary>
        private void DrawFooter() {
            if (table != null) {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal(styles.GetFooterAreaStyle());
                if (GUILayout.Button("Add New Item", styles.GetFooterButtonStyle(true))) {
                    table.Strings.Add(new LocaleString {
                        Code = "NewItem",
                        Value = ""
                    });
                }
                if (GUILayout.Button("Save Table", styles.GetFooterButtonStyle(false)))
                    CompleteSetup();
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// On Asset Selection Change
        /// </summary>
        private void OnSelectionChange() {
            if (Selection.activeObject is LocaleTable) {
                if (table == null || (LocaleTable)Selection.activeObject != table) {
                    SetActiveTable((LocaleTable)Selection.activeObject);
                    if(focusedWindow != this)
                        Focus();
                }
            }
        }

        /// <summary>
        /// Set Active Table
        /// </summary>
        /// <param name="newTable"></param>
        public void SetActiveTable(LocaleTable newTable) {
            table = newTable;
            OnTableChanged?.Invoke(table);
        }

        /// <summary>
        /// Get Active Table
        /// </summary>
        /// <returns></returns>
        public LocaleTable GetActiveTable() {
            return table;
        }
        
        /// <summary>
        /// On Click Save Table
        /// </summary>
        void CompleteSetup() {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.OpenAsset(table);
            OnComplete?.Invoke(table);
            Close();
        }
        
        /// <summary>
        /// Draw Object Field
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="onComplete"></param>
        private void DrawObjectField(LocaleTable variable, Action<LocaleTable> onComplete = null) {
            EditorGUILayout.BeginVertical();
            Object newObject = null;
            newObject = (LocaleTable)EditorGUILayout.ObjectField(variable, typeof(LocaleTable), false, GUILayout.Height(25));
            if(newObject != variable)
                onComplete?.Invoke((LocaleTable)newObject);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw Input Field
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="width"></param>
        /// <param name="onComplete"></param>
        private void DrawInputField(string variable, int width = -1, Action<string> onComplete = null) {
            EditorGUILayout.BeginVertical();
            string newText = "";
            if (width > 0) {
                newText = GUILayout.TextArea(variable, styles.GetBasicFieldStyle(), GUILayout.MinHeight(25), GUILayout.MaxWidth(width), GUILayout.Width(width), GUILayout.ExpandHeight(true));
            }
            else {
                newText = GUILayout.TextArea(variable, styles.GetBasicFieldStyle(), GUILayout.MinHeight(25), GUILayout.ExpandHeight(true));
            }
            if(newText != variable)
                onComplete?.Invoke(newText);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw Selector Field
        /// </summary>
        /// <param name="languageEnum"></param>
        /// <param name="onComplete"></param>
        private void DrawSelectorField(SystemLanguage languageEnum, Action<SystemLanguage> onComplete = null) {
            SystemLanguage newLanguage = SystemLanguage.English;
            EditorGUILayout.BeginVertical(GUILayout.Height(25));
            newLanguage = (SystemLanguage)EditorGUILayout.EnumPopup(languageEnum, styles.GetBasicFieldStyle(), GUILayout.Height(25));
            if(newLanguage != languageEnum)
                onComplete?.Invoke((SystemLanguage)newLanguage);
            EditorGUILayout.EndHorizontal();
        }
    }
}