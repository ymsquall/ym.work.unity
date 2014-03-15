namespace Assets.Script.Tools
{
    public enum OctreeNodePos : byte
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
    
    public class OctreeNode<T>
    {
    #region Construct&Destructor
        public OctreeNode(T obj, OctreeNode<T> parent)
        {
            mObject = obj;
            mParentNode = parent;
            mChilden = new OctreeNode<T>[(int)OctreeNodePos.max];
        }
    #endregion Construct&Destructor

    #region Readonly Propertys
        public OctreeNodePos NodePos
        {
            get {return mSelfPos; }
        }
        public OctreeNode<T> Parent
        {
            get { return mParentNode; }
        }
        public int NodeLayer
        {
            get { return mSelfLayer; }
        }
        public T Object
        {
            get { return mObject; }
        }
        public OctreeNode<T> this[OctreeNodePos index]
        {
            get
            {
                if (index < OctreeNodePos.top || index >= OctreeNodePos.max)
                    return null;
                return mChilden[(int)index];
            }
        }
        public OctreeNode<T> this[int index]
        {
            get
            {
                if (index < 0 || index >= (int)OctreeNodePos.max)
                    return null;
                return mChilden[index];
            }
        }  
    #endregion Readonly Propertys

    #region Methods

    #endregion Methods

    #region Members
        OctreeNodePos mSelfPos = OctreeNodePos.max;
        OctreeNode<T> mParentNode = null;
        OctreeNode<T>[] mChilden = null;
        int mSelfLayer = 0;
        T mObject;
    #endregion Members
    }
}
