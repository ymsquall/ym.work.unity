#if UNITY_EDITOR

using System;
using System.Xml;
using System.Collections; 
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
        string mSceneName;
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
        public bool SaveMap2DData()
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
                    Texture2D tex = null != obj ? obj.Image : null;
                    if (null == tex)
                    {
                        bin = BitConverter.GetBytes((ushort)0); newFile.Write(bin, 0, bin.Length);
                        bin = BitConverter.GetBytes((ushort)0); newFile.Write(bin, 0, bin.Length);
                        bin = BitConverter.GetBytes((ushort)0); newFile.Write(bin, 0, bin.Length);
                    }
                    else
                    {
                        string assetPath = AssetDatabase.GetAssetPath(tex.GetInstanceID());
                        byte[] bytePath = System.Text.Encoding.Default.GetBytes(assetPath);
                        bin = BitConverter.GetBytes((ushort)bytePath.Length); newFile.Write(bin, 0, bin.Length);
                        newFile.Write(bytePath, 0, bytePath.Length);
                        bin = BitConverter.GetBytes((ushort)tex.width); newFile.Write(bin, 0, bin.Length);
                        bin = BitConverter.GetBytes((ushort)tex.height); newFile.Write(bin, 0, bin.Length);
                    }
                    if (null == obj)
                    {
                        bin = BitConverter.GetBytes((char)0);
                        newFile.Write(bin, 0, bin.Length);
                    }
                    else
                    {
                        if (!obj.SaveGridUnit(newFile))
                        {
                            newFile.Close();
                            return false;
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
                    Map2DGridUnit gridUnit = new Map2DGridUnit();
                    mMapUnitList[i, j] = gridUnit;
                    string texPath = "";
                    ushort pathLength = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
                    if (pathLength > 0)
                    {
                        texPath = System.Text.Encoding.Default.GetString(buffer, readIndex, pathLength);
                        //texPath = BitConverter.ToString(buffer, readIndex, pathLength);
                        readIndex += pathLength;
                    }
                    ushort texWidth = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
                    ushort texHeight = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
                    gridUnit.Image = AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D)) as Texture2D;
                    gridUnit.ImageSize = new Vector2((float)texWidth, (float)texHeight);
                    if(!gridUnit.ReadImageSubs(buffer, ref readIndex))
                    {
                        dataFile.Close();
                        return false;
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
            mSceneName = sceneName.Substring(0, sceneName.LastIndexOf("."));
            mCongigFilePath = string.Format("{0}/../Editor/Map2D/{1}.cfg", Application.dataPath, mSceneName);
            mDataFilePath = string.Format("{0}/Scenes/{1}/Map2D_{1}.bin", Application.dataPath, mSceneName);
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
                    mMapUnitList[i, j].Destroy();
                }
            }
            mMapUnitList = null;
            mMapUnitDictionary.Clear();
            mMapUnitDictionary = null;
        }
#endregion

#region 事件处理
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
            // status
            EditorGUILayout.BeginVertical();
            // context
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
                    int id = EditorGUIUtility.GetControlID(FocusType.Passive);
                    if (mMapUnitList[i, j] == null)
                        mMapUnitList[i, j] = GUIUtility.GetStateObject(typeof(Map2DGridUnit), id) as Map2DGridUnit;
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
            // status
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出服务器数据"))
                ExportServerData();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
#endregion

#region 数据导出

        Vector2 SceneSize
        {
            get
            {
                int rowCount = mMapUnitList.GetLength(0);
                int colCount = mMapUnitList.GetLength(1);
                return new Vector2(colCount * mUnitImageWidth, rowCount * mUnitImageHeight);
            }
        }

        bool ExportServerData()
        {
            string path = string.Format("{0}/../Editor/Map2D/{1}.xml", Application.dataPath, mSceneName);
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration decl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "no");
            XmlElement root = xmlDoc.DocumentElement;
            xmlDoc.InsertBefore(decl, root);
            // world  
            XmlElement world = xmlDoc.CreateElement("world");
            xmlDoc.AppendChild(world);
            // map  
            XmlElement map = xmlDoc.CreateElement("map");
            map.SetAttribute("id", mSceneName);
            map.SetAttribute("width", SceneSize.x.ToString());
            map.SetAttribute("height", SceneSize.y.ToString());
            map.SetAttribute("bx", "0");
            map.SetAttribute("by", "0");
            world.AppendChild(map);
            // blocks  
            List<Map2DGridUnit.ImageSubData> blockList = new List<Map2DGridUnit.ImageSubData>(0);
            List<Map2DGridUnit.ImageSubData> npcList = new List<Map2DGridUnit.ImageSubData>(0);
            foreach(Map2DGridUnit unit in mMapUnitList)
            {
                unit.FillXmlData(ref blockList, ref npcList, SceneSize);
            }
            int idIndex = 0;
            XmlElement blocks = xmlDoc.CreateElement("blocks");
            foreach (Map2DGridUnit.ImageSubData b in blockList)
            {
                XmlElement block = xmlDoc.CreateElement("b");
                block.SetAttribute("id", idIndex.ToString());
                block.SetAttribute("x", b.range.x.ToString());
                block.SetAttribute("y", b.range.y.ToString());
                block.SetAttribute("width", b.range.width.ToString());
                block.SetAttribute("height", b.range.height.ToString());
                block.SetAttribute("type", b.type.ToString());
                blocks.AppendChild(block);
                idIndex++;
            }
            map.AppendChild(blocks);
            idIndex = 0;
            XmlElement npcs = xmlDoc.CreateElement("npcs");
            foreach (Map2DGridUnit.ImageSubData n in npcList)
            {
                XmlElement npc = xmlDoc.CreateElement("n");
                npc.SetAttribute("id", idIndex.ToString());
                npc.SetAttribute("x", n.range.x.ToString());
                npc.SetAttribute("y", n.range.y.ToString());
                npc.SetAttribute("type", n.type.ToString());
                npcs.AppendChild(npc);
                idIndex++;
            }
            map.AppendChild(npcs);
            xmlDoc.Save(path);
            return true;
        }

#endregion
    }
}
#endif