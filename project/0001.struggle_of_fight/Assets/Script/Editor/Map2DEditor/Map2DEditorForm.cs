#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DEditorForm : EditorBaseWindow<Map2DEditorForm>
    {
        public const ushort Map2DDataFileVersion = 1;
        //Vector2 mGridsSpcaeSize = new Vector2(5f, 2.5f);
        string mCongigFilePath = "";
        string mDataFilePath = "";
        internal SceneView.OnSceneFunc mSceneDelegate;
        float mMapViewScrollRate = 100.0f;
        ushort mUnitImageWidth = 16;
        ushort mUnitImageHeight = 16;
        Vector2 mScrollViewPos = Vector2.zero;
        bool mRowCountToggle = false;
        bool mColCountToggle = false;
        Map2DGridUnit[,] mMapUnitList = new Map2DGridUnit[1, 1];
        Dictionary<int, Map2DGridUnit> mMapUnitDictionary = new Dictionary<int, Map2DGridUnit>(1);

        public Map2DGridUnit this[int r, int c]
        {
            get
            {
                int rowCount = mMapUnitList.GetLength(0);
                int colCount = mMapUnitList.GetLength(1);
                if (r < 0 || r >= rowCount || c < 0 || c >= colCount)
                    return null;
                return mMapUnitList[r, c];
            }
        }
#region 文件读写
        bool SaveMap2DConfig()
        {
            FileStream newFile = File.OpenWrite(mCongigFilePath);
            byte[] bin = BitConverter.GetBytes(Map2DDataFileVersion);  // data version
            newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(mMapViewScrollRate); newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(mUnitImageWidth); newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(mUnitImageHeight); newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(mScrollViewPos.x); newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(mScrollViewPos.y); newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(mRowCountToggle); newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(mColCountToggle); newFile.Write(bin, 0, bin.Length);
            newFile.Close();
            return true;
        }
        bool ReadMap2DConfig()
        {
            if(!File.Exists(mCongigFilePath))
                SaveMap2DConfig();
            FileStream cfgFile = File.OpenRead(mCongigFilePath);
            byte[] buffer = new byte[cfgFile.Length];
            int length = cfgFile.Read(buffer, 0, Convert.ToInt32(cfgFile.Length));
            if(length != cfgFile.Length)
            {
                EditorUtility.DisplayDialog("错误", "地图编辑器配置文件错误！", "关闭", "");
                cfgFile.Close();
                return false;
            }
            int readIndex = 0;
            ushort version = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
            if(version != Map2DDataFileVersion)
            {
                EditorUtility.DisplayDialog("错误", "地图编辑器配置文件版本不匹配！", "关闭", "");
                cfgFile.Close();
                return false;
            }
            mMapViewScrollRate = BitConverter.ToSingle(buffer, readIndex);   readIndex += sizeof(float);
            mUnitImageWidth = BitConverter.ToUInt16(buffer, readIndex);  readIndex += sizeof(ushort);
            mUnitImageHeight = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
            mScrollViewPos.x = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
            mScrollViewPos.y = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
            mRowCountToggle = BitConverter.ToBoolean(buffer, readIndex); readIndex += sizeof(bool);
            mColCountToggle = BitConverter.ToBoolean(buffer, readIndex); readIndex += sizeof(bool);
            cfgFile.Close();
            return true;
        }
        bool SaveMap2DData()
        {
            FileStream newFile = File.OpenWrite(mDataFilePath);
            byte[] bin = BitConverter.GetBytes(Map2DDataFileVersion);  // data version
            newFile.Write(bin, 0, bin.Length);
            int rowCount = mMapUnitList.GetLength(0);
            int colCount = mMapUnitList.GetLength(1);
            bin = BitConverter.GetBytes(rowCount); newFile.Write(bin, 0, bin.Length);
            bin = BitConverter.GetBytes(colCount); newFile.Write(bin, 0, bin.Length);
            for (int i = 0; i < rowCount; ++ i )
            {
                for(int j = 0; j < colCount; ++j)
                {
                    Map2DGridUnit obj = mMapUnitList[i, j];
                    Texture2D tex = null;
                    if (null != obj && null != obj.GridUnit)
                    {
                        SpriteRenderer sr = obj.GridUnit.GetComponent<SpriteRenderer>();
                        if(null != sr && null != sr.sprite)
                            tex = sr.sprite.texture;
                    }
                    if (null == tex)
                    {
                        bin = BitConverter.GetBytes(-1); newFile.Write(bin, 0, bin.Length);
                        bin = BitConverter.GetBytes((ushort)0); newFile.Write(bin, 0, bin.Length);
                        bin = BitConverter.GetBytes((ushort)0); newFile.Write(bin, 0, bin.Length);
                    }
                    else
                    {
                        SpriteRenderer sr = obj.GridUnit.GetComponent<SpriteRenderer>();
                        bin = BitConverter.GetBytes(sr.sprite.texture.GetInstanceID()); newFile.Write(bin, 0, bin.Length);
                        bin = BitConverter.GetBytes((ushort)sr.sprite.texture.width); newFile.Write(bin, 0, bin.Length);
                        bin = BitConverter.GetBytes((ushort)sr.sprite.texture.height); newFile.Write(bin, 0, bin.Length);
                    }
                    if (null == obj)
                    {
                        bin = BitConverter.GetBytes((char)0);
                        newFile.Write(bin, 0, bin.Length);
                    }
                    else
                    {
                        int count = obj.GridUnit.transform.childCount;
                        bin = BitConverter.GetBytes((char)count); newFile.Write(bin, 0, bin.Length);
                        for (int c = 0; c < count; ++c)
                        {
                            Transform co = obj.GridUnit.transform.GetChild(c);
                            if (co.collider.GetType() == typeof(BoxCollider))
                            {
                                bin = BitConverter.GetBytes((short)PrimitiveType.Cube);
                                newFile.Write(bin, 0, bin.Length);
                            }
                            else if (co.collider.GetType() == typeof(Plane))
                            {
                                bin = BitConverter.GetBytes((short)PrimitiveType.Plane);
                                newFile.Write(bin, 0, bin.Length);
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("错误", "地图编辑器地图单元只支持Plane和Cube类型的碰撞！", "继续", "");
                                bin = BitConverter.GetBytes((short)-1); newFile.Write(bin, 0, bin.Length);
                                // roat
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                // pos
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                // size
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                bin = BitConverter.GetBytes(0.0f); newFile.Write(bin, 0, bin.Length);
                                continue;
                            }
                            // roat
                            bin = BitConverter.GetBytes(co.localRotation.x); newFile.Write(bin, 0, bin.Length);
                            bin = BitConverter.GetBytes(co.localRotation.y); newFile.Write(bin, 0, bin.Length);
                            bin = BitConverter.GetBytes(co.localRotation.z); newFile.Write(bin, 0, bin.Length);
                            bin = BitConverter.GetBytes(co.localRotation.w); newFile.Write(bin, 0, bin.Length);
                            // pos
                            bin = BitConverter.GetBytes(co.localPosition.x); newFile.Write(bin, 0, bin.Length);
                            bin = BitConverter.GetBytes(co.localPosition.y); newFile.Write(bin, 0, bin.Length);
                            bin = BitConverter.GetBytes(co.localPosition.z); newFile.Write(bin, 0, bin.Length);
                            // size
                            bin = BitConverter.GetBytes(co.localScale.x); newFile.Write(bin, 0, bin.Length);
                            bin = BitConverter.GetBytes(co.localScale.y); newFile.Write(bin, 0, bin.Length);
                            bin = BitConverter.GetBytes(co.localScale.z); newFile.Write(bin, 0, bin.Length);
                        }
                    }
                }
            }
            newFile.Close();
            return true;
        }
        bool ReadMap2DData()
        {
            if (!File.Exists(mDataFilePath))
                SaveMap2DData();
            FileStream dataFile = File.OpenRead(mDataFilePath);
            byte[] buffer = new byte[dataFile.Length];
            int length = dataFile.Read(buffer, 0, Convert.ToInt32(dataFile.Length));
            if (length != dataFile.Length)
            {
                EditorUtility.DisplayDialog("错误", "地图编辑器数据文件错误！", "关闭", "");
                dataFile.Close();
                return false;
            }
            int readIndex = 0;
            ushort version = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
            if (version != Map2DDataFileVersion)
            {
                EditorUtility.DisplayDialog("错误", "地图编辑器数据文件版本不匹配！", "关闭", "");
                dataFile.Close();
                return false;
            }
            int rowCount = BitConverter.ToInt32(buffer, readIndex); readIndex += sizeof(int);
            int colCount = BitConverter.ToInt32(buffer, readIndex); readIndex += sizeof(int);
            mMapUnitList = new Map2DGridUnit[rowCount, colCount];
            mMapUnitDictionary = new Dictionary<int, Map2DGridUnit>(rowCount * colCount);
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < colCount; ++j)
                {
                    mMapUnitList[i, j] = new Map2DGridUnit();
                    mMapUnitList[i, j].OnEditorMenuItemCommand += (Map2DGridUnit sender, int rIndex, int cIndex) =>
                                                                    {
                                                                        Map2DEditor.GetOrNewGridEditorForm(rIndex, cIndex);
                                                                    };
                    SpriteRenderer sr = mMapUnitList[i, j].GridUnit.GetComponent<SpriteRenderer>();
                    int instanceID = BitConverter.ToInt32(buffer, readIndex); readIndex += sizeof(int);
                    ushort texWidth = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
                    ushort texHeight =  BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
                    mMapUnitList[i, j].ImageSize = new Vector2((float)texWidth, (float)texHeight);
                    char childCount = BitConverter.ToChar(buffer, readIndex); readIndex += sizeof(char);
                    for (int c = 0; c < childCount; ++ c)
                    {
                        Quaternion roat = Quaternion.LookRotation(Vector3.zero);
                        Vector3 pos = Vector3.zero;
                        Vector3 scale = Vector3.zero;
                        GameObject child = null;
                        PrimitiveType colliderType = (PrimitiveType)BitConverter.ToInt16(buffer, readIndex); readIndex += sizeof(short);
                        // roat
                        roat.x = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        roat.y = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        roat.z = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        roat.w = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        // pos
                        pos.x = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        pos.y = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        pos.z = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        // size
                        scale.x = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        scale.y = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        scale.z = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        switch(colliderType)
                        {
                            case PrimitiveType.Cube:
                                {
                                    child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                }
                                break;
                            case PrimitiveType.Plane:
                                {
                                    child = GameObject.CreatePrimitive(PrimitiveType.Plane);
                                }
                                break;
                            default:
                                EditorUtility.DisplayDialog("错误", "地图编辑器数据文件中出现了未知的碰撞类型！", "关闭", "");
                                continue;
                        }
                        child.transform.localRotation = roat;
                        child.transform.localPosition = pos;
                        child.transform.localScale = scale;
                        child.transform.parent = mMapUnitList[i, j].GridUnit.transform;
                    }
                    if (instanceID != -1)
                    {
                        Texture2D tex = EditorUtility.InstanceIDToObject(instanceID) as Texture2D;
                        if (tex != null)
                            sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width, tex.height));
                    }
                }
            }
            dataFile.Close();
            return true;
        }
