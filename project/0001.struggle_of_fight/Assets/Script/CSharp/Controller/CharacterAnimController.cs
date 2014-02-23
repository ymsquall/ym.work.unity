using UnityEngine;
using System.Collections;

public class CharacterAnimController : MonoBehaviour
{
    public AnimationClip mAnim02_Jumpup;
    public AnimationClip mAnim03_Skill01;
    public AnimationClip mAnim04_Attack01;
    public AnimationClip mAnim05_Attack02;
    public AnimationClip mAnim06_Attack03;
    public AnimationClip mAnim07_AirAttack;
    public AnimationClip mAnim08_Running;
    public AnimationClip mAnim09_BeAttack;
    public AnimationClip mAnim10_Death;
    public AnimationClip mAnim11_Clobber;
    public AnimationClip mAnim12_JumpAir;
    public AnimationClip mAnim13_JumpDown;
    public AnimationClip mAnim14_Idel;

    public float mJumpAnimSpeed = 1.15f;
    public float mRunAnimSpeed = 1.0f;
    public float mInLandAnimSpeed = 1.0f;
    public float mBeAttackAnimSpeed = 0.1f;
    public float mBeAttackAnimMaxTime = 1.0f;
    public float mClobberAnimSpeed = 1.0f;
    public float mClobberAnimMaxTime = 3.0f;

    public float mAttackAnimSpeed = 0.2f;
    public float mSkill1AnimSpeed = 1.0f;
    public float mSkill1MoveSpeed = 15.0f;
    public float mSkill2AnimSpeed = 0.3f;
    
    public CharacterGravityController mGravityController = null;

    public enum CharacterState : byte
    {
        Jumpup = 2,
        Skill01 = 3,
        Attack01 = 4,
        Attack02 = 5,
        Attack03 = 6,
        AirAttack = 7,
        Running = 8,
        BeAttack = 9,
        Death = 10,
        Clobber = 11,
        JumpAir = 12,
        JumpDown = 13,
        Idel = 14,
        Skill02 = 15
    }

    void Awake()
    {
        mController = GetComponent<CharacterController>();
        mPlayingAnim = GetComponent<Animation>();
        if (!mPlayingAnim)
            Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
        if (!mAnim14_Idel)
        {
            mPlayingAnim = null;
            Debug.Log("No idle animation found. Turning off animations.");
        }
    }

    public void DoBeAttack(bool clobber, float clobberDirX)
    {
        if (CharacterState.Idel == mState || CharacterState.Running == mState)
        {
            if (clobber)
            {
                if (mNowAnimTimer <= 0.0f)
                {
                    mGravityController.DoClobber(clobberDirX);
                    mState = CharacterState.Clobber;
                    mNowAnimTimer = mClobberAnimMaxTime;
                    if (mPlayingAnim.IsPlaying(mAnim11_Clobber.name))
                        mPlayingAnim.Stop(mAnim11_Clobber.name);
                }
            }
            else if (mNowAnimTimer <= 0.0f)
            {
                mState = CharacterState.BeAttack;
                mNowAnimTimer = mBeAttackAnimMaxTime;
                if (mPlayingAnim.IsPlaying(mAnim09_BeAttack.name))
                    mPlayingAnim.Stop(mAnim09_BeAttack.name);
            }
        }
    }
    public void OnAnimationOvered(CharacterState state)
    {
        if (mState == state)
        {
            switch (state)
            {
                case CharacterState.BeAttack:
                    {
                        if (mGravityController.Moving)
                            mState = CharacterState.Running;
                        else
                            mState = CharacterState.Idel;
                    }
                    break;
            }
        }
    }
    void AnimationSector()
    {
        // ANIMATION sector
        if (mPlayingAnim)
        {
            if (mState == CharacterState.Jumpup)
            {
                mPlayingAnim[mAnim02_Jumpup.name].speed = mJumpAnimSpeed;
                mPlayingAnim[mAnim02_Jumpup.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim02_Jumpup.name);
            }
            else if (mState == CharacterState.JumpAir)
            {
                mPlayingAnim[mAnim12_JumpAir.name].speed = mJumpAnimSpeed;
                mPlayingAnim[mAnim12_JumpAir.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim12_JumpAir.name);
            }
            else if (mState == CharacterState.JumpDown)
            {
                mPlayingAnim[mAnim13_JumpDown.name].speed = mJumpAnimSpeed;
                mPlayingAnim[mAnim13_JumpDown.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim13_JumpDown.name);
            }
            else if (mState == CharacterState.Attack01)
            {
                mPlayingAnim[mAnim04_Attack01.name].speed = mAttackAnimSpeed;
                mPlayingAnim[mAnim04_Attack01.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim04_Attack01.name);
            }
            else if (mState == CharacterState.Attack02)
            {
                mPlayingAnim[mAnim05_Attack02.name].speed = mAttackAnimSpeed;
                mPlayingAnim[mAnim05_Attack02.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim05_Attack02.name);
            }
            else if (mState == CharacterState.Attack03)
            {
                mPlayingAnim[mAnim06_Attack03.name].speed = mAttackAnimSpeed;
                mPlayingAnim[mAnim06_Attack03.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim06_Attack03.name);
            }
            else if (mState == CharacterState.Skill01)
            {
                mPlayingAnim[mAnim03_Skill01.name].speed = mSkill1AnimSpeed;
                mPlayingAnim[mAnim03_Skill01.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim03_Skill01.name);
            }
            else if (mState == CharacterState.Skill02)
            {
                mPlayingAnim[mAnim04_Attack01.name].speed = mSkill2AnimSpeed;
                mPlayingAnim[mAnim04_Attack01.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim04_Attack01.name);
            }
            else if (mState == CharacterState.BeAttack)
            {
                mPlayingAnim[mAnim09_BeAttack.name].speed = mBeAttackAnimSpeed;
                mPlayingAnim[mAnim09_BeAttack.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim09_BeAttack.name);
            }
            else if (mState == CharacterState.Clobber)
            {
                mPlayingAnim[mAnim11_Clobber.name].speed = mClobberAnimSpeed;
                mPlayingAnim[mAnim11_Clobber.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim11_Clobber.name);
            }
            else
            {
                if (mController.velocity.sqrMagnitude < 0.1)
                {
                    mPlayingAnim[mAnim14_Idel.name].speed = 10.0f;
                    mPlayingAnim.CrossFade(mAnim14_Idel.name);
                }
                else
                {
                    if (mState == CharacterState.Running)
                    {
                        mPlayingAnim[mAnim08_Running.name].speed = Mathf.Clamp(mController.velocity.magnitude, 0.0f, mRunAnimSpeed);
                        mPlayingAnim.CrossFade(mAnim08_Running.name);
                    }
                }
            }
        }
    }
	// Update is called once per frame
	void Update ()
    {
        AnimationSector();
        mNowAnimTimer -= Time.deltaTime;
        if (mState == CharacterState.Clobber && mNowAnimTimer < 0.5f)
        {
            if (mGravityController.Moving)
                mState = CharacterState.Running;
            else
                mState = CharacterState.Idel;
        }
	}

    CharacterState mState = CharacterState.Idel;
    Animation mPlayingAnim = null;
    CharacterController mController = null;
    public float mNowAnimTimer = 0.0f;
}
