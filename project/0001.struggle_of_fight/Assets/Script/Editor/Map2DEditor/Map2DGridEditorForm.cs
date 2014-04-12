#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridEditorForm : EditorBaseWindow<Map2DGridEditorForm>
    {
        int mGridRowIndex = -1, mGridColIndex = -1;
        bool mInited = false;
        Map2DGridEditorImageView mImageView;
        Map2DGridEditorPropertyView mPropertyView;
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
        }
        void OnDestroy()
        {
            Map2DEditor.OnFormClosed(this.name);
        }
        public void Init()
        {
            Map2DGridUnit grid = Map2DEditor.FindGridUnitByIndex(GridRowIndex, GridColIndex);
            foreach (Map2DGridUnit.ImageSubData d in grid.ImageSubList)
            {
                mImageView.AddImageSubItem(d.type, d.range);
            }
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
                Rect rc = new Rect(0,0,position.width - 100, position.height - 100);
                mImageView.Init(id, this, rc);
                mImageView.OnGUI();
            }
            GUILayout.Box("", GUILayout.Width(5), GUILayout.Height(position.height - 100));
            id = EditorGUIUtility.GetControlID(FocusType.Passive);
            Map2DGridEditorToolboxView toolbox = EditorGUIUtility.GetStateObject(typeof(Map2DGridEditorToolboxView), id) as Map2DGridEditorToolboxView;
            if (toolbox != null)
            {
                Rect rc = new Rect(position.width - 95, 0, 95, position.height - 100);
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
                Rect rc = new Rect(0, position.height - 95, position.width, 95);
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
    }
}

#endif