#endregion

#region 不重要的重写接口
        internal void DrawSceneGUI(SceneView view)
        {
            //Debug.Log(string.Format("on event button[{0}]", Event.current.button));
            //SceneView.RepaintAll();
        }
        void OnDisable()
        {
        }
        void OnFocus()
        {
            Debug.Log("当窗口获得焦点时调用一次");
        }
        void OnLostFocus()
        {
            Debug.Log("当窗口丢失焦点时调用一次");
        }
        void OnHierarchyChange()
        {
            Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
        }
        void OnProjectChange()
        {
            Debug.Log("当Project视图中的资源发生改变时调用一次");
        }

        void OnSelectionChange()
        {
            //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
            foreach (Transform t in Selection.transforms)
            {
                //有可能是多选，这里开启一个循环打印选中游戏对象的名称
                Debug.Log("OnSelectionChange" + t.name);
            }
        }
        void OnInspectorUpdate()
        {
            //这里开启窗口的重绘，不然窗口信息不会刷新
            this.Repaint();
        }
        void OnInspectorGUI()
        {
            Debug.Log("OnInspectorGUI");
        }
#endregion

#region EditorWindow开启与关闭后的回调
        void OnEnable()
        {
            string sceneName = EditorApplication.currentScene.Substring(EditorApplication.currentScene.LastIndexOf("/") + 1);
            sceneName = sceneName.Substring(0, sceneName.Length - sceneName.LastIndexOf(".") - 2);
            mCongigFilePath = string.Format("{0}/../Editor/Map2D/{1}.cfg", Application.dataPath, sceneName);
            mDataFilePath = string.Format("{0}/../Editor/Map2D/{1}.data", Application.dataPath, sceneName);
            ReadMap2DConfig();
            ReadMap2DData();
            wantsMouseMove = false;
        }
        void OnDestroy()
        {
            if (mSceneDelegate != null)
                SceneView.onSceneGUIDelegate -= mSceneDelegate;
            Map2DEditor.OnFormClosed(Map2DEditor.MainFormName);
            SaveMap2DConfig();
            SaveMap2DData();
            int rowCount = mMapUnitList.GetLength(0);
            int colCount = mMapUnitList.GetLength(1);
            for (int i = 0; i < rowCount; ++i)
            {
                for (int j = 0; j < colCount; ++j)
                {
                    mMapUnitList[i,j].Destroy();
                }
            }
            mMapUnitList = null;
            mMapUnitDictionary.Clear();
            mMapUnitDictionary = null;
        }
