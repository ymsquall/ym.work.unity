#if UNITY_EDITOR

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public enum Map2DGridImageSubType
    {
        地面,
        墙壁,
        刷怪点,
        //ground = "平台",
        //wall = "墙壁",
        //monster_pt = "刷怪点",
        max
    }
    public class Map2DGridUnit
    {
        static GUIContent[] MRBMenuItemList = 
            new GUIContent[] { new GUIContent("编辑单元"),
                             };
        int mControlID = -1;
        int mRowIndex = -1;
        int mColIndex = -1;
        Rect mViewRect;
        Vector2 mImageSize = Vector2.zero;
        Texture2D mImage = null;

        public struct ImageSubData
        {
            public Map2DGridImageSubType type;
            public Rect range;
        }
        List<ImageSubData> mImageSubList = new List<ImageSubData>(0);

        public int ControlID { get { return mControlID; } }
        public int RowIndex { get { return mRowIndex; } }
        public int ColIndex { get { return mColIndex; } }
        public Rect ViewRect { get { return mViewRect; } }
        public Texture2D Image
        {
            set { mImage = value; }
            get { return mImage; }
        }
        public Vector2 ImageSize
        {
            set { mImageSize = value; }
            get { return mImageSize; }
        }
        public List<ImageSubData> ImageSubList
        {
            get { return mImageSubList; }
        }

        public void Init(int id, int rIndex, int cIndex, Rect rc, Texture2D img)
        {
            mControlID = id;
            mRowIndex = rIndex;
            mColIndex = cIndex;
            mViewRect = rc;
            Image = img;
        }
        public void Destroy()
        {
            Map2DGridEditorForm form = Map2DEditor.GetOrNewGridEditorForm(mRowIndex, mColIndex, false);
            if (null != form)
                form.Close();
        }
        public void OnEvent(Event e)
        {
            if (!mViewRect.Contains(e.mousePosition))
                return;
            EventType et = e.GetTypeForControl(mControlID);
            switch (et)
            {
                //case EventType.MouseUp:
                case EventType.ContextClick:
                    {
                        if (e.button == 1)
                        {
                            var mousePos = e.mousePosition;
                            //EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Assets/", null);
                            EditorUtility.DisplayCustomMenu(new Rect(mousePos.x, mousePos.y, 0, 0), MRBMenuItemList, 1, OnGridUnitMenuItemCommands, null);
                            //e.Use();
                        }
                    }
                    break;
            }
        }
        void OnGridUnitMenuItemCommands(object userData, string[] options, int selected)
        {
            switch(selected)
            {
                case 0:
                    {
                        Map2DEditor.GetOrNewGridEditorForm(RowIndex, ColIndex);
                    }
                    break;
            }
        }
        public bool ReadImageSubs(byte[] buffer, ref int readIndex)
        {
            int childCount = BitConverter.ToChar(buffer, readIndex); readIndex += sizeof(char);
            for (int c = 0; c < childCount; ++c)
            {
                ImageSubData data = new ImageSubData();
                data.type = (Map2DGridImageSubType)BitConverter.ToInt16(buffer, readIndex); readIndex += sizeof(short);
                // renge
                data.range = new Rect(0, 0, 0, 0);
                data.range.x = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                data.range.y = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                data.range.width = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                data.range.height = BitConverter.ToSingle(buffer, readIndex); readIndex += sizeof(float);
                switch (data.type)
                {
                    case Map2DGridImageSubType.地面:
                    case Map2DGridImageSubType.墙壁:
                    case Map2DGridImageSubType.刷怪点:
                        mImageSubList.Add(data);
                        break;
                    default:
                        EditorUtility.DisplayDialog("错误", "地图编辑器数据文件中出现了未知的碰撞类型！", "关闭", "");
                        continue;
                }
            }
            return true;
        }
        public bool SaveGridUnit(FileStream file)
        {
            Map2DGridEditorForm gridEditor = Map2DEditor.GetOrNewGridEditorForm(RowIndex, ColIndex, false);
            if (null != gridEditor)
                mImageSubList = gridEditor.ImageSubList;
            byte[] bin = BitConverter.GetBytes((char)mImageSubList.Count); file.Write(bin, 0, bin.Length);
            foreach (ImageSubData d in mImageSubList)
            {
                bin = BitConverter.GetBytes((short)d.type); file.Write(bin, 0, bin.Length);
                // range
                bin = BitConverter.GetBytes(d.range.x); file.Write(bin, 0, bin.Length);
                bin = BitConverter.GetBytes(d.range.y); file.Write(bin, 0, bin.Length);
                bin = BitConverter.GetBytes(d.range.width); file.Write(bin, 0, bin.Length);
                bin = BitConverter.GetBytes(d.range.height); file.Write(bin, 0, bin.Length);
            }
            return true;
        }
        public bool FillXmlData(ref List<ImageSubData> blockList, ref List<ImageSubData> npcList, Vector2 sceneSize)
        {
            //Vector2 mapUnitOffPos = new Vector2(ImageSize.x * ColIndex, ImageSize.y * RowIndex);
            Vector2 mapUnitOffPos = new Vector2(ImageSize.x * ColIndex - sceneSize.x / 2.0f,
                                                ImageSize.y * RowIndex - sceneSize.y / 2.0f);
            foreach(ImageSubData d in mImageSubList)
            {
                ImageSubData newData = d;
                newData.range.x += mapUnitOffPos.x;
                newData.range.y += mapUnitOffPos.y;
                if(d.type == Map2DGridImageSubType.地面 || d.type == Map2DGridImageSubType.墙壁)
                {
                    blockList.Add(newData);
                }
                else if(d.type == Map2DGridImageSubType.刷怪点)
                {
                    npcList.Add(newData);
                }
            }
            return true;
        }
    }
}

#endif