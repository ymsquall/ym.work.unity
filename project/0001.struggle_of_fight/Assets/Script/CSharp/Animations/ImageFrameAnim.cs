using UnityEngine;
using System.Collections;

[AddComponentMenu("Extern/序列帧动画组件")]
public class ImageFrameAnim : MonoBehaviour
{
    public Sprite[] mImages = null;
    public float mMaxTime = 1.0f;
    public bool mLoop = false;
    public int mTimes = 1;
    public float mOnceLoopDelayTime = 1.0f;
    public int mStartFrameIndex = 0;
    public bool mPaused = false;

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
	void Awake ()
    {
        mRenderer = GetComponent<SpriteRenderer>();
        if (mRenderer == null)
            Debug.LogError("ImageFrameAnim组件必须依附于SpriteRenderer组件！");
        if (mImages == null || mImages.Length <= 1)
            Debug.LogError("ImageFrameAnim组件必须设置大于1张图片！");
        if (mStartFrameIndex >= mImages.Length)
            Debug.LogError("ImageFrameAnim组件起始帧索引不能大于帧数量！");
        if (mLoop && mTimes == 0)
            Debug.Log("播放次数设为0是就不需要开启循环标志，该动画也将无限循环播放！");
        mParent = transform.parent.gameObject;
        if (mPaused)
            mRenderer.sprite = null;
        else
            mRenderer.sprite = mImages[mStartFrameIndex];
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
        mRenderer.sprite = null;
    }
    void ApplyTransform()
    {
        if(mParent != null)
        {
            //var cameraTransform = Camera.main.transform;
            //if (mParent.transform.rotation.y > 0.0f)
            //{
            //    transform.localRotation = mParent.transform.rotation
            //    transform.Rotate(0,0,0);
            //}
            //else
            //{
            //    transform.Rotate(0, 180, 0);
            //}
            transform.localRotation = new Quaternion(0, mParent.transform.rotation.y / 2.0f, 0, 0);
            Debug.Log(string.Format("player roatY{0}", mParent.transform.rotation.y / 2.0f));
        }
    }
	// Update is called once per frame
	void Update ()
    {
        if (mEnded || mPaused || mStoped)
            return;
        if (mOnceLoopDelayTimer > 0.0f)
        {
            mOnceLoopDelayTimer -= Time.deltaTime;
            return;
        }
        ApplyTransform();
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
}
