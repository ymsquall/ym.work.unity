using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Tools
{
    public class StyleBox9Grid
    {
        public enum Style : byte
        {
            top,
            rtop,
            right,
            rbottom,
            bottom,
            lbottom,
            left,
            ltop,
            max,
        }
        protected StyleBox9Grid() { }
        public StyleBox9Grid this[Style index]  
        {  
            get
            {
                if (index < Style.top || index >= Style.max)
                    return null;
                return mStyles[(int)index];
            }
            protected set
            {
                if (index < Style.top || index >= Style.max)
                    return;
                mStyles[(int)index] = value;
            }
        }
        public StyleBox9Grid this[int index]
        {
            get
            {
                if (index < 0 || index >= (int)Style.max)
                    return null;
                return mStyles[index];
            }
            protected set
            {
                if (index < 0 || index >= (int)Style.max)
                    return;
                mStyles[index] = value;
            }
        }
        public int RowIndex
        {
            get { return mRowIndex; }
            private set { mRowIndex = value; }
        }
        public int ColIndex
        {
            get { return mColIndex; }
            private set { mColIndex = value; }
        }
        public int Index
        {
            get { return mIndex; }
            private set { mIndex = value; }
        }

        public static T[,] BuildBox9Grids<T>(int rowNum, int colNum)
            where T : StyleBox9Grid, new()
        {
            T[,] pRet = new T[rowNum, colNum];
            for (int i = 0; i < rowNum; ++i)
            {
                for (int j = 0; j < colNum; ++j)
                {
                    T pObject = new T();
                    pObject.RowIndex = i;
                    pObject.ColIndex = j;
                    pObject.Index = i * colNum + j;
                    pRet[i, j] = pObject;
                }
            }
            return pRet;
        }

        protected StyleBox9Grid[] mStyles = new StyleBox9Grid[(int)Style.max];
        protected int mRowIndex = 0;
        protected int mColIndex = 0;
        protected int mIndex = 0;
    }
}
