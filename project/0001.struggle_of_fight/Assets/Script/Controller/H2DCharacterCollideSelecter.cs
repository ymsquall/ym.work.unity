using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public class H2DCharacterCollideSelecter : MonoBehaviour
    {
        public Object 宿主程序;
        public H2DCCollideSelecterType 碰撞标记;
        void Awake()
        {
            if (null == 宿主程序)
            {
                Debug.LogError("H2DCreatureCollideSelecter的宿主程序设置非法！");
                return;
            }
            Component componet = transform.parent.GetComponent(宿主程序.name);
            if (null == componet)
            {
                Debug.LogError("H2DCreatureCollideSelecter的宿主程序设置非法！");
                return;
            }
            mSelecter = componet as IH2DCharacterCollideSelecter;
            if (null == mSelecter)
            {
                Debug.LogError("H2DCreatureCollideSelecter的宿主程序必须继承自IH2DCreatureCollideSelecter！");
                return;
            }
        }
        void OnCollisionEnter(Collision collisionInfo)
        {
            mSelecter.OnH2DCCollisionEnter(collisionInfo, 碰撞标记);
        }
        void OnCollisionExit(Collision collisionInfo)
        {
            mSelecter.OnH2DCCollisionExit(collisionInfo, 碰撞标记);
        }
        void OnCollisionStay(Collision collisionInfo)
        {
            mSelecter.OnH2DCCollisionStay(collisionInfo, 碰撞标记);
        }
        IH2DCharacterCollideSelecter mSelecter = null;
    }
}