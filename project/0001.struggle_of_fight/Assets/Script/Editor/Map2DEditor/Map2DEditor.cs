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

        [MenuItem("Editor/横版地图编辑器")]
        static void DoIt()
        {
            // 扫描文件建立文件名<=>InstanceID关联
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(Application.dataPath);
            //string[] picFiles = FileUtils.EnumAllFilesByPath(Application.dataPath, true, ".png", ".jpg", ".dds");
            //foreach (string fn in picFiles)
            //{
            //    Map2DEditor.AssetsFile2InstanceDicts.Add(fn, -1);
            //    AssetDatabase.GetAssetPath
            //    ScanAssets(fn);
            //}
            //while (Map2DEditor.AssetsFile2InstanceDicts.Count != Map2DEditor.AssetsInstance2FileDicts.Count)
            //    System.Threading.Thread.Sleep(1000);
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