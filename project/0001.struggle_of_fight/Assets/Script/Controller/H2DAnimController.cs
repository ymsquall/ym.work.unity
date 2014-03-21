using UnityEngine;
using System.Collections;


namespace Assets.Script.Controller
{
    using PlayerAnimInstance = IH2DCAnimation<H2DAnimController>;
    public class H2DAnimController
    {
        public H2DAnimController(PlayerAnimInstance instacne)
        {
            for (int i = 0; i < (int)AnimationType.EANT_Max; ++i)
            {
                mAnimClipSpeedList[i] = 1.0f;
                mAnimCrossFadeList[i] = 0.1f;
                mAnimWarpModeList[i] = WrapMode.ClampForever;
            }
        }
        public AnimationType NowAnimType
        {
            set 
            {
                mNowAnimType = value;
                Update();
            }
            get { return mNowAnimType; }
        }
        public bool SetAnimClip(AnimationType type, AnimationClip clip, float speed, float cross, WrapMode mode)
        {
            if (type >= AnimationType.EANT_Max || type < AnimationType.EANT_Idel)
                return false;
            int index = (int)type;
            mAnimClipList[index] = clip;
            mAnimClipSpeedList[index] = speed;
            mAnimCrossFadeList[index] = cross;
            mAnimWarpModeList[index] = mode;
            return true;
        }
        public void Update()
        {
            AnimationClip pClip = mAnimClipList[(int)mNowAnimType];
            if (pClip && mAnimation)
            {
                AnimationState pState = mAnimation[pClip.name];
                if (pState)
                {
                    pState.speed = mAnimClipSpeedList[(int)AnimationType.EANT_Idel];
                    pState.wrapMode = WrapMode.ClampForever;
                    mAnimation.CrossFade(pClip.name, mAnimCrossFadeList[(int)AnimationType.EANT_Idel]);
                }
            }
        }

        PlayerAnimInstance mPlayerInstance;
        Animation mAnimation = null;
        AnimationClip[] mAnimClipList = new AnimationClip[(int)AnimationType.EANT_Max];
        float[] mAnimClipSpeedList = new float[(int)AnimationType.EANT_Max];
        float[] mAnimCrossFadeList = new float[(int)AnimationType.EANT_Max];
        WrapMode[] mAnimWarpModeList = new WrapMode[(int)AnimationType.EANT_Max];
        AnimationType mNowAnimType = AnimationType.EANT_Idel;
    }
}