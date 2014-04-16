#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Assets.Script.Tools;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridEditorForm : EditorBaseWindow<Map2DGridEditorForm>
    {
        int mGridRowIndex = -1, mGridColIndex = -1;
        bool mInited = false;
        Map2DGridEditorImageView mImageView;
        Map2DGridEditorPropertyView mPropertyView;
        Dictionary<string, List<Map2DGridUnit.ImageSubData>> mTemplateList = new Dictionary<string,List<Map2DGridUnit.ImageSubData>>();

        float mToolboxWidth = 100.0f;
        float mPropertyHeight = 120.0f;

        public int GridRowIndex
        {
            set { mGridRowIndex = value; }
            get { return mGridRowIndex; }
        }
        public int GridColIndex
        {
            set { mGridColIndex = value; }
            get { return mGridColIndex; }
        }

        public List<Map2DGridUnit.ImageSubData> ImageSubList
        {
            get
            {
                return mImageView.ImageSubList;
            }
        }
        void OnEnable()
        {
            wantsMouseMove = true;
            string templatePath = string.Format("{0}/../Editor/Map2D/Templates/Grids/", Application.dataPath, TemplateFileName);
            string[] files = FileUtils.EnumAllFilesByPath(templatePath, false);
            foreach(string f in files)
            {
                string fileName = f.Replace(templatePath, "");
                mTemplateList.Add(fileName, new List<Map2DGridUnit.ImageSubData>(0));
                FileStream dataFile = File.OpenRead(f);
                byte[] buffer = new byte[dataFile.Length];
                int length = dataFile.Read(buffer, 0, Convert.ToInt32(dataFile.Length));
                int readIndex = 0;
                int count = BitConverter.ToInt32(buffer, readIndex); readIndex += sizeof(Int32);
                for(int i = 0; i < count; ++ i)
                {
                    Map2DGridUnit.ImageSubData data = new Map2DGridUnit.ImageSubData();
                    data.type = (Map2DGridImageSubType)BitConverter.ToChar(buffer, readIndex); readIndex += sizeof(char);
                    data.range = new Rect(0, 0, 0, 0);
                    data.range.x = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                    data.range.y = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                    data.range.width = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                    data.range.height = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                    int imageFileLength = BitConverter.ToUInt16(buffer, readIndex); readIndex += sizeof(ushort);
                    if (imageFileLength > 0)
                    {
                        data.imageFile = System.Text.Encoding.Default.GetString(buffer, readIndex, imageFileLength);
                        readIndex += imageFileLength;
                        data.collider.x = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        data.collider.y = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        data.collider.width = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                        data.collider.height = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                    }
                    mTemplateList[fileName].Add(data);
                }
                dataFile.Close();
            }
        }
        void OnDestroy()
        {
            Map2DEditor.DoSaved();
            Map2DEditor.OnFormClosed(this.name);
        }
        public void Init()
        {
            Map2DGridUnit grid = Map2DEditor.FindGridUnitByIndex(GridRowIndex, GridColIndex);
            foreach (Map2DGridUnit.ImageSubData d in grid.ImageSubList)
            {
                mImageView.AddImageSubItem(d.type, d.range, d.imageFile, d.collider);
            }
            mImageView.OnSaveGridToTemplate += OnSaveGridToTemplate;
            mImageView.OnResetGridFromTemplate += OnResetGridFromTemplate;
            mInited = true;
        }
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            int id = EditorGUIUtility.GetControlID(FocusType.Passive);
            mImageView = EditorGUIUtility.GetStateObject(typeof(Map2DGridEditorImageView), id) as Map2DGridEditorImageView;
            if (mImageView != null)
            {
                if(!mInited)
                {
                    Init();
                }
                Rect rc = new Rect(0, 0, position.width - mToolboxWidth, position.height - mPropertyHeight);
                mImageView.Init(id, this, rc);
                mImageView.OnGUI();
            }
            GUILayout.Box("", GUILayout.Width(5), GUILayout.Height(position.height - mPropertyHeight));
            id = EditorGUIUtility.GetControlID(FocusType.Passive);
            Map2DGridEditorToolboxView toolbox = EditorGUIUtility.GetStateObject(typeof(Map2DGridEditorToolboxView), id) as Map2DGridEditorToolboxView;
            if (toolbox != null)
            {
                Rect rc = new Rect(position.width - mToolboxWidth + 5, 0, mToolboxWidth - 5, position.height - mPropertyHeight);
                toolbox.Init(id, this, rc);
                toolbox.OnGUI();
            }
            GUILayout.EndHorizontal();

            GUILayout.Box("", GUILayout.Width(position.width), GUILayout.Height(5));
            EditorGUILayout.BeginHorizontal();
            id = EditorGUIUtility.GetControlID(FocusType.Keyboard);
            mPropertyView = EditorGUIUtility.GetStateObject(typeof(Map2DGridEditorPropertyView), id) as Map2DGridEditorPropertyView;
            if (mPropertyView != null)
            {
                Rect rc = new Rect(0, position.height - mPropertyHeight + 5, position.width, mPropertyHeight - 5);
                mPropertyView.Init(id, this, rc);
                mPropertyView.OnGUI();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        public void OnImageSubItemSelected(Map2DGridEditorImageSubItem item)
        {
            mPropertyView.Selected = item;
        }
        public void OnDeSelectImageSubItem(Map2DGridEditorImageSubItem item)
        {
            mPropertyView.Selected = null;
        }
        string TemplateFileName
        {
            get
            {
                string imgFilePath = "";
                Map2DGridUnit grid = Map2DEditor.FindGridUnitByIndex(GridRowIndex, GridColIndex);
                if (null != grid && null != grid.Image)
                {
                    imgFilePath = AssetDatabase.GetAssetPath(grid.Image.GetInstanceID());
                    //imgFilePath = imgFilePath.Substring(0, imgFilePath.LastIndexOf("."));
                }
                else
                {
                    imgFilePath = EditorApplication.currentScene;
                    imgFilePath = imgFilePath.Substring(0, imgFilePath.LastIndexOf("."));
                    imgFilePath += string.Format(".{0}_{1}", GridRowIndex, GridColIndex);
                }
                imgFilePath = imgFilePath.Replace('/', '.');
                return imgFilePath;
            }
        }
        void OnSaveGridToTemplate(Map2DGridEditorImageView sender)
        {
            sender = sender != null ? sender : mImageView;
            string templateFilePath = string.Format("{0}/../Editor/Map2D/Templates/Grids/{1}", Application.dataPath, TemplateFileName);
            FileStream newFile = File.OpenWrite(templateFilePath);
            byte[] bin = BitConverter.GetBytes((Int32)sender.ImageSubList.Count); newFile.Write(bin, 0, bin.Length);
            foreach (Map2DGridUnit.ImageSubData d in sender.ImageSubList)
            {
                bin = BitConverter.GetBytes((char)d.type); newFile.Write(bin, 0, bin.Length);
                bin = BitConverter.GetBytes(d.range.x); newFile.Write(bin, 0, bin.Length);
                bin = BitConverter.GetBytes(d.range.y); newFile.Write(bin, 0, bin.Length);
                bin = BitConverter.GetBytes(d.range.width); newFile.Write(bin, 0, bin.Length);
                bin = BitConverter.GetBytes(d.range.height); newFile.Write(bin, 0, bin.Length);
                if (d.imageFile.Length > 0)
                {
                    byte[] bytePath = System.Text.Encoding.Default.GetBytes(d.imageFile);
                    bin = BitConverter.GetBytes((ushort)bytePath.Length); newFile.Write(bin, 0, bin.Length);
                    newFile.Write(bytePath, 0, bytePath.Length);
                    bin = BitConverter.GetBytes(d.collider.x); newFile.Write(bin, 0, bin.Length);
                    bin = BitConverter.GetBytes(d.collider.y); newFile.Write(bin, 0, bin.Length);
                    bin = BitConverter.GetBytes(d.collider.width); newFile.Write(bin, 0, bin.Length);
                    bin = BitConverter.GetBytes(d.collider.height); newFile.Write(bin, 0, bin.Length);
                }
                else
                {
                    bin = BitConverter.GetBytes((ushort)0);
                    newFile.Write(bin, 0, bin.Length);
                }
            }
            newFile.Close();
            if (mTemplateList.ContainsKey(TemplateFileName))
                mTemplateList[TemplateFileName] = sender.ImageSubList;
        }
        void OnResetGridFromTemplate(Map2DGridEditorImageView sender)
        {
            if (mTemplateList.ContainsKey(TemplateFileName))
            {
                mImageView.ResetFromList(mTemplateList[TemplateFileName]);
            }
        }
    }
}

#endif