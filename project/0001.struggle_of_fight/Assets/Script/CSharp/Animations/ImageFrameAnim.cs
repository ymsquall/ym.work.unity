using UnityEngine;
using System.Collections;

[AddComponentMenu("自定义/动画/序列帧动画")]
public class ImageFrameAnim : MonoBehaviour
{
    public Sprite[] mImages = null;
    public float mMaxTime = 1.0f;
    public bool mLoop = false;
    public int mTimes = 1;
    public float mOnceLoopDelayTime = 1.0f;
    public int mStartFrameIndex = 0;
    public bool mPaused = false;
    public bool mFlipY = false;
    public float mDepthWithParent = 0.2f;
    public Vector2 mPlanOffset = Vector2.zero;

    public delegate bool EventHandler(Object sender, int totalTimes);
    public event EventHandler OnEndedOfOnce;
    public event EventHandler OnEndedOfOnceLoop;
    public event EventHandler OnEndedOfAll;

    public int InOnceLoopTimes
    {
        get { return mOnceLoopTimes; }
    }
    public int LoopTimes
    {
        get { return mTotalTimes / mTimes; }
    }
    public bool Stoped
    {
        set 
        {
            if (value)
            {
                mOnceLoopTimes = 0;
                mAnimCurrentTime = 0.0f;
                mFrameIndex = 0;
                mOnceLoopDelayTimer = 0.0f;
                mRenderer.sprite = null;
            }
            else
                mRenderer.sprite = mImages[mStartFrameIndex];
            mStoped = value;
        }
        get { return mStoped; }
    }
    public void Play()
    {
        Stoped = true;
        mPaused = false;
        Stoped = false;
        mEnded = false;
    }
    protected virtual bool Init()
    {
        bool ret = true;
        mRenderer = GetComponent<SpriteRenderer>();
        if (mRenderer == null)
        {
            Debug.LogError("ImageFrameAnim组件必须依附于SpriteRenderer组件！");
            ret = false;
        }
        if (mImages == null || mImages.Length <= 1)
        {
            Debug.LogError("ImageFrameAnim组件必须设置大于1张图片！");
            ret = false;
        }
        if (mStartFrameIndex >= mImages.Length)
        {
            Debug.LogError("ImageFrameAnim组件起始帧索引不能大于帧数量！");
            ret = false;
        }
        if (mLoop && mTimes == 0)
            Debug.Log("播放次数设为0是就不需要开启循环标志，循环标志只负责控制mTimes次结束后再次循环！");
        mParent = transform.parent.gameObject;
        if (mPaused)
            mRenderer.sprite = null;
        else
            mRenderer.sprite = mImages[mStartFrameIndex];
        if (!mFlipY)
            mDepthWithParent = -mDepthWithParent;
        mStartPosition = transform.localPosition;
        mStartRotation = transform.rotation;
        return ret;
    }
	void Awake ()
    {
        Init();
	}
    void OnOnceEnded()
    {
        mOnceLoopTimes++;
        if (mLoop)
        {
            if (OnEndedOfOnce != null)
                OnEndedOfOnce(this, mTotalTimes);
            if (mOnceLoopTimes >= mTimes)
            {
                if (OnEndedOfOnceLoop != null)
                    OnEndedOfOnceLoop(this, mTotalTimes);
                mOnceLoopTimes = 0;
            }
        }
        else
        {
            if (OnEndedOfOnceLoop != null)
                OnEndedOfOnceLoop(this, mTotalTimes);
            if (mTimes != 0 && mOnceLoopTimes >= mTimes)
            {
                if (OnEndedOfAll != null)
                    OnEndedOfAll(this, mTotalTimes);
                mOnceLoopTimes = 0;
                mEnded = true;
            }
        }
        mAnimCurrentTime = 0.0f;
        mFrameIndex = 0;
        mOnceLoopDelayTimer = mOnceLoopDelayTime;
        if (mOnceLoopDelayTimer <= 0.0f && mTimes > 0)
            mRenderer.sprite = null;
    }
    protected virtual void LockPosAndRotaForParent()
    {
        mFinalPosition = mStartPosition;
        if (mParent != null)
        {
            if (mParent.transform.rotation.y > 0.0f)
            {
                mFinalRotation = Quaternion.Euler(0, mFlipY ? 180 : 0, 0) * mStartRotation;
                mFinalPosition.x = mFlipY ? mStartPosition.x + mDepthWithParent : mStartPosition.x - mDepthWithParent;
            }
            else
            {
                mFinalRotation = Quaternion.Euler(0, mFlipY ? 0 : 180, 0) * mStartRotation;
                mFinalPosition.x = mFlipY ? mStartPosition.x - mDepthWithParent : mStartPosition.x + mDepthWithParent;
            }
        }
        else
        {
            mFinalRotation = mStartRotation;
        }
        mFinalPosition.y = mStartPosition.y + mPlanOffset.y;
        mFinalPosition.z = mStartPosition.z + mPlanOffset.x;
    }
    protected virtual void CustonMove()
    {
    }
    protected virtual void CustonRotation()
    {
    }
    void ApplyTransform()
    {
        transform.localPosition = mFinalPosition;
        transform.rotation = mFinalRotation;
    }
    protected bool UpdateTransform()
    {
        if (mEnded || mPaused || mStoped)
            return false;
        if (mOnceLoopDelayTimer > 0.0f)
        {
            mOnceLoopDelayTimer -= Time.deltaTime;
            return false;
        }
        LockPosAndRotaForParent();
        CustonMove();
        CustonRotation();
        ApplyTransform();
        return true;
    }
    protected virtual bool UpdateFrame()
    {
        mFrameIndex = (int)(mAnimCurrentTime / mMaxTime * (float)mImages.Length);
        mAnimCurrentTime += Time.deltaTime;
        if (mAnimCurrentTime >= mMaxTime)
        {
            OnOnceEnded();
        }
        else if (mFrameIndex != mLastFrameIndex)
        {
            mRenderer.sprite = mImages[mFrameIndex];
            mLastFrameIndex = mFrameIndex;
        }
        return true;
    }
	// Update is called once per frame
	void Update ()
    {
        if (!UpdateTransform())
            return;
        UpdateFrame();
	}
    SpriteRenderer mRenderer = null;
    GameObject mParent = null;
    float mAnimCurrentTime = 0.0f;
    float mOnceLoopDelayTimer = 0.0f;
    int mFrameIndex = 0;
    int mLastFrameIndex = -1;
    int mOnceLoopTimes = 0;
    int mTotalTimes = 0;
    bool mStoped = false;
    bool mEnded = false;
    Vector3 mStartPosition;
    Quaternion mStartRotation;
    Vector3 mFinalPosition;
    Quaternion mFinalRotation;
}
