using UnityEngine;
using System;
using System.Collections;
using Assets.Script.Tools;

namespace Assets.Script.Scenes.Map2D
{
    [AddComponentMenu("自定义/横版2D/地图/无缝地图")]
    public class Map2DTiled : MonoBehaviour
    {
        public int 总行数 = 1;
        public int 总列数 = 1;
        public bool 按行循环 = false;
        public bool 按列循环 = false;
		public GameObject[] 贴图列表 = null;
        //public string[] 贴图列表 = new string[1];
        void Awake()
        {
            if (总行数 <= 0)
            {
                Debug.LogException(new Exception("无缝地图设置的行数必须大于0！"));
                return;
            }
            if (总列数 <= 0)
            {
                Debug.LogException(new Exception("无缝地图设置的列数必须大于0！"));
                return;
            }
            if (贴图列表.Length != 总行数 * 总列数)
            {
                Debug.LogException(new Exception("无缝地图设置的行列数与总图片数不匹配！"));
                return;
            }
            mGrids = StyleBox9Grid.BuildBox9Grids<Map2DGrid>(总行数, 总列数);
            Map2DGrid.EnumLinkedMapGrids<Map2DGrid>(mGrids, 贴图列表);
        }

        void Start()
        {
            //foreach(Map2DGrid grid in mGrids)
            //{
            //}
        }

        Map2DGrid[,] mGrids = null;
    }

}
