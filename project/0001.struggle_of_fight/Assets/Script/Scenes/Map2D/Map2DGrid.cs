using UnityEngine;
using Assets.Script.Tools;

namespace Assets.Script.Scenes.Map2D
{
    public class Map2DGrid : StyleBox9Grid
    {
        public Map2DGrid() { }
        public GameObject Block
        {
            get { return mBlock; }
            private set { mBlock = value; }
        }
        public static void EnumLinkedMapGrids<T>(T[,] array, GameObject[] param)
            where T : Map2DGrid
        {
            int rowCount = array.GetLength(0);
            int colCount = array.GetLength(1);
            foreach (T pObj in array)
            {
                int topIndex = pObj.RowIndex - 1;
                int bottomIndex = pObj.RowIndex + 1;
                int leftIndex = pObj.ColIndex - 1;
                int rightIndex = pObj.ColIndex + 1;
                pObj[Style.top] = topIndex >= 0 ? array[topIndex, pObj.ColIndex] : null;
                pObj[Style.rtop] = (topIndex >= 0 && rightIndex < colCount) ? array[topIndex, rightIndex] : null;
				pObj[Style.right] = rightIndex < colCount ? array[pObj.RowIndex, rightIndex] : null;
                pObj[Style.rbottom] = (bottomIndex < rowCount && rightIndex < colCount) ? array[bottomIndex, rightIndex] : null;
                pObj[Style.bottom] = bottomIndex < rowCount ? array[bottomIndex, pObj.ColIndex] : null;
                pObj[Style.lbottom] = (bottomIndex < rowCount && leftIndex >= 0) ? array[bottomIndex, leftIndex] : null;
                pObj[Style.left] = leftIndex >= 0 ? array[pObj.RowIndex, leftIndex] : null;
                pObj[Style.ltop] = (topIndex >= 0 && leftIndex >= 0) ? array[topIndex, leftIndex] : null;
                pObj.Block = param[pObj.Index];
				/*
				string probName = param[pObj.Index].name;
				var tmpObj = Resources.Load(probName);
				GameObject obj = tmpObj as GameObject;
				pObj.Block = obj as Map2DBlock;
				*/
            }
        }
        GameObject mBlock;
    }
}
