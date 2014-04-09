#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridEditorForm : EditorBaseWindow<Map2DGridEditorForm>
    {
        int mGridRowIndex = -1, mGridColIndex = -1;
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
        void OnEnable()
        {
            wantsMouseMove = true;
        }
        void OnDestroy()
        {
            Map2DEditor.OnFormClosed(this.name);
        }
        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            int id = EditorGUIUtility.GetControlID(FocusType.Passive);
            Map2DGridEditorImageView gridImage = EditorGUIUtility.GetStateObject(typeof(Map2DGridEditorImageView), id) as Map2DGridEditorImageView;
            if (gridImage != null)
            {
                Rect rc = new Rect(0,0,position.width - 100, position.height - 100);
                gridImage.Init(id, this, rc);
                gridImage.OnGUI();
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
            Map2DGridEditorPropertyView property = EditorGUIUtility.GetStateObject(typeof(Map2DGridEditorPropertyView), id) as Map2DGridEditorPropertyView;
            if (property != null)
            {
                Rect rc = new Rect(0, position.height - 95, position.width, 95);
                property.Init(id, this, rc);
                property.OnGUI();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}

#endif