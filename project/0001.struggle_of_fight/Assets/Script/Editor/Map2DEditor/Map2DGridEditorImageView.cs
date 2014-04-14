#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridEditorImageSubItem : IEditorMouseDragItem
    {
        public Map2DGridEditorImageSubItem(Map2DGridImageSubType type, Vector2 pos, Map2DGridEditorImageView parent)
        {
            mImageSubType = type;
            mParent = parent;
            pos = Point2WorldPoint(pos);
            switch(mImageSubType)
            {
                case Map2DGridImageSubType.地面:
                    mRange.width = 100; mRange.height = 10;
                    break;
                case Map2DGridImageSubType.墙壁:
                    mRange.width = 20; mRange.height = 100;
                    break;
                case Map2DGridImageSubType.刷怪点:
                    mRange.width = 60; mRange.height = 60;
                    break;
            }
            mRange.x = pos.x - mRange.width * 0.5f;
            mRange.y = pos.y - mRange.height * 0.5f;
        }
        public Map2DGridEditorImageSubItem(Map2DGridImageSubType type, Rect range, Map2DGridEditorImageView parent)
        {
            mImageSubType = type;
            mParent = parent;
            mRange = range;
        }
        int mControlID = -1;
        Map2DGridEditorImageView mParent = null;
        Map2DGridImageSubType mImageSubType;
        Rect mRange = new Rect(0, 0, 0, 0);
        //Vector2 mBeginDraginPos;

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
        public Map2DGridImageSubType ImageSubType
        {
            set { mImageSubType = value; }
            get { return mImageSubType; }
        }
        public Rect LocalRange
        {
            set { mRange = value; }
            get { return mRange; }
        }
        public Rect WorldRange
        {
            get
            {
                Rect ret = mRange;
                ret.x = mParent.Position.x + mRange.x * mParent.Scale;
                ret.y = mParent.Position.y + mRange.y * mParent.Scale;
                ret.width *= mParent.Scale;
                ret.height *= mParent.Scale;
                return ret;
            }
        }
        public Vector2 Point2WorldPoint(Vector2 pos)
        {
            return pos / mParent.Scale;
        }
#region IEditorMouseDragItem
        MouseDragItemType IEditorMouseDragItem.ItemType
        {
            get { return MouseDragItemType.ImageSubItem; }
        }
        void IEditorMouseDragItem.OnBeginDragin(Vector2 pos)
        {
            //mBeginDraginPos = pos;
        }
        void IEditorMouseDragItem.DrawDraginGUI(Vector2 pos)
        {
            mRange.center += Event.current.delta / mParent.Scale;
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
            bool inRange = WorldRange.Contains(e.mousePosition);
            bool draginMe = EditorMouseDelegate.Current.DraginItem(this);
            EventType et = e.GetTypeForControl(ControlID);
            switch(et)
            {
                case EventType.MouseDown:
                    {
                        if(e.button == 0)
                        {
                            EditorMouseDelegate.Current.EndDrag(e.button);
                            if(inRange)
                            {
                                EditorMouseDelegate.Current.BeginDrag(e, this);
                                e.Use();
                            }
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        if (e.button == 0)
                        {
                            if (draginMe)
                            {
                                ((IEditorMouseDragItem)this).DrawDraginGUI(e.mousePosition);
                                e.Use();
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    {
                        if (e.button == 0)
                        {
                            if (draginMe || inRange)
                            {
                                if (null != OnMouseLBUpInSide)
                                    OnMouseLBUpInSide(this);
                                e.Use();
                            }
                            else
                            {
                                //EditorMouseDelegate.Current.EndDrag(e.button);
                                if (null != OnMouseLBUpInOutside)
                                    OnMouseLBUpInOutside(this);
                            }
                        }
                    }
                    break;
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

        public List<Map2DGridUnit.ImageSubData> ImageSubList
        {
            get
            {
                List<Map2DGridUnit.ImageSubData> list = new List<Map2DGridUnit.ImageSubData>();
                foreach(Map2DGridEditorImageSubItem i in mImageSubList)
                {
                    Map2DGridUnit.ImageSubData data;
                    data.type = i.ImageSubType;
                    data.range = i.LocalRange;
                    list.Add(data);
                }
                return list;
            }
        }
        public Map2DGridEditorImageSubItem AddImageSubItem(Map2DGridImageSubType type, Vector2 pos)
        {
            //int count = mImageSubList.Count;
            Map2DGridEditorImageSubItem item = new Map2DGridEditorImageSubItem(type, pos, this);
            item.OnMouseLBUpInOutside += OnClearDraginItem;
            item.OnMouseLBUpInSide += OnItemDraginEnded;
            mImageSubList.Add(item);
            return item;
        }
        public Map2DGridEditorImageSubItem AddImageSubItem(Map2DGridImageSubType type, Rect range)
        {
            Map2DGridEditorImageSubItem item = new Map2DGridEditorImageSubItem(type, range, this);
            item.OnMouseLBUpInOutside += OnClearDraginItem;
            item.OnMouseLBUpInSide += OnItemDraginEnded;
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
                if (item.ItemType != MouseDragItemType.ToolboxItem)
                    continue;
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
                                    AddImageSubItem(toolItem.ImageSubType, e.mousePosition);
                                    dragItem = true;
                                }
                                if (EditorMouseDelegate.Current.DraginType == MouseDragItemType.ToolboxItem)
                                    EditorMouseDelegate.Current.EndDrag(e.button);
                                if (dragItem)
                                {
                                    OnItemDraginEnded(null);
                                    e.Use();
                                }
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
        void OnClearDraginItem(Map2DGridEditorImageSubItem sender)
        {
            Map2DGridEditorForm parent = mParent as Map2DGridEditorForm;
            if (null != parent)
                parent.OnDeSelectImageSubItem(sender);
        }
        void OnItemDraginEnded(Map2DGridEditorImageSubItem sender)
        {
            Map2DGridEditorForm parent = mParent as Map2DGridEditorForm;
            if (null != parent)
                parent.OnImageSubItemSelected(sender);
            Map2DEditor.DoSaved();
        }
    }
}

#endif