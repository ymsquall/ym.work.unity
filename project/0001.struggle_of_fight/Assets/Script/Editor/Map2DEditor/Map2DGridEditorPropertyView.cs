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
                Texture2D image = EditorGUILayout.ObjectField(Selected.Image, typeof(Texture2D), true,
                    GUILayout.Width(50), GUILayout.Height(50)) as Texture2D;
                if (image != Selected.Image)
                {
                    Selected.Image = image;
                    Rect range = mSelected.LocalRange;
                    range.width = Selected.Image.width;
                    range.height = Selected.Image.height;
                    mSelected.LocalRange = range;
                }
                Rect newRange = EditorGUILayout.RectField("范围:", Selected.LocalRange, GUILayout.Width(140));
                if (newRange != Selected.LocalRange)
                {
                    if (null == Selected.Image)
                        mSelected.LocalRange = newRange;
                    else
                    {
                        newRange.width = Selected.Image.width;
                        newRange.height = Selected.Image.height;
                        mSelected.LocalRange = newRange;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(GUILayout.Width(100));
                Map2DGridImageSubType newType = (Map2DGridImageSubType)EditorGUILayout.EnumPopup("类型:", Selected.ImageSubType);
                if (newType != Selected.ImageSubType)
                    mSelected.ImageSubType = newType;
                if (Selected.Image != null)
                {
                    Rect newCollidere = EditorGUILayout.RectField("碰撞范围:", Selected.Collider, GUILayout.Width(140));
                    if (newCollidere != Selected.Collider)
                        Selected.Collider = newCollidere;
                }
                //GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif