//----------------------------------------------
// Flip Web Apps: Prefs Editor
// Copyright © 2016-2017 Flip Web Apps / Mark Hewitt
//
// Please direct any bugs/comments/suggestions to http://www.flipwebapps.com
// 
// The copyright owner grants to the end user a non-exclusive, worldwide, and perpetual license to this Asset
// to integrate only as incorporated and embedded components of electronic games and interactive media and 
// distribute such electronic game and interactive media. End user may modify Assets. End user may otherwise 
// not reproduce, distribute, sublicense, rent, lease or lend the Assets. It is emphasized that the end 
// user shall not be entitled to distribute or transfer in any way (including, without, limitation by way of 
// sublicense) the Assets in any other way than as integrated components of electronic games and interactive media. 

// The above copyright notice and this permission notice must not be removed from any files.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace PrefsEditor.Editor
{
    /// <summary>
    /// Editor window for player preferences.
    /// </summary>
    public class PrefsEditorWindow : EditorWindow
    {
         readonly string[] _tabs = { "Prefs", "Editor Prefs" };
        int _currentTab;

        readonly string[] _systemEntries = { "UnityGraphicsQuality", "Screenmanager Resolution Width", "Screenmanager Is Fullscreen mode" };
        enum ItemType { Int, Float, String }

        Texture2D _newIcon;
        Texture2D _saveIcon;
        Texture2D _refreshIcon;
        Texture2D _deleteIcon;
        Texture2D _lockIcon;
        Texture2D _redTexture;

        Vector2 _scrollPosition = Vector2.zero;

        [SerializeField]
        string _passPhrase = "";


        readonly List<PrefsEntry> _playerPrefsEntries = new List<PrefsEntry>();
        string _playerPrefsFilter;
        [SerializeField]
        bool _showNew;
        string _newItemKey;
        ItemType _newItemType;
        int _newItemValueInt;
        float _newItemValueFloat;
        string _newItemValueString;
        bool _newItemEncrypted;

        readonly List<PrefsEntry> _editorPrefsEntries = new List<PrefsEntry>();
        string _editorPrefsFilter;
        [SerializeField]
        bool _showNewEditor;
        string _newItemKeyEditor;
        ItemType _newItemTypeEditor;
        int _newItemValueIntEditor;
        float _newItemValueFloatEditor;
        string _newItemValueStringEditor;

        // Add menu item for showing the window
        [MenuItem("Window/Prefs Editor")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            //var prefsEditorWindow = 
            GetWindow< PrefsEditorWindow>("Prefs Editor", true);
        }


        void OnEnable()
        {
            _newIcon = AssetDatabase.LoadAssetAtPath(@"Assets\FlipWebApps\PrefsEditor\Sprites\New.png", typeof(Texture2D)) as Texture2D;
            _saveIcon = AssetDatabase.LoadAssetAtPath(@"Assets\FlipWebApps\PrefsEditor\Sprites\Save.png", typeof(Texture2D)) as Texture2D;
            _refreshIcon = AssetDatabase.LoadAssetAtPath(@"Assets\FlipWebApps\PrefsEditor\Sprites\Refresh.png", typeof(Texture2D)) as Texture2D;
            _deleteIcon = AssetDatabase.LoadAssetAtPath(@"Assets\FlipWebApps\PrefsEditor\Sprites\Delete.png", typeof(Texture2D)) as Texture2D;
            _lockIcon = AssetDatabase.LoadAssetAtPath(@"Assets\FlipWebApps\PrefsEditor\Sprites\Lock.png", typeof(Texture2D)) as Texture2D;
            _redTexture = MakeColoredTexture(1, 1, new Color(1.0f, 0.0f, 0.0f, 0.1f));
            _playerPrefsFilter = "";
            _editorPrefsFilter = "";
            RefreshPlayerPrefs();
        }


        /// <summary>
        /// Draw the GUI
        /// </summary>
        void OnGUI()
        {
            // Main tabs and display
            _currentTab = GUILayout.Toolbar(_currentTab, _tabs);
            switch (_currentTab)
            {
                case 0:
                    DrawToolbar();
                    if (_showNew) DrawNew();
                    GUILayout.Space(5);
                    DrawPrefs(_playerPrefsEntries, true);
                    break;
                case 1:
                    DrawToolbarEditor();
                    if (_showNewEditor) DrawNewEditor();
                    GUILayout.Space(5);
                    DrawPrefs(_editorPrefsEntries, false);
                    break;
            }

        }


        /// <summary>
        /// Draws the toolbar.
        /// </summary>
        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (ButtonTrimmed("New...", _newIcon, EditorStyles.toolbarButton, "Add a new item"))
            {
                _newItemKey = "";
                _newItemValueInt = 0;
                _newItemValueFloat = 0;
                _newItemValueString = "";
                _newItemEncrypted = !(string.IsNullOrEmpty(_passPhrase));
                _showNew = true;
                ClearFocus();
            }

            if (ButtonTrimmed("Save All", _saveIcon, EditorStyles.toolbarButton, "Save modified entries"))
            {
                Save();
                RefreshPlayerPrefs();
            }

            if (ButtonTrimmed("Delete All...", null, EditorStyles.toolbarButton, "Delete all prefs entries"))
            {
                if (EditorUtility.DisplayDialog("Delete All Player Prefs",
                    "Are you sure you want to delete all Player Prefs?", "Yes", "No"))
                    DeleteAll();
            }

            GUILayout.Label(new GUIContent("Pass Phrase: ", "A pass phrase that should be used for encrypting / decrypting values."));
            _passPhrase = GUILayout.TextField(_passPhrase, EditorStyles.toolbarTextField, GUILayout.Width(150));
            if (_passPhrase != null)
            {
                SecurePlayerPrefs.PassPhrase = _passPhrase;
            }

            GUILayout.Space(20);
            GUILayout.FlexibleSpace();

            // filter
            EditorGUILayout.BeginHorizontal();
            GUI.changed = false;
            EditorGUI.BeginChangeCheck();
            _playerPrefsFilter = EditorGUILayout.TextField(_playerPrefsFilter, GuiStyles.ToolbarSearchField, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var entry in _playerPrefsEntries)
                    entry.MatchesFilter = entry.Key.IndexOf(_playerPrefsFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            if (GUILayout.Button("", string.IsNullOrEmpty(_playerPrefsFilter) ? GuiStyles.ToolbarSearchFieldCancelEmpty : GuiStyles.ToolbarSearchFieldCancel, GUILayout.ExpandWidth(false)))
            {
                _playerPrefsFilter = "";
                GUIUtility.keyboardControl = 0;
                foreach (var entry in _playerPrefsEntries)
                    entry.MatchesFilter = true;
            }
            EditorGUILayout.EndHorizontal();

            if (ButtonTrimmed("Refresh", _refreshIcon, EditorStyles.toolbarButton, "Reload prefs to reflect any changes"))
                RefreshPlayerPrefs();

            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Draws the toolbar.
        /// </summary>
        void DrawToolbarEditor()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (ButtonTrimmed("New...", _newIcon, EditorStyles.toolbarButton, "Add a new item"))
            {
                _newItemKeyEditor = "";
                _newItemValueIntEditor = 0;
                _newItemValueFloatEditor = 0;
                _newItemValueStringEditor = "";
                _showNewEditor = true;
                ClearFocus();
            }

            if (ButtonTrimmed("Save All", _saveIcon, EditorStyles.toolbarButton, "Save modified entries"))
            {
                Save();
                RefreshPlayerPrefs();
            }

            GUILayout.Space(20);
            GUILayout.FlexibleSpace();

            // filter
            EditorGUILayout.BeginHorizontal();
            GUI.changed = false;
            EditorGUI.BeginChangeCheck();
            _editorPrefsFilter = EditorGUILayout.TextField(_editorPrefsFilter, GuiStyles.ToolbarSearchField, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var entry in _editorPrefsEntries)
                    entry.MatchesFilter = entry.Key.IndexOf(_editorPrefsFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            if (GUILayout.Button("", string.IsNullOrEmpty(_editorPrefsFilter) ? GuiStyles.ToolbarSearchFieldCancelEmpty : GuiStyles.ToolbarSearchFieldCancel, GUILayout.ExpandWidth(false)))
            {
                _editorPrefsFilter = "";
                GUIUtility.keyboardControl = 0;
                foreach (var entry in _editorPrefsEntries)
                    entry.MatchesFilter = true;
            }
            EditorGUILayout.EndHorizontal();

            if (ButtonTrimmed("Refresh", _refreshIcon, EditorStyles.toolbarButton, "Reload prefs to reflect any changes"))
                RefreshPlayerPrefs();

            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Draws the new item.
        /// </summary>
        void DrawNew()
        {
            EditorGUILayout.BeginVertical("box");

            _newItemType = (ItemType)EditorGUILayout.EnumPopup(new GUIContent("Type", "The type of value this prefs item will contain"), _newItemType);
            _newItemKey = EditorGUILayout.TextField(new GUIContent("Key", "Aunique key for this prefs item"), _newItemKey);
            switch (_newItemType)
            {
                case ItemType.Int:
                    _newItemValueInt = EditorGUILayout.IntField(new GUIContent("Value", "This items value"), _newItemValueInt);
                    break;
                case ItemType.Float:
                    _newItemValueFloat = EditorGUILayout.FloatField(new GUIContent("Value", "This items value"), _newItemValueFloat);
                    break;
                case ItemType.String:
                    _newItemValueString = EditorGUILayout.TextField(new GUIContent("Value", "This items value"), _newItemValueString);
                    break;
            }
            _newItemEncrypted = EditorGUILayout.Toggle(new GUIContent("Encrypted", "Specifies whether this item should be encrypted."), _newItemEncrypted);
            if (_newItemEncrypted && string.IsNullOrEmpty(_passPhrase))
                EditorGUILayout.HelpBox("Please ensure you specify a pass phrase before adding encrypted items.", MessageType.Warning);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (ButtonTrimmed("Add", null, EditorStyles.miniButtonRight, "Create a new prefs item with the values entered above."))
            {
                if (!string.IsNullOrEmpty(_newItemKey))
                {
                    PrefsEntry newPrefsEntry = null;
                    switch (_newItemType)
                    {
                        case ItemType.Int:
                            newPrefsEntry = new PrefsEntry(_newItemKey, _newItemValueInt, _newItemEncrypted, true);
                            break;
                        case ItemType.Float:
                            newPrefsEntry = new PrefsEntry(_newItemKey, _newItemValueFloat, _newItemEncrypted, true);
                            break;
                        case ItemType.String:
                            newPrefsEntry = new PrefsEntry(_newItemKey, _newItemValueString, _newItemEncrypted, true);
                            break;
                    }
                    newPrefsEntry.MatchesFilter = _newItemKey.IndexOf(_playerPrefsFilter + "", StringComparison.OrdinalIgnoreCase) >= 0;
                    newPrefsEntry.Save();
                    _playerPrefsEntries.Add(newPrefsEntry);
                }
                ClearFocus();
                _showNew = false;
            }

            if (ButtonTrimmed("Cancel", null, EditorStyles.miniButtonRight, "Close this popup without adding a prefs item"))
            {
                _showNew = false;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the new item.
        /// </summary>
        void DrawNewEditor()
        {
            EditorGUILayout.BeginVertical("box");

            _newItemTypeEditor = (ItemType)EditorGUILayout.EnumPopup(new GUIContent("Type", "The type of value this prefs item will contain"), _newItemTypeEditor);
            _newItemKeyEditor = EditorGUILayout.TextField(new GUIContent("Key", "Aunique key for this prefs item"), _newItemKeyEditor);
            switch (_newItemTypeEditor)
            {
                case ItemType.Int:
                    _newItemValueIntEditor = EditorGUILayout.IntField(new GUIContent("Value", "This items value"), _newItemValueIntEditor);
                    break;
                case ItemType.Float:
                    _newItemValueFloatEditor = EditorGUILayout.FloatField(new GUIContent("Value", "This items value"), _newItemValueFloatEditor);
                    break;
                case ItemType.String:
                    _newItemValueStringEditor = EditorGUILayout.TextField(new GUIContent("Value", "This items value"), _newItemValueStringEditor);
                    break;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (ButtonTrimmed("Add", null, EditorStyles.miniButtonRight, "Create a new prefs item with the values entered above."))
            {
                if (!string.IsNullOrEmpty(_newItemKeyEditor))
                {
                    PrefsEntry newPrefsEntry = null;
                    switch (_newItemTypeEditor)
                    {
                        case ItemType.Int:
                            newPrefsEntry = new PrefsEntry(_newItemKeyEditor, _newItemValueIntEditor, false, false);
                            break;
                        case ItemType.Float:
                            newPrefsEntry = new PrefsEntry(_newItemKeyEditor, _newItemValueFloatEditor, false, false);
                            break;
                        case ItemType.String:
                            newPrefsEntry = new PrefsEntry(_newItemKeyEditor, _newItemValueStringEditor, false, false);
                            break;
                    }
                    newPrefsEntry.MatchesFilter = _newItemKey.IndexOf(_editorPrefsFilter + "", StringComparison.OrdinalIgnoreCase) >= 0;
                    newPrefsEntry.Save();
                    _editorPrefsEntries.Add(newPrefsEntry);
                }
                ClearFocus();
                _showNewEditor = false;
            }

            if (ButtonTrimmed("Cancel", null, EditorStyles.miniButtonRight, "Close this popup without adding a prefs item"))
            {
                _showNewEditor = false;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draw the player prefs entries
        /// </summary>
        private void DrawPrefs(List<PrefsEntry> _prefsEntries, bool isPlayerPrefs)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var boldGUIStyle = new GUIStyle(EditorStyles.numberField);
            boldGUIStyle.fontStyle = FontStyle.Bold;

            for (var i = 0; i < _prefsEntries.Count; i++)
            {
                var prefsEntry = _prefsEntries[i];
                if (prefsEntry.MatchesFilter)
                {
                    var s = new GUIStyle();
                    if (prefsEntry.HasError)
                        s.normal.background = _redTexture;

                    EditorGUILayout.BeginHorizontal(s);

                    // type
                    string type;
                    string typeTooltip;
                    switch (prefsEntry.Type)
                    {
                        case SecurePlayerPrefs.ItemType.Int:
                            //case SecurePlayerPrefs.ItemType.Bool:
                            type = "I";
                            typeTooltip = "Int Type";
                            break;
                        case SecurePlayerPrefs.ItemType.Float:
                            type = "F";
                            typeTooltip = "Float Type";
                            break;
                        case SecurePlayerPrefs.ItemType.String:
                            //case SecurePlayerPrefs.ItemType.Vector2:
                            //case SecurePlayerPrefs.ItemType.Vector3:
                            type = "S";
                            typeTooltip = "String Type";
                            break;
                        default:
                            type = "?";
                            typeTooltip = "Unknown";
                            break;
                    }
                    GUILayout.Label(new GUIContent(type, typeTooltip), GUILayout.Width(20));

                    if (isPlayerPrefs)
                    {
                        if (prefsEntry.IsEncrypted)
                            GUILayout.Label(new GUIContent(null, _lockIcon, "Encrypted Values\n\nKey: " + prefsEntry.OriginalEncryptedKey + "\nValue: " + prefsEntry.OriginalEncryptedValue), GUILayout.Width(20));
                        else
                            GUILayout.Label(new GUIContent("-", "Not encrypted"), GUILayout.Width(20));
                    }

                    // key
                    prefsEntry.Key = EditorGUILayout.TextField(prefsEntry.Key, prefsEntry.IsModified ? boldGUIStyle : EditorStyles.textField, GUILayout.MinWidth(80), GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));

                    // value
                    switch (prefsEntry.Type)
                    {
                        case SecurePlayerPrefs.ItemType.Int:
                            //case SecurePlayerPrefs.ItemType.Bool:
                            prefsEntry.ValueInt = EditorGUILayout.IntField(prefsEntry.ValueInt, prefsEntry.IsModified ? boldGUIStyle : EditorStyles.textField, GUILayout.MinWidth(80));
                            break;
                        case SecurePlayerPrefs.ItemType.Float:
                            prefsEntry.ValueFloat = EditorGUILayout.FloatField(prefsEntry.ValueFloat, prefsEntry.IsModified ? boldGUIStyle : EditorStyles.textField, GUILayout.MinWidth(80));
                            break;
                        case SecurePlayerPrefs.ItemType.String:
                            //case SecurePlayerPrefs.ItemType.Vector2:
                            //case SecurePlayerPrefs.ItemType.Vector3:
                            prefsEntry.ValueString = EditorGUILayout.TextField(prefsEntry.ValueString, prefsEntry.IsModified ? boldGUIStyle : EditorStyles.textField, GUILayout.MinWidth(80));
                            break;
                    }

                    // save button
                    if (ButtonTrimmed("", _saveIcon, GUI.skin.button, "Save this entry"))
                    {
                        prefsEntry.Save();
                    }

                    // delete button
                    if (ButtonTrimmed("", _deleteIcon, GUI.skin.button, "Delete this entry"))
                    {
                        prefsEntry.Delete();
                        _prefsEntries.Remove(prefsEntry);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }


        #region ToolbarOptions

        /// <summary>
        /// save all changes
        /// </summary>
        void Save()
        {
            foreach (var playerPrefsEntry in _playerPrefsEntries)
            {
                playerPrefsEntry.Save();
            }
        }


        /// <summary>
        /// Delete all entries
        /// </summary>
        void DeleteAll()
        {
            SecurePlayerPrefs.DeleteAll();
            RefreshPlayerPrefs();
        }

        #endregion ToolbarOptions


        #region Load Preferences
        /// <summary>
        /// Load the preferences based upon the current platform
        /// </summary>
        void RefreshPlayerPrefs()
        {
            _playerPrefsEntries.Clear();
            _editorPrefsEntries.Clear();

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                RefreshPlayerPrefsWindows();
                RefreshEditorPrefsWindows();
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                RefreshPlayerPrefsOSX();
                RefreshEditorPrefsOSX();
            }
            else
            {
                Debug.Log("This currently only works on Windows and Mac.");
            }

            _playerPrefsEntries.Sort((p1, p2) => p1.Key.CompareTo(p2.Key));

        }


        /// <summary>
        /// On Windows, PlayerPrefs are stored in the registry under HKCU\Software\[company name]\[product name] key, where 
        /// company and product names are the names set up in Project Settings. (http://docs.unity3d.com/ScriptReference/PlayerPrefs.html)
        /// EditorPrefs are stored in the registry under the HKCU\Software\Unity Technologies\UnityEditor N.x key where N.x is the major version number.
        /// </summary>
        void RefreshPlayerPrefsWindows()
        {
            var registryPath = "Software\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName;
#if UNITY_5_5_OR_NEWER
            registryPath = "Software\\Unity\\UnityEditor\\" + PlayerSettings.companyName + "\\" + PlayerSettings.productName;
#endif
            var prefsKeyStore = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryPath);
            var prefsKeyNames = prefsKeyStore.GetValueNames();
            prefsKeyNames.ToList().Sort();
            foreach (var prefsKey in prefsKeyNames)
            {
                var keyName = prefsKey.Substring(0, prefsKey.LastIndexOf("_", StringComparison.Ordinal));
                if (!_systemEntries.Contains(keyName))
                    _playerPrefsEntries.Add(new PrefsEntry(keyName, true)
                    {
                        MatchesFilter = keyName.IndexOf(_playerPrefsFilter + "", StringComparison.OrdinalIgnoreCase) >= 0
                    });

            }
        }


        /// <summary>
        /// On Windows, EditorPrefs are stored in the registry under the HKCU\Software\Unity Technologies\Unity Editor N.x key where N.x is the major version number.
        /// </summary>
        void RefreshEditorPrefsWindows()
        {
            var registryPath = "Software\\Unity Technologies\\Unity Editor " + Application.unityVersion.Substring(0, Application.unityVersion.IndexOf(".", StringComparison.Ordinal)) + ".x";
            var prefsKeyStore = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryPath);
            var prefsKeyNames = prefsKeyStore.GetValueNames();
            prefsKeyNames.ToList().Sort();
            foreach (var prefsKey in prefsKeyNames)
            {
                var keyName = prefsKey.Substring(0, prefsKey.LastIndexOf("_", StringComparison.Ordinal));
                _editorPrefsEntries.Add(new PrefsEntry(keyName, false)
                {
                    MatchesFilter = keyName.IndexOf(_editorPrefsFilter + "", StringComparison.OrdinalIgnoreCase) >= 0
                });
            }
        }


        /// <summary>
        /// On Mac OS X PlayerPrefs are stored in ~/Library/Preferences folder, in a file named unity.[company name].[product name].plist, 
        /// where company and product names are the names set up in Project Settings. The same .plist file is used for both Projects run 
        /// in the Editor and standalone players. (http://docs.unity3d.com/ScriptReference/PlayerPrefs.html)
        /// </summary>
        void RefreshPlayerPrefsOSX()
        {
            var prefsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";

            if (File.Exists(prefsPath))
            {
                var prefsPlist = (Dictionary<string, object>)Plist.readPlist(prefsPath);
                foreach (var prefsKey in prefsPlist.Keys)
                {
                    var keyName = prefsKey;
                    if (!_systemEntries.Contains(keyName))
                        _playerPrefsEntries.Add(new PrefsEntry(keyName, true)
                        {
                            MatchesFilter = keyName.IndexOf(_playerPrefsFilter + "", StringComparison.OrdinalIgnoreCase) >= 0
                        });
                }
            }
            else
            {
                Debug.Log("OSX Prefs file not found '" + prefsPath + "'");
            }
        }


        /// <summary>
        /// On Mac OS X PlayerPrefs are stored in ~/Library/Preferences folder, in a file named com.unity3d.UnityEditor.plist 
        /// </summary>
        void RefreshEditorPrefsOSX()
        {
            var prefsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/com.unity3d.UnityEditor.plist";
            if (File.Exists(prefsPath))
            {
                var prefsPlist = (Dictionary<string, object>)Plist.readPlist(prefsPath);
                foreach (var prefsKey in prefsPlist.Keys)
                {
                    var keyName = prefsKey;
                    _editorPrefsEntries.Add(new PrefsEntry(keyName, false)
                    {
                        MatchesFilter = keyName.IndexOf(_editorPrefsFilter + "", StringComparison.OrdinalIgnoreCase) >= 0
                    });
                }
            }
            else
            {
                Debug.Log("OSX Prefs file not found '" + prefsPath + "'");
            }
        }
        #endregion Load Preferences


        #region Editor Helper Functions

        /// <summary>
        /// Show a button trimmed to the length of the text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static bool ButtonTrimmed(string text, GUIStyle style)
        {
            return GUILayout.Button(text, style, GUILayout.MaxWidth(style.CalcSize(new GUIContent(text)).x));
        }


        /// <summary>
        /// Show a button trimmed to the length of the text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="texture"></param>
        /// <param name="style"></param>
        /// <param name="tooltip"></param>
        /// <returns></returns>
        public static bool ButtonTrimmed(string text, Texture2D texture, GUIStyle style, string tooltip = null)
        {
            if (texture != null)
                return GUILayout.Button(new GUIContent(text, texture, tooltip), style, GUILayout.MaxWidth(style.CalcSize(new GUIContent(text)).x + texture.width));
            else
                return ButtonTrimmed(text, style);
        }


        /// <summary>
        /// Make a texture of the given size and color
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private Texture2D MakeColoredTexture(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        /// <summary>
        /// Clear focus from the current item
        /// </summary>
        public void ClearFocus()
        {
            GUIUtility.keyboardControl = 0;
        }

        #endregion Editor Helper Functions

    }


    [Serializable]
    public class PrefsEntry
    {
        public SecurePlayerPrefs.ItemType Type;
        public bool IsPlayerPrefs;  // if not player then editor
        public bool MatchesFilter = true;

        public string OriginalEncryptedKey;
        public string OriginalEncryptedValue;

        public string OriginalKey { get; private set; }
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                HasError = string.IsNullOrEmpty(value);

                if (value != _key)
                {
                    _key = value;
                    if (!_isKeySet)
                    {
                        OriginalKey = Key;
                        _isKeySet = true;
                    }
                    else if (Key != OriginalKey)
                        IsModified = true;
                }
            }
        }
        string _key;
        bool _isKeySet;

        public int ValueInt
        {
            get
            {
                return _valueInt;
            }
            set
            {
                if (value != _valueInt)
                {
                    _valueInt = value;
                    if (_isValueIntSet)
                        IsModified = true;
                    _isValueIntSet = true;
                }
            }
        }
        int _valueInt;
        bool _isValueIntSet;

        public float ValueFloat
        {
            get
            {
                return _valueFloat;
            }
            set
            {
                if (!Mathf.Approximately(value, _valueFloat))
                {
                    _valueFloat = value;
                    if (_isValueFloatSet)
                        IsModified = true;
                    _isValueFloatSet = true;
                }
            }
        }
        float _valueFloat;
        bool _isValueFloatSet;

        public string ValueString {
            get
            {
                return _valueString;
            }
            set
            {
                if (value != _valueString)
                {
                    _valueString = value;
                    if (_isValueStringSet)
                        IsModified = true;
                    _isValueStringSet = true;
                }
            }
        }
        string _valueString;
        bool _isValueStringSet;

        public bool IsModified;
        public bool IsEncrypted;
        public bool HasError;

        public PrefsEntry(string key, bool isPlayerPrefs)
        {
            IsPlayerPrefs = isPlayerPrefs;
            LoadValue(key);
        }

        public PrefsEntry(string key, int value, bool isEncrypted, bool isPlayerPrefs)
        {
            Key = key;
            ValueInt = value;
            IsPlayerPrefs = isPlayerPrefs;
            Type = SecurePlayerPrefs.ItemType.Int;
            SetupSpecifiedEntry(key, isEncrypted);
        }

        public PrefsEntry(string key, float value, bool isEncrypted, bool isPlayerPrefs)
        {
            Key = key;
            ValueFloat = value;
            IsPlayerPrefs = isPlayerPrefs;
            Type = SecurePlayerPrefs.ItemType.Float;
            SetupSpecifiedEntry(key, isEncrypted);
        }

        public PrefsEntry(string key, string value, bool isEncrypted, bool isPlayerPrefs)
        {
            Key = key;
            ValueString = value ?? "";
            IsPlayerPrefs = isPlayerPrefs;
            Type = SecurePlayerPrefs.ItemType.String;
            SetupSpecifiedEntry(key, isEncrypted);
        }

        void SetupSpecifiedEntry(string key, bool isEncrypted)
        {
            IsEncrypted = isEncrypted;
            if (IsEncrypted)
                OriginalEncryptedKey = SecurePlayerPrefs.EncryptKey(key);
        }

        /// <summary>
        /// Load setup key. The passed key will always be the version read from disk so may be encrypted
        /// As we can't tell the type, we use an unlikely value to determine whether it exists or not!
        /// NOTE: (should we do a double check on 2 default values to be 100% sure)?
        /// </summary>
        void LoadValue(string key)
        {
            SecurePlayerPrefs.ItemType itemType = SecurePlayerPrefs.ItemType.None;
            var stringValue = IsPlayerPrefs ? PlayerPrefs.GetString(key, SecurePlayerPrefs.NotFoundString) : EditorPrefs.GetString(key, SecurePlayerPrefs.NotFoundString);

            // if player prefs then try and handle encrypted prefs.
            if (IsPlayerPrefs)
            {
                if (stringValue != SecurePlayerPrefs.NotFoundString)
                    itemType = SecurePlayerPrefs.GetItemType(stringValue);

                if (itemType != SecurePlayerPrefs.ItemType.None)
                {
                    // check we can decrypt key - otherwise probably not encrypted
                    var decryptedKey = SecurePlayerPrefs.DecryptKey(key);
                    if (decryptedKey != null)
                    {

                        if (!ValidateEncryptedValue(itemType, stringValue)) return;
                        IsEncrypted = true;
                        OriginalEncryptedKey = key;
                        OriginalEncryptedValue = stringValue;
                        Key = decryptedKey;
                        Type = itemType;

                        switch (itemType)
                        {
                            case SecurePlayerPrefs.ItemType.Int:
                                //case SecurePlayerPrefs.ItemType.Bool:
                                ValueInt = SecurePlayerPrefs.GetInt(decryptedKey, 0, true);
                                break;
                            case SecurePlayerPrefs.ItemType.Float:
                                ValueFloat = SecurePlayerPrefs.GetFloat(decryptedKey, 0, true);
                                break;
                            case SecurePlayerPrefs.ItemType.String:
                                //case SecurePlayerPrefs.ItemType.Vector2:
                                //case SecurePlayerPrefs.ItemType.Vector3:
                                ValueString = SecurePlayerPrefs.GetString(decryptedKey, "", true);
                                return;
                        }
                    }
                    else
                    {
                        itemType = SecurePlayerPrefs.ItemType.None;
                    }
                }
            }

            // If we still haven't gotten anything.
            if (itemType == SecurePlayerPrefs.ItemType.None)
            {
                // if it wasn't an encrypted item 
                Key = key;
                if (stringValue != SecurePlayerPrefs.NotFoundString)
                {
                    Type = SecurePlayerPrefs.ItemType.String;
                    ValueString = stringValue;
                }
                else
                {
                    var intValue = IsPlayerPrefs ? PlayerPrefs.GetInt(Key, int.MinValue + 10) : EditorPrefs.GetInt(Key, int.MinValue + 10);
                    if (intValue != int.MinValue + 10)
                    {
                        Type = SecurePlayerPrefs.ItemType.Int;
                        ValueInt = intValue;
                    }
                    else
                    {
                        var floatValue = IsPlayerPrefs ? PlayerPrefs.GetFloat(Key, float.MinValue + 10) : EditorPrefs.GetFloat(Key, float.MinValue + 10);
                        if (!Mathf.Approximately(floatValue, float.MinValue + 10))
                        {
                            Type = SecurePlayerPrefs.ItemType.Float;
                            ValueFloat = floatValue;
                        }
                    }
                }
            }
        }

        bool ValidateEncryptedValue(SecurePlayerPrefs.ItemType itemType, string encryptedValue)
        {
            if (SecurePlayerPrefs.DecryptValue(encryptedValue, itemType) == null)
            {
                Debug.LogWarning(
                    string.Format(
                        "Unable to decrypt or parse '{0}' ({1}). Please check you have the correct pass phrase set and refresh",
                        _key, encryptedValue));
                Type = SecurePlayerPrefs.ItemType.String;
                ValueString = encryptedValue;
                IsEncrypted = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Save the prefs. 
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            if (IsPlayerPrefs)
            {
                if (IsEncrypted && !SecurePlayerPrefs.IsPassPhraseSet) Debug.LogWarning("Please set the pass phrase that should be used. It is a security risk using the default value.");

                if (Type == SecurePlayerPrefs.ItemType.Int)
                {
                    OriginalEncryptedValue = SecurePlayerPrefs.EncryptValue(BitConverter.GetBytes(ValueInt), SecurePlayerPrefs.ItemType.Int);
                    SecurePlayerPrefs.SetInt(Key, ValueInt, IsEncrypted);
                }
                else if (Type == SecurePlayerPrefs.ItemType.Float)
                {
                    OriginalEncryptedValue = SecurePlayerPrefs.EncryptValue(BitConverter.GetBytes(ValueFloat), SecurePlayerPrefs.ItemType.Float);
                    SecurePlayerPrefs.SetFloat(Key, ValueFloat, IsEncrypted);
                }
                else
                {
                    OriginalEncryptedValue = SecurePlayerPrefs.EncryptValue(System.Text.Encoding.UTF8.GetBytes(ValueString), SecurePlayerPrefs.ItemType.String);
                    SecurePlayerPrefs.SetString(Key, ValueString, IsEncrypted);
                }

                // if key is changed then also delete the old key.
                if (OriginalKey != Key)
                    SecurePlayerPrefs.DeleteKey(OriginalKey, IsEncrypted);

                SecurePlayerPrefs.Save();
            }
            else
            {
                if (Type == SecurePlayerPrefs.ItemType.Int)
                {
                    EditorPrefs.SetInt(Key, ValueInt);
                }
                else if (Type == SecurePlayerPrefs.ItemType.Float)
                {
                    EditorPrefs.SetFloat(Key, ValueFloat);
                }
                else
                {
                    EditorPrefs.SetString(Key, ValueString);
                }
            }

            OriginalKey = Key;
            IsModified = false;
        }


        /// <summary>
        /// Delete the prefs. Take care how this PlayerPrefsEntry is used after deletion.
        /// </summary>
        public void Delete()
        {
            if (IsPlayerPrefs)
            {
                SecurePlayerPrefs.DeleteKey(OriginalKey, IsEncrypted);
                SecurePlayerPrefs.Save();
            }
            else
            {
                EditorPrefs.DeleteKey(OriginalKey);
            }
        }
    }

    public static class GuiStyles
    {
        public static readonly GUIStyle ToolbarSearchField = "ToolbarSeachTextField";
        public static readonly GUIStyle ToolbarSearchFieldCancel = "ToolbarSeachCancelButton";
        public static readonly GUIStyle ToolbarSearchFieldCancelEmpty = "ToolbarSeachCancelButtonEmpty";
    }
    }
