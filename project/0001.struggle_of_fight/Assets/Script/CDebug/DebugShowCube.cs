using UnityEngine;
using System.Collections;

namespace Assets.Script.CDebug
{
    [AddComponentMenu("自定义/Debug/DebugShowCube")]
    public class DebugShowCube : MonoBehaviour
    {
        /*
        public enum AnchorType
        {
            center,
            left,
            top,
            right,
            bottom,
        }

        public AnchorType mAnchorType = AnchorType.bottom;
        */
        public bool mAnchorToBottom = true;
        // Use this for initialization
        void Start()
        {
            mParent = transform.parent.gameObject;
            mDebugCube = transform.gameObject;
            mDebugCube.transform.localRotation = Quaternion.identity;
            Vector3 parentSize = mParent.collider.bounds.size;
            //Vector3 parentScale = mParent.transform.localScale;
            Vector3 thisSize = mDebugCube.collider.bounds.size;
            Vector3 thisScale = mDebugCube.transform.localScale;
            float scaleX = parentSize.x / thisSize.x;
            thisScale.x = scaleX;
            mDebugCube.transform.localScale = thisScale;
            /*
            switch (mAnchorType)
            {
            case AnchorType.left:
                thisPos.x = ((thisSize.x * thisScale.x) - (parentSize.x * parentScale.x)) / 2.0f;
                break;
            case AnchorType.top:
                thisPos.y = ((parentSize.y * parentScale.y) + (thisSize.y * thisScale.y)) / 2.0f;
                break;
            case AnchorType.right:
                thisPos.x = ((parentSize.x * parentScale.x) + (thisSize.x * thisScale.x)) / 2.0f;
                break;
            case AnchorType.bottom:
                thisPos.y = ((parentSize.y * parentScale.y) - (thisSize.y * thisScale.y)) / 2.0f;
                break;
            }
            */
            if (mAnchorToBottom)
            {
                Vector3 parentAxisSize = mDebugCube.transform.rotation * parentSize;
                //Vector3 parentAxisScale = mDebugCube.transform.rotation * parentScale;
                Vector3 axisSize = mDebugCube.transform.rotation * thisSize;
                //Vector3 axisScale = mDebugCube.transform.rotation * thisScale;
                Vector3 upAxis = Vector3.up;
                upAxis.Normalize();
                Vector3 thisPos = mDebugCube.transform.localPosition;
                mDebugCube.transform.localPosition = thisPos + upAxis * (parentAxisSize.y - axisSize.y) / 2.0f;
            }
        }
        GameObject mParent = null;
        GameObject mDebugCube = null;
    }
}