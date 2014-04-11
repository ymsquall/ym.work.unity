#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DEditor : ScriptableObject
    {
        public const string MainFormName = "Map2DMainForm";
        [MenuItem("Editor/横版地图编辑器")]
        static void DoIt()
        {
            var mainForm = EditorWindowsManager.GetWindow<Map2DEditorForm>(MainFormName);
            if (UnityEditor.EditorApplication.currentScene == null || UnityEditor.EditorApplication.currentScene == "")
            {
                EditorUtility.DisplayDialog("错误", "必须打开已有场景后才能进行地图编辑！", "关闭", "");
                if (null != mainForm)
                {
                    EditorWindowsManager.ClosedWindow(MainFormName);
                    mainForm.Close();
                }
                return;
            }
            if (null == mainForm)
            {
                mainForm = Map2DEditorForm.CreateForm(MainFormName, "横版地图编辑器");
                mainForm.mSceneDelegate = new SceneView.OnSceneFunc(OnDrawSceneFunc);
                SceneView.onSceneGUIDelegate += mainForm.mSceneDelegate;
            }
        }

        public static void OnDrawSceneFunc(SceneView sceneView)
        {
            var mainForm = EditorWindowsManager.GetWindow<Map2DEditorForm>(MainFormName);
            mainForm.DrawSceneGUI(sceneView);
        }

        public static void OnFormClosed(string name)
        {
            EditorWindowsManager.ClosedWindow(name);
        }

        public static Map2DGridEditorForm GetOrNewGridEditorForm(int rIndex, int cIndex)
        {
            string name = string.Format("GridForm({0}-{1})", rIndex, cIndex);
            var form = EditorWindowsManager.GetWindow<Map2DGridEditorForm>(name);
            if (null != form)
                return form;
            string title = string.Format("单元{0}-{1}", rIndex, cIndex);
            form = Map2DGridEditorForm.CreateForm(name, title);
            form.GridRowIndex = rIndex;
            form.GridColIndex = cIndex;
            return form;
        }

        public static Map2DGridUnit FindGridUnitByIndex(int rIndex, int cIndex)
        {
            var mainForm = EditorWindowsManager.GetWindow<Map2DEditorForm>(MainFormName);
            if (null == mainForm)
                return null;
            return mainForm[rIndex, cIndex];
        }
        
        public static Color ColorByToolType(string type)
        {
            Color color = Color.white;
            if (Map2DGridEditorToolboxView.ToolsTips[0] == type)
                color = Color.green;
            else if (Map2DGridEditorToolboxView.ToolsTips[1] == type)
                color = Color.cyan;
            else if (Map2DGridEditorToolboxView.ToolsTips[2] == type)
                color = Color.red;
            return color;
        }
    }
}

#endif