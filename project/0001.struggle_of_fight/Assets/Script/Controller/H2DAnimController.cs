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
            mPlayerInstance = instacne;
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
            if (null == clip || type >= AnimationType.EANT_Max || type < AnimationType.EANT_Idel)
                return false;
            int index = (int)type;
            mAnimClipList[index] = clip;
            mAnimClipSpeedList[index] = speed;
            mAnimCrossFadeList[index] = cross;
            mAnimWarpModeList[index] = mode;
            if (null == mAnimation && null != mPlayerInstance)
                mAnimation = mPlayerInstance.AnimationInst;
            mAnimation.AddClip(clip, clip.name);
            if (null != mAnimation && null == mAnimation.clip)
                mAnimation.clip = clip;
            return true;
        }
        public void Update()
        {
            int index = (int)mNowAnimType;
            AnimationClip pClip = mAnimClipList[index];
            if (pClip && mAnimation)
            {
                AnimationState pState = mAnimation[pClip.name];
                if (pState)
                {
                    pState.speed = mAnimClipSpeedList[index];
                    pState.wrapMode = mAnimWarpModeList[index];
                    mAnimation.CrossFade(pClip.name, mAnimCrossFadeList[index]);
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