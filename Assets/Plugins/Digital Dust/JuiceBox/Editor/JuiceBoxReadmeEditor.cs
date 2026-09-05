using System.IO;
using UnityEditor;
using UnityEngine;

// ==============================================================================
//  JuiceBoxReadmeEditor: custom inspector that renders a JuiceBoxReadme asset as a
//  formatted welcome page, and highlights the asset in the Project view on import.
// ==============================================================================
namespace JuiceBox
{
    [CustomEditor(typeof(JuiceBoxReadme))]
    [InitializeOnLoad]
    public class JuiceBoxReadmeEditor : Editor
    {
        private const float SectionSpacing = 16f;
        private const float MaxIconWidth = 128f;

        private bool _initialized;
        private bool _hasDemoMaterials;
        private string _fixMessage;
        private bool _fixOk;
        private GUIStyle _titleStyle;
        private GUIStyle _headingStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _linkStyle;

        static JuiceBoxReadmeEditor()
        {
            EditorApplication.update += HighlightOnImport;
        }

        // update rather than delayCall, unsubscribed on the tick that does the
        // work: delayCall fires inside the editor's inspector GUI phase, so
        // assigning Selection from it rebuilds a live inspector mid-pass and Unity
        // logs "GUI Window tried to begin rendering while something else had not
        // finished rendering!". Waiting out isUpdating/isCompiling also lets a
        // fresh package import settle, which is when this runs on a new install.
        static void HighlightOnImport()
        {
            if (EditorApplication.isUpdating || EditorApplication.isCompiling) return;
            EditorApplication.update -= HighlightOnImport;

            string[] ids = AssetDatabase.FindAssets("t:JuiceBoxReadme");
            if (ids.Length == 0) return;

            for (int i = 0; i < ids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(ids[i]);
                JuiceBoxReadme readme = AssetDatabase.LoadAssetAtPath<JuiceBoxReadme>(path);
                if (readme == null || !readme.highlightOnImport) continue;

                readme.highlightOnImport = false;
                EditorUtility.SetDirty(readme);
                AssetDatabase.SaveAssetIfDirty(readme);
                Selection.activeObject = readme;
                return;
            }
        }

        public override void OnInspectorGUI()
        {
            JuiceBoxReadme readme = (JuiceBoxReadme)target;
            Init();

            GUILayout.BeginHorizontal();
            if (readme.icon != null)
            {
                float iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, MaxIconWidth);
                GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
            }
            GUILayout.Label(readme.title, _titleStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(SectionSpacing);

            for (int i = 0; readme.sections != null && i < readme.sections.Length; i++)
            {
                JuiceBoxReadme.Section section = readme.sections[i];

                if (!string.IsNullOrEmpty(section.heading))
                    GUILayout.Label(section.heading, _headingStyle);

                if (!string.IsNullOrEmpty(section.text))
                    GUILayout.Label(section.text, _bodyStyle);

                if (!string.IsNullOrEmpty(section.linkText))
                {
                    if (LinkLabel(new GUIContent(section.linkText)))
                        OpenLink(readme, section.url);
                }

                GUILayout.Space(SectionSpacing);
            }

            DrawDemoMaterialFix();
        }

        private void DrawDemoMaterialFix()
        {
            if (!_hasDemoMaterials) return;

            if (GUILayout.Button(DemoMaterialPipelineFixer.ButtonLabel(),
                GUILayout.Height(26f)))
                _fixOk = DemoMaterialPipelineFixer.Fix(out _fixMessage);

            if (!string.IsNullOrEmpty(_fixMessage))
                EditorGUILayout.HelpBox(_fixMessage,
                    _fixOk ? MessageType.Info : MessageType.Warning);
        }

        private static void OpenLink(JuiceBoxReadme readme, string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                Application.OpenURL(url);
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(readme);
            string assetDir = Path.GetDirectoryName(assetPath);
            string fullPath = Path.Combine(assetDir, url);
            fullPath = Path.GetFullPath(fullPath);

            if (File.Exists(fullPath))
                Application.OpenURL("file:///" + fullPath.Replace('\\', '/'));
            else
                Debug.LogWarning("JuiceBox: Could not find file at " + fullPath);
        }

        private void Init()
        {
            if (_initialized) return;

            _bodyStyle = new GUIStyle(EditorStyles.label);
            _bodyStyle.wordWrap = true;
            _bodyStyle.fontSize = 14;
            _bodyStyle.richText = true;

            _titleStyle = new GUIStyle(_bodyStyle);
            _titleStyle.fontSize = 26;

            _headingStyle = new GUIStyle(_bodyStyle);
            _headingStyle.fontSize = 18;
            _headingStyle.fontStyle = FontStyle.Bold;

            _linkStyle = new GUIStyle(_bodyStyle);
            _linkStyle.wordWrap = false;
            _linkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            _linkStyle.stretchWidth = false;

            _hasDemoMaterials = DemoMaterialPipelineFixer.HasDemoMaterials();

            _initialized = true;
        }

        private bool LinkLabel(GUIContent label)
        {
            Rect position = GUILayoutUtility.GetRect(label, _linkStyle);

            Handles.BeginGUI();
            Handles.color = _linkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, label, _linkStyle);
        }
    }
}
