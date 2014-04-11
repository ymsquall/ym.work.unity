#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    class Map2DGridEditorToolboxItem : IEditorMouseDragItem
    {
        int mControlID = -1;
        Rect mRange = new Rect(0, 0, 0, 0);
        string mToolTips;
        bool mSelected = false;

#region IEditorMouseDragItem
        MouseDragItemType IEditorMouseDragItem.ItemType
        {
            get { return MouseDragItemType.ToolboxItem; }
        }
        void IEditorMouseDragItem.OnBeginDragin(Vector2 pos)
        {

        }
        void IEditorMouseDragItem.DrawDraginGUI(Vector2 pos)
        {
            if (Map2DGridEditorToolboxView.ToolsTips[0] == mToolTips)
                NGUIEditorTools.DrawTexture(NGUIEditorTools.blankTexture, new Rect(pos.x - 50, pos.y - 5, 100, 10), new Rect(0, 0, 1, 1), Color.gray);
            else if (Map2DGridEditorToolboxView.ToolsTips[1] == mToolTips)
                NGUIEditorTools.DrawTexture(NGUIEditorTools.blankTexture, new Rect(pos.x - 10, pos.y - 50, 20, 100), new Rect(0, 0, 1, 1), Color.gray);
            else if (Map2DGridEditorToolboxView.ToolsTips[2] == mToolTips)
                NGUIEditorTools.DrawTexture(NGUIEditorTools.blankTexture, new Rect(pos.x - 30, pos.y - 30, 60, 60), new Rect(0, 0, 1, 1), Color.gray);
        }
#endregion

        public int ControlID { get { return mControlID; } }
        public string ToolTips { get { return mToolTips; } }
        public Rect Range { get { return mRange; } }
        public bool Selected { set{ mSelected = value; } get { return mSelected; } }

        public void Init(int id, string tips, Rect rc)
        {
            mControlID = id;
            mToolTips = tips;
            mRange = rc;
        }
        public void OnEvent(Event e)
        {
            if (e.type == EventType.Layout || e.type == EventType.Repaint)
                return;
            if (!mRange.Contains(e.mousePosition))
                return;
            EventType et = e.GetTypeForControl(ControlID);
            Debug.Log("Map2DGridEditorToolboxItem.OnEvent=" + et.ToString());
            switch (et)
            {
                case EventType.MouseDrag:
                    EditorMouseDelegate.Current.EndDrag(e.button);
                    EditorMouseDelegate.Current.BeginDrag(e, this);
                    break;
            }
        }
    }
    public class Map2DGridEditorToolboxView
    {
        int mControlID;
        EditorWindow mParent;
        Rect mSize;

        public static string[] ToolsTips = { "平台", "墙壁", "刷怪点" };
        Dictionary<int, Map2DGridEditorToolboxItem> mToolItems = new Dictionary<int, Map2DGridEditorToolboxItem>(0);

        public void Init(int id, EditorWindow parent, Rect rc)
        {
            mControlID = id;
            mParent = parent;
            mSize = rc;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            Rect itemRect = new Rect(10, 5, 0, 0);
            foreach (string s in ToolsTips)
            {
                itemRect.x = mSize.x;
                itemRect.width = 50;
                itemRect.height = 30;
                int id = EditorGUIUtility.GetControlID(FocusType.Passive);
                Map2DGridEditorToolboxItem tool = EditorGUIUtility.GetStateObject(typeof(Map2DGridEditorToolboxItem), id) as Map2DGridEditorToolboxItem;
                tool.Init(id, s, itemRect);
                if (mToolItems.ContainsKey(id))
                    mToolItems[id] = tool;
                else
                    mToolItems.Add(id, tool);
                //GUILayout.Box("", GUILayout.Width(itemRect.width), GUILayout.Height(itemRect.height));
                {
                    GUIStyle cc = new GUIStyle();
                    cc.normal.background = null;
                    cc.normal.textColor = Map2DEditor.ColorByToolType(s);
                    cc.fontSize = 25;
                    GUI.Label(itemRect, s, cc);
                }
                tool.OnEvent(Event.current);
                itemRect.y += itemRect.height + 5;
            }
            EditorGUILayout.EndVertical();
        }

        void OnEvent()
        {

        }
    }
}

#endif