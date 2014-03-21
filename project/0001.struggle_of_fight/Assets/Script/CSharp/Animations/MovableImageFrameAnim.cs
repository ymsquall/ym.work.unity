using UnityEngine;
using System.Collections;

[AddComponentMenu("自定义/动画/序列帧动画(运动)")]
public class MovableImageFrameAnim : ImageFrameAnim
{
    public enum MoveType
    {
        MoveType_Default,
        MoveType_Released,
    }
    public MoveType mMoveType = MoveType.MoveType_Default;
    // Use this for initialization
    void Awake()
    {
        Init();
    }
    protected override void PlayImpl()
    {
        mSelfMoveDir = mParent.transform.rotation.y > 0.0f ? Vector3.forward : Vector3.back;
    }
    protected override void CustonMove()
    {
        mSelfMoveDstance += mSelfMoveDir * mSelfMoveSpeed * Time.deltaTime;
        //mFinalPosition += mSelfMoveDstance;
    }
	// Update is called once per frame
	void Update ()
    {
        if (!UpdateTransform())
            return;
        UpdateFrame();
    }
    protected Vector3 mSelfMoveDstance = Vector3.zero;
}
