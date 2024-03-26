using System;
using System.IO;
using DevsDaddy.SimpleLocalization.Core.Constants;
using DevsDaddy.SimpleLocalization.Core.Locales;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevsDaddy.SimpleLocalization.Core.Editor
{
    /// <summary>
    /// Localization Wizzard Window
    /// </summary>
    public class LocalizationWizzard : EditorWindow
    {
        // Current Wizzard Tab
        private static LocaleDB currentConfig = null;
        private static WizzardTab currentWizzardTab = WizzardTab.Welcome;
        
        // Window Sizes
        private static readonly Vector2 minWindowSize = new Vector2(450, 600);
        private static readonly Vector2 maxWindowSize = new Vector2(550, 1024);
        
        // Styles and Images
        private WizzardStyles styles = new WizzardStyles();
        private Texture wizzardHeaderImage;

        // Modules
        private static string rootPath = "";

        // Scroll Positions
        Vector2 scrollPos;
        
        /// <summary>
        /// Show Locale Wizzard
        /// </summary>
        [MenuItem("Simple Localization/Setup Wizzard", false, 0)]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            EditorPrefs.SetBool(GeneralConstants.EDITOR_WIZZARD_KEY, true);
            LocalizationWizzard window = (LocalizationWizzard)EditorWindow.GetWindow(typeof(LocalizationWizzard));
            window.titleContent = new GUIContent(GeneralStrings.SETUP_WIZZARD_TITLE, GeneralStrings.SETUP_WIZZARD_HINT);
            window.maxSize = maxWindowSize;
            window.minSize = minWindowSize;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, minWindowSize.x, minWindowSize.y);
            window.Show();
            window.CenterOnMainWin();
            window.SwitchTab(WizzardTab.Welcome);
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
            // Load Image
            if (wizzardHeaderImage == null)
                wizzardHeaderImage = Resources.Load<Texture>(GeneralConstants.EDITOR_WIZZARD_HEADER);
            
            // Draw Window BG
            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), styles.GetBGTexture(), ScaleMode.StretchToFill, true);
            
            // Draw Texture
            float ratio = wizzardHeaderImage.height / wizzardHeaderImage.width;
            float w = position.width;
            float h = position.width / wizzardHeaderImage.width * wizzardHeaderImage.height;
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(new Rect(0, 0, w, h), wizzardHeaderImage, ScaleMode.StretchToFill, true, ratio);
            GUILayout.EndHorizontal();
            GUILayout.Space(h);
            
            // Header
            GUILayout.BeginHorizontal();
            GUILayout.Label(GetTabText(), styles.GetHeaderStyle());
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Draw Wizzard Body
        /// </summary>
        private void DrawBody() {
            if (currentWizzardTab == WizzardTab.Welcome)
                DrawWelcomeScreen();
            if (currentWizzardTab == WizzardTab.GeneralSetup)
                DrawConfigEditor();
            if (currentWizzardTab == WizzardTab.Complete)
                DrawCompleteSetup();
        }
        
        /// <summary>
        /// Draw Welcome Screen
        /// </summary>
        private void DrawWelcomeScreen() {
            GUILayout.BeginVertical(styles.GetBodyAreaStyle(), GUILayout.ExpandHeight(true));
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Label($"{GeneralStrings.VERSION_TITLE} {GeneralConstants.PACKAGE_VERSION}", styles.GetRegularTextStyle(TextAnchor.MiddleCenter));
            GUILayout.Space(20);
            GUILayout.Label(GeneralStrings.SETUP_WIZZARD_THANKS, styles.GetRegularTextStyle(TextAnchor.MiddleCenter));
            GUILayout.Space(10);
            if(GUILayout.Button("Open GitHub", styles.GetBasicButtonSyle())) {
                Application.OpenURL("https://github.com/DevsDaddy/UnitySimpleLocalization");
            }
            if(GUILayout.Button("Check New Versions", styles.GetBasicButtonSyle())) {
                Application.OpenURL("https://github.com/DevsDaddy/UnitySimpleLocalization/releases");
            }
            if(GUILayout.Button("Join Discord", styles.GetBasicButtonSyle())) {
                Application.OpenURL("https://discord.gg/xuNTKRDebx");
            }
            if(GUILayout.Button("Report a Bug", styles.GetBasicButtonSyle())) {
                Application.OpenURL("https://github.com/DevsDaddy/UnitySimpleLocalization/issues");
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw Config Editor
        /// </summary>
        private void DrawConfigEditor() {
            GUILayout.BeginVertical(styles.GetBodyAreaStyle(), GUILayout.ExpandHeight(true));
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Label("GENERAL", styles.GetSubHeaderStyle());
            GUILayout.BeginHorizontal();
            DrawObjectField(0, "Choose Default Language Table", "Default Language Table:", currentConfig.DefaultLocale,
                (obj) => { currentConfig.DefaultLocale = (LocaleTable)obj; });
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            DrawToggleListElement("<b>Initialize at Startup</b>", "Initialization of the localization system at the start of the game. (Otherwise, you need to call the initialization method).", currentConfig.InitAtStartup,
                () => {
                    currentConfig.InitAtStartup = !currentConfig.InitAtStartup;
                });
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            DrawInputField("Type your fallback message for lost locale in table", "<b>Fallback String:</b>", currentConfig.FallbackString, fallback => currentConfig.FallbackString = fallback);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.Label($"LANGUAGE TABLES ({currentConfig.AvailableLocales.Count})", styles.GetSubHeaderStyle());
            DrawTables();
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Add Exists", styles.GetFooterButtonStyle(true), GUILayout.ExpandWidth(false))) {
                currentConfig.AvailableLocales.Add(null);
            }
            if(GUILayout.Button("Create Table", styles.GetFooterButtonStyle(false), GUILayout.ExpandWidth(false))) {
                CreateLocaleTableWindow dialogue = CreateLocaleTableWindow.CreateInstance<CreateLocaleTableWindow>();
                dialogue.Show();
                dialogue.OnComplete = (newTable) => {
                    currentConfig.AvailableLocales.Add(newTable);
                };
            }
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draw Tables
        /// </summary>
        private void DrawTables() {
            if (currentConfig.AvailableLocales.Count > 0)
                for (int i = 0; i < currentConfig.AvailableLocales.Count; i++) {
                    DrawTable(i);
                }
            else {
                GUILayout.Label("No languages found. Please, add at least one language.", styles.GetWarningTextStyle(TextAnchor.MiddleCenter));
            }
        }

        /// <summary>
        /// Draw Locale Table Editor
        /// </summary>
        /// <param name="index"></param>
        private void DrawTable(int index) {
            DrawObjectField(index, "Choose Language Table", "Language Table:", currentConfig.AvailableLocales[index],
                (obj) => { currentConfig.AvailableLocales[index] = (LocaleTable)obj; }, () => {
                    currentConfig.AvailableLocales.RemoveAt(index);
                });
        }
        
        /// <summary>
        /// Draw Complete Setup
        /// </summary>
        private void DrawCompleteSetup() {
            GUILayout.BeginVertical(styles.GetBodyAreaStyle(), GUILayout.ExpandHeight(true));
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Label("AND NOW IS DONE", styles.GetSubHeaderStyle());
            GUILayout.Label("Your Simple Localization System is now ready to go.\n\n<b>Do not remove the SIMPLE_LOCALE object from the scene!</b>", styles.GetRegularTextStyle());
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw Wizzard Footer
        /// </summary>
        private void DrawFooter() {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(styles.GetFooterAreaStyle());
            if (currentWizzardTab == WizzardTab.GeneralSetup || currentWizzardTab == WizzardTab.Complete) {
                if(GUILayout.Button("Go Back", styles.GetFooterButtonStyle(true))) {
                    SwitchTab((currentWizzardTab == WizzardTab.GeneralSetup) ? WizzardTab.Welcome : WizzardTab.GeneralSetup);
                }
            }
            if (currentWizzardTab == WizzardTab.Welcome || currentWizzardTab == WizzardTab.GeneralSetup) {
                if(GUILayout.Button("Continue", styles.GetFooterButtonStyle(false))) {
                    SwitchTab(currentWizzardTab = (currentWizzardTab == WizzardTab.Welcome) ? WizzardTab.GeneralSetup : WizzardTab.Complete);
                }
            }
            if (currentWizzardTab == WizzardTab.Complete) {
                if (GUILayout.Button("Complete Setup", styles.GetFooterButtonStyle(false)))
                    CompleteSetup();
            }
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
        /// Draw Input Field
        /// </summary>
        /// <param name="index"></param>
        /// <param name="placeholder"></param>
        /// <param name="label"></param>
        /// <param name="variable"></param>
        /// <param name="onComplete"></param>
        /// <param name="onRemove"></param>
        private void DrawObjectField(int index, string placeholder, string label, LocaleTable variable, Action<LocaleTable> onComplete = null, Action onRemove = null) {
            Object newObject = null;
            GUILayout.BeginHorizontal(styles.GetTableStyle(index % 2 == 0));
            GUILayout.BeginVertical();
            GUILayout.Label($"<b>{label}</b>", styles.GetRegularTextStyle(TextAnchor.UpperLeft));
            newObject = (LocaleTable)EditorGUILayout.ObjectField(variable, typeof(LocaleTable), false);
            GUILayout.Space(10);
            if (onRemove != null) {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove", styles.GetBasicButtonSyle(true), GUILayout.ExpandWidth(false))) {
                    onRemove?.Invoke();
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Edit", styles.GetBasicButtonSyle(true), GUILayout.ExpandWidth(false))) {
                    AssetDatabase.OpenAsset(variable);
                    EditLocaleTableWindow editor = EditLocaleTableWindow.CreateInstance<EditLocaleTableWindow>();
                    editor.SetActiveTable(variable);
                    editor.Show();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            if(newObject != variable)
                onComplete?.Invoke((LocaleTable)newObject);
        }
        
        /// <summary>
        /// Draw Toggle List Element
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="toggleVariable"></param>
        /// <param name="onToggle"></param>
        private void DrawToggleListElement(string title, string description, bool toggleVariable, Action onToggle) {
            GUILayout.BeginHorizontal(styles.GetListElementStyle());
            GUILayout.BeginVertical();
            if (GUILayout.Button("", styles.GetSwitchButtonStyle(toggleVariable), GUILayout.ExpandWidth(false))) {
                onToggle?.Invoke();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(title, styles.GetRegularTextStyle(TextAnchor.UpperLeft));
            GUILayout.Label(description, styles.GetRegularTextStyle(TextAnchor.UpperLeft));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Complete Setup
        /// </summary>
        private void CompleteSetup() {
            // Save Configs
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Add GameObject on Scene
            CreateLocalizationWorker();
            
            // Close Window
            Debug.Log($"{GeneralStrings.LOG_PREFIX} Setup <color=green><b>is Done</b></color>");
            Close();
        }

        [MenuItem("Simple Localization/Create Localization Worker", false, 20)]
        public static void CreateLocalizationWorker() {
            Localization worker = FindObjectOfType<Localization>();
            if (worker == null) {
                GameObject workerObject = new GameObject("__SIMPLE_LOCALE__");
                workerObject.AddComponent<Localization>();
                workerObject.transform.SetAsFirstSibling();
                Debug.Log($"{GeneralStrings.LOG_PREFIX} Added Worker GameObject at Current Scene");
            }
            else {
                Selection.activeObject = worker;
            }
        }
        
        /// <summary>
        /// Initialize Wizzard
        /// </summary>
        [InitializeOnLoadMethod]
        private static void InitializeWizzard() {
            // Get Config
            if (string.IsNullOrEmpty(rootPath)) rootPath = GetRoot();
            bool isWizzardShown = EditorPrefs.GetBool(GeneralConstants.EDITOR_WIZZARD_KEY, false);
            if (!isWizzardShown) {
                currentWizzardTab = WizzardTab.Welcome;
                Init();
            }
        }
        
        [MenuItem("Simple Localization/Utils/Reset Setup Wizzard", false, 99)]
        private static void ResetWizzard() {
            EditorPrefs.SetBool(GeneralConstants.EDITOR_WIZZARD_KEY, false);
        }
        
        /// <summary>
        /// Switch Wizzard to Tab 
        /// </summary>
        /// <param name="tab"></param>
        private void SwitchTab(WizzardTab tab) {
            currentWizzardTab = tab;
            scrollPos = new Vector2(0, 0);

            if (currentWizzardTab == WizzardTab.GeneralSetup) {
                GetConfig();
                currentConfig.ConfigRevision += 1;
                EditorUtility.SetDirty(currentConfig);
            }
        }
        
        /// <summary>
        /// Get Tab Text
        /// </summary>
        /// <returns></returns>
        private string GetTabText() {
            switch (currentWizzardTab) {
                case WizzardTab.Welcome:
                    return "Welcome to Simple Locale!";
                case WizzardTab.GeneralSetup:
                    return "Configure Simple Locale";
                case WizzardTab.Complete:
                    return "Complete Setup";
            }
            
            return "";
        }
        
        /// <summary>
        /// Get Script Path for
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetRoot() {
            var asset = "";
            var guids = AssetDatabase.FindAssets( string.Format( "{0} t:script", nameof(LocalizationWizzard)));
            
            if ( guids.Length > 1 ) {
                foreach ( var guid in guids ) {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var filename = Path.GetFileNameWithoutExtension( assetPath );
                    if ( filename == nameof(LocalizationWizzard) ) {
                        asset = guid;
                        break;
                    }
                }
            } else if ( guids.Length == 1 ) {
                asset = guids [0];
            } else {
                Debug.LogErrorFormat("Unable to locate {0}", nameof(LocalizationWizzard));
                return null;
            }
            
            string path = AssetDatabase.GUIDToAssetPath (asset);
            string relative = path.Replace($"Core/Editor/{nameof(LocalizationWizzard)}.cs", "");
            return relative;
        }
        
        /// <summary>
        /// Get Current Config
        /// </summary>
        private static void GetConfig() {
            if (string.IsNullOrEmpty(rootPath)) rootPath = GetRoot();
            
            // Load Configurations
            LocaleDB config = Resources.Load<LocaleDB>(GeneralConstants.CONFIG_PATH);
            if (config == null) {
                currentConfig = CreateInstance<LocaleDB>();
                LocaleTable table = ScriptableObject.CreateInstance<LocaleTable>();
                table.LocaleCode = SystemLanguage.English;
                table.LocaleName = "English";
                CheckFolders();
                string pathToTable = $"{rootPath}Resources/Locales/English.asset";
                AssetDatabase.CreateAsset(table, pathToTable);
                AssetDatabase.Refresh();
                currentConfig.DefaultLocale = table;
                currentConfig.AvailableLocales.Add(table);
                string pathToConfig = $"{rootPath}Resources/{GeneralConstants.CONFIG_PATH}.asset";
                AssetDatabase.CreateAsset(currentConfig, pathToConfig);
                AssetDatabase.Refresh();
                Debug.Log($"{GeneralStrings.LOG_PREFIX} Config Created At: {pathToConfig}");
                return;
            }

            currentConfig = config;
            
            // Generate key if Null
            Debug.Log($"{GeneralStrings.LOG_PREFIX} Loaded Config: {currentConfig}");
        }

        /// <summary>
        /// Check Folders
        /// </summary>
        public static void CheckFolders() {
            if (string.IsNullOrEmpty(rootPath)) rootPath = GetRoot();
            string baseFolder = $"{rootPath}Resources/";
            string localesFolder = $"{rootPath}Resources/Locales/";
            if (!Directory.Exists(baseFolder)) Directory.CreateDirectory(baseFolder);
            if (!Directory.Exists(localesFolder)) Directory.CreateDirectory(localesFolder);
        }
    }
}