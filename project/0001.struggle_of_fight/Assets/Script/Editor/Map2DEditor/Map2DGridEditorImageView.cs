#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridEditorImageSubItem : IEditorMouseDragItem
    {
        public Map2DGridEditorImageSubItem(string type, Vector2 pos)
        {
            mImageSubType = type;
            if (Map2DGridEditorToolboxView.ToolsTips[0] == mImageSubType)
            {
                mRegion.width = 100; mRegion.height = 10;
            }
            else if (Map2DGridEditorToolboxView.ToolsTips[1] == mImageSubType)
            {
                mRegion.width = 20; mRegion.height = 100;
            }
            else if (Map2DGridEditorToolboxView.ToolsTips[2] == mImageSubType)
            {
                mRegion.width = 60; mRegion.height = 60;
            }
            mRegion.x = pos.x - mRegion.width * 0.5f;
            mRegion.y = pos.y - mRegion.height * 0.5f;
        }
        int mControlID = -1;
        Map2DGridEditorImageView mParent = null;
        string mImageSubType;
        Rect mRegion = new Rect(0, 0, 0, 0);
        Vector2 mBeginDraginPos;
        Vector2 mDraginPosOffset;

        public delegate void OnMouseLBEvent(Map2DGridEditorImageSubItem sender);
        public OnMouseLBEvent OnMouseLBUpInOutside;
        public OnMouseLBEvent OnMouseLBUpInSide;

        public int ControlID
        {
            get { return mControlID; }
        }
        public Map2DGridEditorImageView Parent
        {
            set { mParent = value; }
            get { return mParent; }
        }
        public Rect WorldRange
        {
            get
            {
                Rect ret = mRegion;
                ret.x += mParent.Position.x;
                ret.y += mParent.Position.y;
                ret.width *= mParent.Scale;
                ret.height *= mParent.Scale;
                return ret;
            }
        }
#region IEditorMouseDragItem
        MouseDragItemType IEditorMouseDragItem.ItemType
        {
            get { return MouseDragItemType.ImageSubItem; }
        }
        void IEditorMouseDragItem.OnBeginDragin(Vector2 pos)
        {
            mBeginDraginPos = pos;
            mDraginPosOffset = pos - mRegion.center;
            mDraginPosOffset.x -= 1;
            mDraginPosOffset.y -= 1;
        }
        void IEditorMouseDragItem.DrawDraginGUI(Vector2 pos)
        {
            Vector2 realPos = pos - mDraginPosOffset - mParent.Position;
            mRegion.center = realPos;
            //mRegion.x = realPos.x - mRegion.width;
            //mRegion.y = realPos.y - mRegion.height;
            //NGUIEditorTools.DrawTexture(NGUIEditorTools.blankTexture, new Rect(mRegion.center.x, mRegion.center.y, mRegion.width, mRegion.height), new Rect(0, 0, 1, 1), Map2DEditor.ColorByToolType(mImageSubType));
        }
#endregion
        public void OnGUI()
        {
            mControlID = EditorGUIUtility.GetControlID(FocusType.Native);
            NGUIEditorTools.DrawTexture(NGUIEditorTools.blankTexture, WorldRange, new Rect(0, 0, 1, 1), Map2DEditor.ColorByToolType(mImageSubType));
            OnEvent(Event.current);
        }
        void OnEvent(Event e)
        {
            if (e.type == EventType.Layout || e.type == EventType.Repaint)
                return;
            EventType et = e.GetTypeForControl(ControlID);
            if (WorldRange.Contains(e.mousePosition))
            {
                Debug.Log(et.ToString());
                switch(et)
                {
                    case EventType.MouseDown:
                        {
                            if(e.button == 0)
                            {
                                EditorMouseDelegate.Current.EndDrag(e.button);
                                EditorMouseDelegate.Current.BeginDrag(e, this);
                                e.Use();
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        {
                            if (e.button == 0)
                            {
                                if (EditorMouseDelegate.Current.DraginType == MouseDragItemType.ImageSubItem)
                                {
                                    e.Use();
                                }
                            }
                        }
                        break;
                    case EventType.MouseUp:
                        {
                            if (e.button == 0)
                            {
                                if (null != OnMouseLBUpInSide)
                                    OnMouseLBUpInSide(this);
                                //e.Use();
                            }
                        }
                        break;
                }
            }
            else if (EditorMouseDelegate.Current.DraginType == MouseDragItemType.ImageSubItem)
            {
                switch (et)
                {
                    case EventType.MouseDrag:
                        {
                            if (e.button == 0)
                            {
                                if (EditorMouseDelegate.Current.DraginType == MouseDragItemType.ImageSubItem)
                                {
                                    e.Use();
                                }
                            }
                        }
                        break;
                    case EventType.MouseUp:
                        {
                            if (e.button == 0)
                            {
                                if (null != OnMouseLBUpInSide)
                                    OnMouseLBUpInSide(this);
                                //e.Use();
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (et)
                {
                    case EventType.MouseUp:
                        {
                            if (e.button == 0)
                            {
                                EditorMouseDelegate.Current.EndDrag(e.button);
                                if (null != OnMouseLBUpInOutside)
                                    OnMouseLBUpInOutside(this);
                                //e.Use();
                            }
                        }
                        break;
                }
            }
        }
    }
    public class Map2DGridEditorImageView
    {
        EditorWindow mParent;
        float mImageScrollScale = 1.0f;
        Vector2 mImageScrollPos = Vector2.zero;
        Rect mRegion = new Rect(0, 0, 0, 0);
        Rect mImageRealViewSize = new Rect(0, 0, 0, 0);
        int mControlID = -1;
        List<Map2DGridEditorImageSubItem> mImageSubList = new List<Map2DGridEditorImageSubItem>(0);

        struct ImageSubbject
        {
            int id;
            Transform obj;
            Rect rc;
        }

        public int ControlID
        {
            get { return mControlID; }
        }
        public EditorWindow Parent
        {
            get { return mParent; }
        }
        public Vector2 Position
        {
            get { return mImageScrollPos; }
        }
        public float Scale { get { return mImageScrollScale; } }
        public Map2DGridEditorImageSubItem AddImageSubItem(string type, Vector2 pos)
        {
            //int count = mImageSubList.Count;
            Map2DGridEditorImageSubItem item = new Map2DGridEditorImageSubItem(type, pos);
            item.Parent = this;
            item.OnMouseLBUpInOutside += OnClearSelectedItem;
            item.OnMouseLBUpInSide += OnSelectedItem;
            mImageSubList.Add(item);
            return item;
        }
        public void Init(int controlID, EditorWindow parent, Rect rc)
        {
            mControlID = controlID;
            mParent = parent;
            mRegion = rc;
        }
        public void OnGUI()
        {
            var parent = mParent as Map2DGridEditorForm;
            Map2DGridUnit grid = Map2DEditor.FindGridUnitByIndex(parent.GridRowIndex, parent.GridColIndex);
            if (null == grid)
                return;
            Texture image = grid.Image;
            if (null == image)
                image = Texture2D.CreateExternalTexture((int)grid.ImageSize.x, (int)grid.ImageSize.y, TextureFormat.Alpha8, false, false, IntPtr.Zero);
            //mImageScrollPos = EditorGUILayout.BeginScrollView(mImageScrollPos, false, false);
            EditorGUILayout.BeginScrollView(mImageScrollPos, GUILayout.Width(mRegion.width), GUILayout.Height(mRegion.height));
            mImageRealViewSize.width = image.width * mImageScrollScale;
            mImageRealViewSize.height = image.height * mImageScrollScale;
            GUILayout.BeginArea(mRegion);
            Rect newRect = mImageRealViewSize;
            newRect.x = mImageScrollPos.x;
            newRect.y = mImageScrollPos.y;
            GUI.DrawTexture(newRect, image, ScaleMode.ScaleToFit);
            OnEvent(Event.current);
            foreach (Map2DGridEditorImageSubItem item in mImageSubList)
            {
                item.OnGUI();
            }
            for (int i = 0; i < EditorMouseDelegate.Current.DraginCount; ++i)
            {
                IEditorMouseDragItem item = EditorMouseDelegate.Current[i];
                //if (item.ItemType != MouseDragItemType.ToolboxItem)
                //    continue;
                item.DrawDraginGUI(Event.current.mousePosition);
            }
            GUILayout.EndArea();
            EditorGUILayout.EndScrollView();
        }
        void OnEvent(Event e)
        {
            if (e.type == EventType.Layout || e.type == EventType.Repaint)
                return;
            if (!mRegion.Contains(e.mousePosition))
            {
                if (EditorMouseDelegate.Current.InDrag(MouseButtonType.middel))
                    EditorMouseDelegate.Current.EndDrag((int)MouseButtonType.middel);
                return;
            }
            EventType et = e.GetTypeForControl(ControlID);
            switch (et)
            {
                case EventType.MouseDrag:
                    {
                        if (e.button == 2)
                        {
                            mImageScrollPos += e.delta;
                            // edge check
                            e.Use();
                        }
                    }
                    break;
                case EventType.ScrollWheel:
                    {
                        mImageScrollScale -= e.delta.y * 0.01f;
                        e.Use();
                    }
                    break;
            }
            if (mImageRealViewSize.Contains(e.mousePosition))
            {
                switch (et)
                {
                    case EventType.MouseUp:
                        {
                            if (e.button == 0)
                            {
                                bool dragItem = false;
                                for (int i = 0; i < EditorMouseDelegate.Current.DraginCount; ++i)
                                {
                                    IEditorMouseDragItem item = EditorMouseDelegate.Current[i];
                                    if (item.ItemType != MouseDragItemType.ToolboxItem)
                                        continue;
                                    Map2DGridEditorToolboxItem toolItem = item as Map2DGridEditorToolboxItem;
                                    AddImageSubItem(toolItem.ToolTips, e.mousePosition);
                                    dragItem = true;
                                }
                                EditorMouseDelegate.Current.EndDrag(e.button);
                                if (dragItem)
                                    e.Use();
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        {
                            if (e.button == 0)
                            {
                                //for (int i = 0; i < EditorMouseDelegate.Current.DraginCount; ++i)
                                //{
                                //    IEditorMouseDragItem item = EditorMouseDelegate.Current[i];
                                //    if (item.ItemType != MouseDragItemType.ToolboxItem)
                                //        continue;
                                //    item.DrawDraginGUI(e.mousePosition);
                                //}
                                //Vector2 beginPos = e.mousePosition - e.delta;
                                //int width = Math.Abs((int)e.delta.x);
                                //int height = Math.Abs((int)e.delta.y);
                                //if (width > 0 || height > 0)
                                //{
                                //    if (width == 0)
                                //        width = 5;
                                //    if (height == 0)
                                //        height = 5;
                                //}
                                //width = 100;
                                //height = 100;
                                //GUI.DrawTexture(new Rect(0, 0, 100, 100), g_sDraginTexture, ScaleMode.ScaleToFit);
                                //EditorGUIUtility.DrawColorSwatch(new Rect(beginPos.x, beginPos.y, e.delta.x, e.delta.y), Color.white);
                                //EditorGUI.DrawRect(new Rect(beginPos.x, beginPos.y, e.delta.x, e.delta.y), Color.white);
                                //EditorMouseDelegate.Current.BeginDrag(e);
                                if (EditorMouseDelegate.Current.DraginType == MouseDragItemType.ToolboxItem)
                                    e.Use();
                            }
                        }
                        break;
                }
            }
        }
        void OnClearSelectedItem(Map2DGridEditorImageSubItem sender)
        {

        }
        void OnSelectedItem(Map2DGridEditorImageSubItem sender)
        {

        }
    }
}

#endif