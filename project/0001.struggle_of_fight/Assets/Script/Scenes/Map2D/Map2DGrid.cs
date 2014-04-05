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
            EnumLinkedMapGrids<T>(array, param, false, false);
        }
        public static void EnumLinkedMapGrids<T>(T[,] array, GameObject[] param, bool colLoop, bool rowLoop)
            where T : Map2DGrid
        {
            int rowCount = array.GetLength(0);
            int colCount = array.GetLength(1);
            int lastRowIndex = rowCount - 1;
            //int lastColIndex = colCount - 1;
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
                //
                if (rowLoop)
                {
                    if (pObj.RowIndex == 0)
                        topIndex = lastRowIndex;
                    //else if (pObj.RowIndex >= lastRowIndex)
                    //    bottomIndex = 0;
                }
                //if (colLoop)
                //{
                //    if (pObj.ColIndex == 0)
                //        leftIndex = lastColIndex;
                //    else if (pObj.ColIndex >= lastColIndex)
                //        rightIndex = 0;
                //}
                if (rowLoop)
                {
                    if (pObj.RowIndex == 0)
                    {
                        pObj[Style.top] = array[topIndex, pObj.ColIndex];
                        if (colLoop)
                        {
                            pObj[Style.rtop] = array[topIndex, rightIndex];
                            pObj[Style.ltop] = array[topIndex, leftIndex];
                        }
                    }
                    //else if (pObj.RowIndex >= lastRowIndex)
                    //{
                    //    pObj[Style.bottom] = array[bottomIndex, pObj.ColIndex];
                    //    if (colLoop)
                    //    {
                    //        pObj[Style.rbottom] = array[bottomIndex, rightIndex];
                    //        pObj[Style.lbottom] = array[bottomIndex, leftIndex];
                    //    }
                    //}
                }
                //if (colLoop)
                //{
                //    if (pObj.ColIndex == 0)
                //    {
                //        pObj[Style.left] = array[pObj.RowIndex, leftIndex];
                //        if (rowLoop)
                //        {
                //            pObj[Style.lbottom] = array[bottomIndex, leftIndex];
                //            pObj[Style.ltop] = array[topIndex, leftIndex];
                //        }
                //    }
                //    else if (pObj.ColIndex >= lastColIndex)
                //    {
                //        pObj[Style.right] = array[pObj.RowIndex, rightIndex];
                //        if (rowLoop)
                //        {
                //            pObj[Style.rtop] = array[topIndex, rightIndex];
                //            pObj[Style.rbottom] = array[bottomIndex, rightIndex];
                //        }
                //    }
                //}
            }
        }
        public bool LinkAnotherGrids(Map2D.Map2DGrid[,] anotherGrids, bool t2b, bool l2r)
        {
            int rowCount = anotherGrids.GetLength(0);
            int colCount = anotherGrids.GetLength(1);
            //int lastRowIndex = rowCount - 1;
            //int lastColIndex = colCount - 1;
            if (t2b)
            {
                this[Style.bottom] = anotherGrids[0, ColIndex];
                this[Style.lbottom] = (ColIndex - 1) >= 0 ? anotherGrids[0, ColIndex - 1] : null;
                this[Style.rbottom] = (ColIndex + 1) < colCount ? anotherGrids[0, ColIndex + 1] : null;
            }
            else
            {
                this[Style.top] = anotherGrids[rowCount-1, ColIndex];
                this[Style.ltop] = (ColIndex - 1) >= 0 ? anotherGrids[0, ColIndex - 1] : null;
                this[Style.rtop] = (ColIndex + 1) < colCount ? anotherGrids[0, ColIndex + 1] : null;
            }
            //if (l2r)
            //{
            //    this[Style.right] = anotherGrids[RowIndex, 0];
            //    this[Style.rbottom] = (RowIndex - 1) >= 0 ? anotherGrids[RowIndex - 1, 0] : null;
            //    this[Style.rtop] = (RowIndex + 1) < rowCount ? anotherGrids[RowIndex + 1, 0] : null;
            //}
            //else
            //{
            //    this[Style.left] = anotherGrids[RowIndex, 0];
            //    this[Style.lbottom] = (RowIndex - 1) >= 0 ? anotherGrids[RowIndex - 1, 0] : null;
            //    this[Style.ltop] = (RowIndex + 1) < rowCount ? anotherGrids[RowIndex + 1, 0] : null;
            //}
            return true;
        }
        public GameObject CalcAnotherBlockPosition(StyleBox9Grid.Style s, Bounds bounds, ref Vector3 pos)
        {
            Map2DGrid grid = this[s] as Map2DGrid;
            if(null == grid)
                return null;
            switch(s)
            {
                case StyleBox9Grid.Style.top:
                    pos.y += bounds.size.y;
                    break;
                case StyleBox9Grid.Style.rtop:
                    pos.x += bounds.size.x;
                    pos.y += bounds.size.y;
                    break;
                case StyleBox9Grid.Style.right:
                    pos.x += bounds.size.x;
                    break;
                case StyleBox9Grid.Style.rbottom:
                    pos.x += bounds.size.x;
                    pos.y -= bounds.size.y;
                    break;
                case StyleBox9Grid.Style.bottom:
                    pos.y -= bounds.size.y;
                    break;
                case StyleBox9Grid.Style.lbottom:
                    pos.x -= bounds.size.x;
                    pos.y -= bounds.size.y;
                    break;
                case StyleBox9Grid.Style.left:
                    pos.x -= bounds.size.x;
                    break;
                case StyleBox9Grid.Style.ltop:
                    pos.x -= bounds.size.x;
                    pos.y += bounds.size.y;
                    break;
            }
            return grid.Block;
        }
        GameObject mBlock;
    }
}
