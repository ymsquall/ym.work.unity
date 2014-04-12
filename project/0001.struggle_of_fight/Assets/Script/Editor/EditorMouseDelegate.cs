#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor
{
    public enum MouseButtonType : int
    {
        left = 0,
        right = 1,
        middel = 2,
        max
    }

    public enum MouseDragItemType : sbyte
    {
        none = 0,
        ToolboxItem = 1,
        ImageSubItem = 2,
    }
    public interface IEditorMouseDragItem
    {
        MouseDragItemType ItemType { get; }
        void OnBeginDragin(Vector2 pos);
        void DrawDraginGUI(Vector2 pos);
    }

    public class EditorMouseDelegate : ScriptableObject
    {
        EditorMouseDelegate()
        {
            mCurrent = this;
        }
        static EditorMouseDelegate mCurrent = null;
        bool[] mMouseButtonPressed = new bool[(int)MouseButtonType.max] { false, false, false };
        Vector2 mMouseBeginDragPosition = Vector2.zero;
        List<IEditorMouseDragItem> mMouseDraginObjects = new List<IEditorMouseDragItem>(0);

        public static EditorMouseDelegate Current { get { return mCurrent; } }
        public Vector2 BeginPos { get { return mMouseBeginDragPosition; } }
        public bool InDrag(MouseButtonType buttonID)
        {
            if (buttonID < MouseButtonType.left || buttonID >= MouseButtonType.max)
                return false;
            return mMouseButtonPressed[(int)buttonID];
        }
        public int DraginCount
        {
            get { return mMouseDraginObjects.Count; }
        }
        public MouseDragItemType DraginType
        {
            get 
            {
                if (DraginCount <= 0)
                    return MouseDragItemType.none;
                return mMouseDraginObjects[0].ItemType;
            }
        }
        public IEditorMouseDragItem this[int index]
        {
            get
            {
                if (null == mMouseDraginObjects || index < 0 || index >= mMouseDraginObjects.Count)
                    return null;
                return mMouseDraginObjects[index];
            }
        }
        public bool DraginItem(IEditorMouseDragItem item)
        {
            return mMouseDraginObjects.Contains(item);
        }

        public void BeginDrag(Event e, params IEditorMouseDragItem[] objs)
        {
            mMouseButtonPressed[e.button] = true;
            mMouseBeginDragPosition = e.mousePosition;
            if (objs == null)
            {

            }
            else if (mMouseDraginObjects.Count == 0)
            {
                foreach (IEditorMouseDragItem o in objs)
                {
                    o.OnBeginDragin(e.mousePosition);
                    mMouseDraginObjects.Add(o);
                }
            }
        }
        public void EndDrag(int id)
        {
            mMouseButtonPressed[id] = false;
            mMouseBeginDragPosition = Vector2.zero;
            mMouseDraginObjects.Clear();
        }
        void Update()
        {
            for(int id = (int)MouseButtonType.left; id < (int)MouseButtonType.max; ++ id)
            {
                if(mMouseButtonPressed[id])
                {
                    if (!Input.GetMouseButtonDown(id))
                        EndDrag(id);
                }
            }
        }
    }
}

#endif