#endregion

#region 事件处理
        void OnObjectMenuItemSelected(object userData, string[] options, int selected)
        {

        }
        bool OnEvent(Event e)
        {
            //switch (e.type)
            //{
            //    case EventType.MouseDown:
            //        {
            //            if(e.button == 1)
            //            {
            //                Event se = new Event();
            //                se.button = 0;
            //                se.mousePosition = se.mousePosition;
            //                SendEvent(se);
            //                e.Use();
            //            }
            //        }
            //        break;
            //    case EventType.MouseUp:
            //        break;
            //    case EventType.MouseMove:
            //        break;
            //    case EventType.MouseDrag:
            //        break;
            //    case EventType.KeyDown:
            //        break;
            //    case EventType.KeyUp:
            //        break;
            //    case EventType.ScrollWheel:
            //        break;
            //    case EventType.Repaint:
            //        break;
            //    case EventType.Layout:
            //        break;
            //    case EventType.DragUpdated:
            //        break;
            //    case EventType.DragPerform:
            //        break;
            //    case EventType.Ignore:
            //        break;
            //    case EventType.Used:
            //        break;
            //    case EventType.ValidateCommand:
            //        break;
            //    case EventType.ExecuteCommand:
            //        break;
            //    case EventType.DragExited:
            //        break;
            //    case EventType.ContextClick:
            //        {
            //            var mousePos = e.mousePosition;
            //            //EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Assets/", null);
            //            GUIContent[] menuItems = new GUIContent[] { new GUIContent("编辑"), new GUIContent("创建模板") };
            //            EditorUtility.DisplayCustomMenu(new Rect(mousePos.x, mousePos.y, 0, 0), menuItems, 1, OnObjectMenuItemSelected, null);
            //            //EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Assets/", null);
            //            GUIContent[] menuItems = new GUIContent[] { new GUIContent("编辑"), new GUIContent("创建模板") };
            //            EditorUtility.DisplayCustomMenu(new Rect(mousePos.x, mousePos.y, 0, 0), menuItems, 1, OnObjectMenuItemSelected, null);
            //            e.Use();
            //        }
            //        break;
            //}
            return true;
        }
