namespace Assets.Script.Tools
{
    public enum OctreeNodePos : byte
    {
        none,
        top,
        rtop,
        right,
        rbottom,
        bottom,
        lbottom,
        left,
        ltop
    }
    
    public class OctreeNode<T>
    {
    #region Construct&Destructor
        public OctreeNode(T obj, OctreeNode<T> parent)
        {
            mObject = obj;
            mParentNode = parent;
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
    #endregion Readonly Propertys

    #region Methods

    #endregion Methods

    #region Members
        OctreeNodePos mSelfPos = OctreeNodePos.none;
        OctreeNode<T> mParentNode = null;
        OctreeNode<T>[] mChilden = null;
        int mSelfLayer = 0;
        T mObject;
    #endregion Members
    }
}
