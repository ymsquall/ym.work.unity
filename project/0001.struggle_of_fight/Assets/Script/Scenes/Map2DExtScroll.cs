using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Tools;
using Assets.Script.Controller;

namespace Assets.Script.Scenes
{
    public class Map2DExtScroll : MonoBehaviour
    {
        public bool 横向卷动地图 = true;
        public bool 纵向卷动地图 = false;
        public bool 左到右地图卷动 = true;
        public bool 上到下地图卷动 = true;
        public int 横向卷动次数 = 2;
        public int 纵向卷动次数 = 2;
        public H2DPlayerController 主角 = null;
        // Use this for initialization
        void Awake()
        {
            string[] startedList = { "01", "02", "03", "04" };
            string[] endedList = { "09", "10", "11", "12" };
            string[] scrollList = { "05", "06", "07", "08" };
            string sceneName = Application.loadedLevelName;
            string sceneResPath = string.Format("Assets/Scenes/{0}/resources/MapUnits", sceneName);
            string[] fileList = FileUtils.EnumAllFilesByPath(sceneResPath, true);
            List<GameObject> startMapList = new List<GameObject>(0);
            List<GameObject> endedMapList = new List<GameObject>(0);
            List<GameObject> scrollMapList = new List<GameObject>(0);
            Vector3 pos = Vector3.zero;
            bool first = false;
            foreach (string f in fileList)
            {
                string name = f.Substring(f.LastIndexOf("\\")+1);
                name = name.Substring(0, name.LastIndexOf("."));
                GameObject obj = Resources.Load<GameObject>("MapUnits/" + name);
                foreach(string s in startedList)
                {
                    if(obj.name == s)
                    {
                        if(!first)
                        {
                            GameObject objClean = MonoBehaviour.Instantiate(obj) as GameObject;
                            objClean.name = obj.name;
                            objClean.transform.parent = transform;
                            objClean.transform.localPosition = pos;
                            objClean.layer = transform.gameObject.layer;
                            SpriteRenderer sprite = objClean.renderer as SpriteRenderer;
                            pos.y += sprite.bounds.size.y;
                            first = true;
                        }
                        startMapList.Add(obj);
                    }
                }
                foreach (string s in endedList)
                {
                    if (obj.name == s)
                        endedMapList.Add(obj);
                }
                foreach (string s in scrollList)
                {
                    if (obj.name == s)
                        scrollMapList.Add(obj);
                }
            }
            if (!上到下地图卷动 || !左到右地图卷动)
            {
                startMapList.Reverse();
                endedMapList.Reverse();
                scrollMapList.Reverse();
            }
            mSceneStartedMapList = StyleBox9Grid.BuildBox9Grids<Map2D.Map2DGrid>(4, 1);
            Map2D.Map2DGrid.EnumLinkedMapGrids<Map2D.Map2DGrid>(mSceneStartedMapList, startMapList.ToArray());
            mSceneEndedMapList = StyleBox9Grid.BuildBox9Grids<Map2D.Map2DGrid>(4, 1);
            Map2D.Map2DGrid.EnumLinkedMapGrids<Map2D.Map2DGrid>(mSceneEndedMapList, endedMapList.ToArray());
            mSceneScrollGroupMapList = StyleBox9Grid.BuildBox9Grids<Map2D.Map2DGrid>(4, 1);
            Map2D.Map2DGrid.EnumLinkedMapGrids<Map2D.Map2DGrid>(mSceneScrollGroupMapList, scrollMapList.ToArray(),
                横向卷动地图 && 横向卷动次数 > 0, 纵向卷动地图 && 纵向卷动次数 > 0);
        }
        void Update()
        {
            Vector3 playerPos = 主角.transform.position;
            int mapCount = transform.childCount;
            for(int i = 0; i < mapCount; ++ i)
            {
                GameObject obj = transform.GetChild(i).gameObject;
                if(mLastInMapUnit != obj)
                {
                    SpriteRenderer sprite = obj.renderer as SpriteRenderer;
                    playerPos.z = sprite.bounds.min.z + (sprite.bounds.max.z - sprite.bounds.min.z) / 2.0f;
                    if (sprite.bounds.Contains(playerPos))
                    {
                        string name = sprite.name;
                        foreach (Map2D.Map2DGrid g in mSceneStartedMapList)
                        {
                            if (g.Block.name == name)
                            {
                                LoadMapAreaByGrid(g, obj, sprite.bounds, false);
                                mLastInMapUnit = obj;
                                return;
                            }
                        }
                        foreach (Map2D.Map2DGrid g in mSceneEndedMapList)
                        {
                            if (g.Block.name == name)
                            {
                                LoadMapAreaByGrid(g, obj, sprite.bounds, false);
                                mLastInMapUnit = obj;
                                return;
                            }
                        }
                        foreach (Map2D.Map2DGrid g in mSceneScrollGroupMapList)
                        {
                            string blockName = string.Format("{0}({1})", g.Block.name, mGroupColScrolledCount);
                            if (blockName == name)
                            {
                                LoadMapAreaByGrid(g, obj, sprite.bounds, true);
                                mLastInMapUnit = obj;
                                return;
                            }
                        }
                        break;
                    }
                }
            }
        }
        bool LoadMapAreaByGrid(Map2D.Map2DGrid grid, GameObject inSceneObj, Bounds bounds, bool inLoop)
        {
            bool inGroupLastMap = GridInGroupLastMap(grid);
            if (inGroupLastMap)
            {
                if (!inLoop)
                    grid.LinkAnotherGrids(mSceneScrollGroupMapList, 上到下地图卷动, 左到右地图卷动);
                else
                {
                    if (横向卷动地图)
                    {
                        if (mGroupRowScrolledCount < 横向卷动次数)
                            mGroupRowScrolledCount++;
                        else
                            grid.LinkAnotherGrids(mSceneEndedMapList, 上到下地图卷动, 左到右地图卷动);
                    }
                    if (纵向卷动地图)
                    {
                        if (mGroupColScrolledCount < 纵向卷动次数)
                            mGroupColScrolledCount++;
                        else
                            grid.LinkAnotherGrids(mSceneEndedMapList, 上到下地图卷动, 左到右地图卷动);
                    }
                }
            }
            for (StyleBox9Grid.Style i = StyleBox9Grid.Style.top; i < StyleBox9Grid.Style.max; ++i)
            {
                Vector3 anotherPos = Vector3.zero;
                GameObject anotherObj = grid.CalcAnotherBlockPosition(i, bounds, ref anotherPos);
                if (null != anotherObj)
                {
                    inLoop = false;
                    string anotherName = anotherObj.name;
                    foreach (Map2D.Map2DGrid g in mSceneScrollGroupMapList)
                    {
                        if (anotherObj.name == g.Block.name)
                        {
                            inLoop = true;
                            break;
                        }
                    }
                    if (inLoop)
                    {
                        anotherName = string.Format("{0}({1})", anotherName, mGroupColScrolledCount);
                    }
                    anotherPos += inSceneObj.transform.localPosition;
                    int mapCount = transform.childCount;
                    bool alreadyCreatedMap = false;
                    for (int m = 0; m < mapCount; ++m)
                    {
                        GameObject obj = transform.GetChild(m).gameObject;
                        if (obj.name == anotherName)
                        {
                            alreadyCreatedMap = true;
                            obj.transform.position = anotherPos;
                            break;
                        }
                    }
                    if (alreadyCreatedMap)
                        continue;
                    GameObject objClone = MonoBehaviour.Instantiate(anotherObj) as GameObject;
                    objClone.name = anotherName;
                    objClone.transform.parent = transform;
                    objClone.transform.localPosition = anotherPos;
                    objClone.layer = transform.gameObject.layer;
                }
            }
            return true;
        }
        bool GridInGroupLastMap(Map2D.Map2DGrid grid)
        {
            if (!左到右地图卷动 || !上到下地图卷动)
            {
                if (mSceneStartedMapList[0, 0] == grid)
                    return true;
                if (mSceneScrollGroupMapList[0, 0] == grid)
                    return true;
                if (mSceneEndedMapList[0, 0] == grid)
                    return true;
            }
            else
            {
                int rowCount = mSceneStartedMapList.GetLength(0);
                int colCount = mSceneStartedMapList.GetLength(1);
                if (mSceneStartedMapList[rowCount - 1, colCount - 1] == grid)
                    return true;
                rowCount = mSceneScrollGroupMapList.GetLength(0);
                colCount = mSceneScrollGroupMapList.GetLength(1);
                if (mSceneScrollGroupMapList[rowCount - 1, colCount - 1] == grid)
                    return true;
                rowCount = mSceneEndedMapList.GetLength(0);
                colCount = mSceneEndedMapList.GetLength(1);
                if (mSceneEndedMapList[rowCount - 1, colCount - 1] == grid)
                    return true;
            }
            return false;
        }
        GameObject mLastInMapUnit = null;
        Map2D.Map2DGrid[,] mSceneStartedMapList = null;
        Map2D.Map2DGrid[,] mSceneEndedMapList = null;
        Map2D.Map2DGrid[,] mSceneScrollGroupMapList = null;
        int mGroupRowScrolledCount = 0;
        int mGroupColScrolledCount = 0;
    }
}

