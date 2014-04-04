#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DEditor : ScriptableObject
    {
        const string MainFormName = "Map2DMainForm";
        [MenuItem("Editor/横版地图编辑器")]
        static void DoIt()
        {
            Map2DEditorForm.CreateForm(MainFormName, "横版地图编辑器");
        }

        public static Map2DEditorForm GetEditorForm()
        {
            return EditorWindowsManager.GetWindow<Map2DEditorForm>(MainFormName);
        }
    }
}

#endif