#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DEditorMenuItem : ScriptableObject
    {
        [MenuItem("Editor/地图编辑器")]
        static void DoIt()
        {
            EditorWindow wnd = new EditorWindow();
            wnd.Show();
            EditorUtility.DisplayDialog("Map2DEditor", "Do It in C# !", "OK", "");
        }
    }
}

#endif