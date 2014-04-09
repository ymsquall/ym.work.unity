#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridEditorImageView
    {
        EditorWindow mParent;
        float mImageScrollScale = 1.0f;
        Vector2 mImageScrollPos = Vector2.zero;
        Rect mSize = new Rect(0, 0, 0, 0);
        Rect mImageRealViewSize = new Rect(0, 0, 0, 0);
        int mControlID = -1;

        struct ColliderObject
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
        public void Init(int controlID, EditorWindow parent, Rect rc)
        {
            mControlID = controlID;
            mParent = parent;
            mSize = rc;
        }
        static Texture g_sDraginTexture = Texture2D.CreateExternalTexture(100, 100, TextureFormat.Alpha8, false, false, IntPtr.Zero);
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
            EditorGUILayout.BeginScrollView(mImageScrollPos, GUILayout.Width(mSize.width), GUILayout.Height(mSize.height));
            mImageRealViewSize.width = image.width * mImageScrollScale;
            mImageRealViewSize.height = image.height * mImageScrollScale;
            GUILayout.BeginArea(mSize);
            Rect newRect = mImageRealViewSize;
            newRect.x = mImageScrollPos.x;
            newRect.y = mImageScrollPos.y;
            GUI.DrawTexture(newRect, image, ScaleMode.ScaleToFit);
            OnEvent(Event.current);
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
            if (!mSize.Contains(e.mousePosition))
            {
                if (EditorMouseDelegate.Current.Dragin(MouseButtonType.middel))
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
                                if (EditorMouseDelegate.Current.DraginCount > 0)
                                    e.Use();
                            }
                        }
                        break;
                }
            }
        }
    }
}

#endif