#endregion

#region 表现层
        void OnGUI()
        {
            //OnEvent(Event.current);
            GUILayout.BeginHorizontal();
            GUILayout.Label("图片宽度:");
            mUnitImageWidth = ushort.Parse(GUILayout.TextField(mUnitImageWidth.ToString(), GUILayout.Width(40)));
            GUILayout.Label("\t图片高度:");
            mUnitImageHeight = ushort.Parse(GUILayout.TextField(mUnitImageHeight.ToString(), GUILayout.Width(40)));
            //
            GUILayout.FlexibleSpace();
            int rowCount = mMapUnitList.GetLength(0);
            int colCount = mMapUnitList.GetLength(1);
            GUILayout.Label("单元行数:");
            int newRowCount = rowCount;
            if (mRowCountToggle)
                GUILayout.TextArea(rowCount.ToString(), GUILayout.Width(30));
            else
                newRowCount = int.Parse(GUILayout.TextField(rowCount.ToString(), GUILayout.Width(30)));
            mRowCountToggle = GUILayout.Toggle(mRowCountToggle, "锁定行");
            GUILayout.Label("单元列数:");
            int newColCount = colCount;
            if (mColCountToggle)
                GUILayout.TextArea(colCount.ToString(), GUILayout.Width(30));
            else
                newColCount = int.Parse(GUILayout.TextField(colCount.ToString(), GUILayout.Width(30)));
            mColCountToggle = GUILayout.Toggle(mColCountToggle, "锁定列");
            //
            GUILayout.FlexibleSpace();
            mMapViewScrollRate = EditorGUILayout.Slider(mMapViewScrollRate, 3, 1000.0f);
            GUILayout.EndHorizontal();
            mScrollViewPos = GUILayout.BeginScrollView(mScrollViewPos); // begin ver scroll bar
            GUILayout.BeginVertical();
            if (rowCount != newRowCount || colCount != newColCount)
            {
                var oldUnits = mMapUnitList.Clone() as Map2DGridUnit[,];
                mMapUnitList = new Map2DGridUnit[newRowCount, newColCount];
                mMapUnitDictionary = new Dictionary<int, Map2DGridUnit>(newRowCount * newColCount);
                for (int i = 0; i < newRowCount; ++i)
                {
                    for (int j = 0; j < newColCount; ++j)
                    {
                        if (i < rowCount && j < colCount)
                            mMapUnitList[i, j] = oldUnits[i, j];
                    }
                }
            }
            for (int i = 0; i < newRowCount; ++i)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < newColCount; ++j)
                {
                    Rect gridRect = new Rect();
                    gridRect.width = (float)mUnitImageWidth * (mMapViewScrollRate / 100.0f);
                    gridRect.height = (float)mUnitImageHeight * (mMapViewScrollRate / 100.0f);
                    gridRect.x = (float)j * gridRect.width;
                    gridRect.y = (float)i * gridRect.height;
                    int instanceID = i * newColCount + j;
                    int id = EditorGUIUtility.GetControlID(instanceID.GetHashCode(), FocusType.Passive, gridRect);
                    if (mMapUnitList[i, j] == null)
                    {
                        mMapUnitList[i, j] = GUIUtility.GetStateObject(typeof(Map2DGridUnit), id) as Map2DGridUnit;
                        mMapUnitList[i, j].OnEditorMenuItemCommand += (Map2DGridUnit sender, int rIndex, int cIndex) =>
                                                                        {
                                                                            Map2DEditor.GetOrNewGridEditorForm(rIndex, cIndex);
                                                                        };
                    }
                    Texture2D newTex = EditorGUILayout.ObjectField(mMapUnitList[i, j].Image, typeof(Texture2D), true,
                        GUILayout.Width(gridRect.width), GUILayout.Height(gridRect.height)) as Texture2D;
                    if(newTex != null)
                    {
                        if(mUnitImageWidth < (ushort)newTex.width)
                            mUnitImageWidth = (ushort)newTex.width;
                        if (mUnitImageHeight < (ushort)newTex.height)
                            mUnitImageHeight = (ushort)newTex.height;
                    }
                    mMapUnitList[i, j].Init(id, i, j, gridRect, newTex);
                    mMapUnitList[i, j].ImageSize = new Vector2((float)mUnitImageWidth, (float)mUnitImageHeight);
                    mMapUnitDictionary[id] = mMapUnitList[i, j];
                    mMapUnitList[i, j].OnEvent(Event.current);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
#endregion
    }
}
#endif