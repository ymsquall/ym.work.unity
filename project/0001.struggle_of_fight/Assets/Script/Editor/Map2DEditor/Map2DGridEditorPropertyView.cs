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
        Rect mRange;

        Map2DGridEditorImageSubItem mSelected = null;
        public Map2DGridEditorImageSubItem Selected
        {
            set { mSelected = value; }
            get { return mSelected; }
        }

        public void Init(int id, EditorWindow parent, Rect rc)
        {
            mControlID = id;
            mParent = parent;
            mRange = rc;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if(null != Selected)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(150));
                Rect newRange = EditorGUILayout.RectField("范围:", Selected.LocalRange, GUILayout.Width(140));
                if (newRange != Selected.LocalRange)
                    mSelected.LocalRange = newRange;
                Map2DGridImageSubType newType = (Map2DGridImageSubType)EditorGUILayout.EnumPopup("类型:", Selected.ImageSubType);
                if (newType != Selected.ImageSubType)
                    mSelected.ImageSubType = newType;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif