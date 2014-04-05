#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace Assets.Script.Editor.Map2DEditor
{
    public class Map2DEditorForm : EditorBaseWindow<Map2DEditorForm>
    {
        internal SceneView.OnSceneFunc mSceneDelegate;
        float mMapViewScrollRate = 100.0f;
        int mUnitImageWidth = 16;
        int mUnitImageHeight = 16;
        Vector2 mScrollViewPos = Vector2.zero;
        bool mRowCountToggle = false;
        bool mColCountToggle = false;
        Sprite[,] mMapUnitList = new Sprite[1, 1];
        void OnEnable()
        {
        }
        void OnDisable()
        {
        }
        void OnDestroy()
        {
            if (mSceneDelegate != null)
                SceneView.onSceneGUIDelegate -= mSceneDelegate;
        }

        internal void DrawSceneGUI(SceneView view)
        {
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("图片宽度: ");
            mUnitImageWidth = int.Parse(GUILayout.TextField(mUnitImageWidth.ToString(), GUILayout.Width(40)));
            GUILayout.Label("\t图片高度: ");
            mUnitImageHeight = int.Parse(GUILayout.TextField(mUnitImageHeight.ToString(), GUILayout.Width(40)));
            //
            GUILayout.FlexibleSpace();
            int rowCount = mMapUnitList.GetLength(0);
            int colCount = mMapUnitList.GetLength(1);
            GUILayout.Label("单元行数: ");
            int newRowCount = rowCount;
            if (mRowCountToggle)
                GUILayout.TextArea(rowCount.ToString(), GUILayout.Width(30));
            else
                newRowCount = int.Parse(GUILayout.TextField(rowCount.ToString(), GUILayout.Width(30)));
            mRowCountToggle = GUILayout.Toggle(mRowCountToggle, "锁定行");
            GUILayout.Label("\t单元列数: ");
            int newColCount = colCount;
            if (mColCountToggle)
                GUILayout.TextArea(colCount.ToString(), GUILayout.Width(30));
            else
                newColCount = int.Parse(GUILayout.TextField(colCount.ToString(), GUILayout.Width(30)));
            mColCountToggle = GUILayout.Toggle(mColCountToggle, "锁定列");
            //
            GUILayout.FlexibleSpace();
            mMapViewScrollRate = EditorGUILayout.Slider(mMapViewScrollRate, 3, 1000.0f);
            GUILayout.EndHorizontal();
            mScrollViewPos = GUILayout.BeginScrollView(mScrollViewPos); // begin ver scroll bar
            GUILayout.BeginVertical();
            if (rowCount != newRowCount || colCount != newColCount)
            {
                var oldUnits = mMapUnitList.Clone() as Sprite[,];
                mMapUnitList = new Sprite[newRowCount, newColCount];
                for (int i = 0; i < newRowCount; ++i)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < newColCount; ++j)
                    {
                        if (i < rowCount && j < colCount)
                            mMapUnitList[i, j] = oldUnits[i, j];
                        Texture2D tex = mMapUnitList[i, j] != null ? mMapUnitList[i, j].texture : null;
                        Texture2D newTex = EditorGUILayout.ObjectField(tex, typeof(Texture2D),
                            GUILayout.Width((float)mUnitImageWidth * (mMapViewScrollRate / 100.0f)),
                            GUILayout.Height((float)mUnitImageHeight * (mMapViewScrollRate / 100.0f))) as Texture2D;
                        if (newTex != tex)
                        {
                            if (null != mMapUnitList[i, j])
                                GameObject.DestroyObject(mMapUnitList[i, j]);
                            if (null != newTex)
                                mMapUnitList[i, j] = Sprite.Create(newTex,
                                    new Rect(0, 0, newTex.width, newTex.height), new Vector2(newTex.width, newTex.height));
                            else
                                mMapUnitList[i, j] = null;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                for (int i = 0; i < newRowCount; ++i)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < newColCount; ++j)
                    {
                        Texture2D tex = mMapUnitList[i, j] != null ? mMapUnitList[i, j].texture : null;
                        Texture2D newTex = EditorGUILayout.ObjectField(tex, typeof(Texture2D),
                            GUILayout.Width((float)mUnitImageWidth * (mMapViewScrollRate / 100.0f)),
                            GUILayout.Height((float)mUnitImageHeight * (mMapViewScrollRate / 100.0f))) as Texture2D;
                        if (newTex != tex)
                        {
                            if(null != mMapUnitList[i, j])
                                GameObject.DestroyObject(mMapUnitList[i, j]);
                            if (null != newTex)
                                mMapUnitList[i, j] = Sprite.Create(newTex,
                                    new Rect(0, 0, newTex.width, newTex.height), new Vector2(newTex.width, newTex.height));
                            else
                                mMapUnitList[i, j] = null;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            //GUILayout.Label("单元列表: \t");
            //for (int i = 0; i < newRowCount; ++i)
            //{
            //    for (int j = 0; j < newColCount; ++j)
            //    {
            //        GameObject mapUnit = null;
            //        mapUnit = EditorGUILayout.ObjectField(mapUnit, typeof(GameObject)) as GameObject;
            //    }
            //}
            //GUILayout.Box("fdsfsf");
            //GUILayout.EndArea();
            //GUILayout.VerticalScrollbar(0, 100, 0, 100);
            //GUILayout.HorizontalScrollbar(0, 100, 0, 100);
            //mMainCamera.Render();
        }

        void OnInspectorGUI()
        {
            Debug.Log("OnInspectorGUI");
        }  
    }
}

#endif