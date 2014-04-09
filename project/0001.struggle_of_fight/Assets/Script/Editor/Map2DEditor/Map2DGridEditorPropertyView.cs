#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridEditorPropertyView
    {
        int mControlID;
        EditorWindow mParent;
        Rect mSize;

        public void Init(int id, EditorWindow parent, Rect rc)
        {
            mControlID = id;
            mParent = parent;
            mSize = rc;
        }

        public void OnGUI()
        {
        }
    }
}

#endif