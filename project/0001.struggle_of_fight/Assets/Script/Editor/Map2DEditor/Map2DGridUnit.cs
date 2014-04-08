using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DGridUnit
    {
        int mControlID = -1;
        int mRowIndex = -1;
        int mColIndex = -1;
        Rect mViewRect;
        GameObject mGridUnitObj;
        public Map2DGridUnit()
        {
            Created();
        }
        public void Created()
        {
            mGridUnitObj = new GameObject();
            mGridUnitObj.AddComponent<SpriteRenderer>();
        }
        public void Destroy()
        {
            if (null != mGridUnitObj)
            {
                GameObject.DestroyImmediate(mGridUnitObj);
                GridUnit = null;
            }
        }
        public int ControlID { get { return mControlID; } }
        public int RowIndex { get { return mRowIndex; } }
        public int ColIndex { get { return mColIndex; } }
        public Rect ViewRect { get { return mViewRect; } }
        public Texture2D Image
        {
            set
            {
                SpriteRenderer sr = GridUnit.GetComponent<SpriteRenderer>();
                Texture2D oldTex = null;
                if(sr.sprite != null)
                    oldTex = sr.sprite.texture;
                if(oldTex != value)
                {
                    if (null != value)
                        sr.sprite = Sprite.Create(value, new Rect(0, 0, value.width, value.height), new Vector2(value.width, value.height));
                    else
                        sr.sprite = null;
                }
            }
            get
            {
                SpriteRenderer sr = GridUnit.GetComponent<SpriteRenderer>();
                if (sr.sprite == null)
                    return null;
                return sr.sprite.texture;
            } 
        }
        public GameObject GridUnit
        {
            set { mGridUnitObj = value; }
            get { return mGridUnitObj; }
        }
        public void Init(int id, int rIndex, int cIndex, Rect rc, Texture2D img)
        {
            mControlID = id;
            mRowIndex = rIndex;
            mColIndex = cIndex;
            mViewRect = rc;
            Image = img;
        }
        public void OnEvent(Event e)
        {
            if (!mViewRect.Contains(e.mousePosition))
                return;
            EventType et = e.GetTypeForControl(mControlID);
            switch(et)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                //case EventType.Repaint:
                //case EventType.Layout:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.Ignore:
                case EventType.Used:
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                case EventType.DragExited:
                case EventType.ContextClick:
                    Debug.Log(string.Format("Control[{2}] Event[{0}], CommandName[{1}]", e.type.ToString(), e.commandName, mControlID));
                    e.Use();
                    break;
            }
            switch (et)
            {
                case EventType.MouseDown:
                    {
                        GUIUtility.hotControl = mControlID;
                    }
                    break;
                case EventType.MouseUp:
                    {
                        if (e.button == 1)
                        {
                            var mousePos = e.mousePosition;
                            //EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "Assets/", null);
                            GUIContent[] menuItems = new GUIContent[] { new GUIContent("编辑"), new GUIContent("创建模板") };
                            EditorUtility.DisplayCustomMenu(new Rect(mousePos.x, mousePos.y, 0, 0), menuItems, 1, OnGridUnitMenuItemCommands, null);
                            e.Use();
                        }
                    }
                    break;
            }
        }
        void OnGridUnitMenuItemCommands(object userData, string[] options, int selected)
        {

        }
    }
}