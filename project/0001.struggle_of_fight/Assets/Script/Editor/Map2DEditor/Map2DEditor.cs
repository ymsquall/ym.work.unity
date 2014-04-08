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
            var mainForm = GetEditorForm();
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

        static public void OnDrawSceneFunc(SceneView sceneView)
        {
            var mainForm = GetEditorForm();
            mainForm.DrawSceneGUI(sceneView);
        }

        public static Map2DEditorForm GetEditorForm()
        {
            return EditorWindowsManager.GetWindow<Map2DEditorForm>(MainFormName);
        }
    }
}

#endif