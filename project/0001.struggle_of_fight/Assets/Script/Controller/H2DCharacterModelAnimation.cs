using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using CharaAnimSuperT = IH2DCAnimation<H2DAnimController>;
    public class H2DCharacterModelAnimation : MonoBehaviour
    {
        public Object 宿主程序;
        // Use this for initialization
        void Awake()
        {
            //mAnimation = GetComponent<Animation>();
            if (null == 宿主程序)
            {
                Debug.LogError("H2DPlayerModelAnimation中找不到Animation组件！");
                return;
            }
            if (null == 宿主程序)
            {
                Debug.LogError("H2DPlayerModelAnimation的宿主程序设置非法！");
                return;
            }
            Component componet = transform.parent.GetComponent(宿主程序.name);
            if (null == componet)
            {
                Debug.LogError("H2DPlayerModelAnimation的宿主程序设置非法！");
                return;
            }
            mAnimController = componet as CharaAnimSuperT;
            if (null == mAnimController)
            {
                Debug.LogError("H2DPlayerModelAnimation的宿主程序必须继承自IH2DCAnimation<H2DAnimController>！");
                return;
            }
        }
        // 动画帧事件（播放完毕）
        void OnPlayAnimationOvered(AnimationType animType)
        {
            mAnimController.OnAnimOvered(animType);
        }
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
        }
        void OnTriggerEnter(Collider other)
        {

        }
        void OnCollisionEnter(Collision collisionInfo)
        {

        }
        //Animation mAnimation;
        CharaAnimSuperT mAnimController;
    }
}