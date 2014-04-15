#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Script.Tools;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DEditor : ScriptableObject
    {
        public static Dictionary<int, string> AssetsInstance2FileDicts = new Dictionary<int,string>(0);
        public static Dictionary<string, int> AssetsFile2InstanceDicts = new Dictionary<string, int>(0);
        public const string MainFormName = "Map2DMainForm";
        public const float DefaultPlaneU3DSize = 10.0f;
        public const float DefaultCubeU3DSize = 1.0f;
        public const float DefaultQuadU3DSize = 1.0f;
        public const int ColliderLayerID_Ground = 9;
        public const int ColliderLayerID_Wall = 11;
        public static float PixelUnit2U3DUnit(float p)
        {
            return p / 100.0f;
        }
        public static float U3DUnit2PixelUnit(float m)
        {
            return m * 100.0f;
        }

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
        public static void DoSaved()
        {
            var mainForm = EditorWindowsManager.GetWindow<Map2DEditorForm>(MainFormName);
            if(null != mainForm)
                mainForm.SaveMap2DData();
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

        public static Map2DGridEditorForm GetOrNewGridEditorForm(int rIndex, int cIndex, bool newForm = true)
        {
            string name = string.Format("GridForm({0}-{1})", rIndex, cIndex);
            var form = EditorWindowsManager.GetWindow<Map2DGridEditorForm>(name);
            if (null != form)
                return form;
            if (!newForm)
                return null;
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

        public static Color ColorByToolType(Map2DGridImageSubType type)
        {
            Color color = Color.white;
            switch(type)
            {
                case Map2DGridImageSubType.地面:
                    color = Color.green;
                    break;
                case Map2DGridImageSubType.墙壁:
                    color = Color.cyan;
                    break;
                case Map2DGridImageSubType.刷怪点:
                    color = Color.red;
                    break;
            }
            return color;
        }
    }
}

#endif