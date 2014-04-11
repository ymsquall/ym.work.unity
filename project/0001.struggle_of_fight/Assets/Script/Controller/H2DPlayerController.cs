using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerGravitySuperT = IH2DCGravity<H2DGravityController>;
    using PlayerMovableSuperT = IH2DCMovable<H2DMovableController>;
    using PlayerAnimSuperT = IH2DCAnimation<H2DAnimController>;
    using PlayerColliderSuperT = IH2DCCollider<H2DColliderController>;
    using PlayerColliderSelecterT = IH2DCharacterCollideSelecter;
    using PlayerOperationsSuperT = IH2DCOperations<H2DOperationsController>;
    public class H2DPlayerController : H2DCharacterController,
                                       PlayerGravitySuperT,
                                       PlayerMovableSuperT,
                                       PlayerAnimSuperT,
                                       PlayerColliderSuperT,
                                       PlayerColliderSelecterT,
                                       PlayerOperationsSuperT,
                                       IH2DCCamera
    {
        static H2DPlayerController mLocalPlayer = null;
        public static H2DPlayerController LocalPlayer { get { return mLocalPlayer; } }
        float mTouchMoveSpeed = 0.0f;
        public float TouchMoveSpeed
        {
            set { mTouchMoveSpeed = value; }
            get { return mTouchMoveSpeed; }
        }
        float PlayerMovableSuperT.InputSpeedX
        {
            get
            {
                if (mAnimController.NowAnimType != AnimationType.EANT_Idel &&
                    mAnimController.NowAnimType != AnimationType.EANT_Running &&
                    mAnimController.NowAnimType != AnimationType.EANT_Airing &&
                    mAnimController.NowAnimType != AnimationType.EANT_Droping &&
                    mAnimController.NowAnimType != AnimationType.EANT_AirAttack01 &&
                    mAnimController.NowAnimType != AnimationType.EANT_Skill02)
                    return 0.0f;
                return Input.GetAxisRaw("Horizontal") + TouchMoveSpeed;
            }
        }
#region 玩家操作相关
        public int 普攻最大连击数 = 3;
        public float 普攻连击超时时间 = 1.0f;
        public float 技能1持续时间 = 0.5f;
        // 由于跳跃判断是在Update中，有时感觉已经落地，经由反射神经下意识按跳可能出现帧差，导致这次跳跃按键不起跳，这样感受不好
        // 所以从玩家抬起跳跃按纽后延迟一段时间保证跳跃标记能够保留到下一帧，这样就不会出现跳跃按钮空按的情况了
        public float 跳跃操作延时 = 0.2f;
        float mJumpBtnTouchEndedTimer = 0.0f;
        //bool mJumpBtnTouched = false;
        int mComboWithJumpCount = 0;
        H2DOperationsController mOperationsController;
        public AnimationType AnimType
        {
            get { return mAnimController.NowAnimType; }
        }
        int PlayerOperationsSuperT.AttackComboMaxNum
        {
            get { return 普攻最大连击数; }
        }
        float PlayerOperationsSuperT.AttackComboTimeout
        {
            get { return 普攻连击超时时间; }
        }
        float PlayerOperationsSuperT.Skill1MaxTime
        {
            get { return 技能1持续时间; }
        }
        bool PlayerOperationsSuperT.Init()
        {
            mOperationsController = new H2DOperationsController(ThisOperate);
            return true;
        }
        bool PlayerOperationsSuperT.Update()
        {
            //if (!mJumpBtnTouched)
            //{
            //    mJumpBtnTouchEndedTimer -= Time.deltaTime;
            //    if (mJumpBtnTouchEndedTimer <= 0.0f)
            //        mUpdateCanJump = false;
            //}
            mOperationsController.Update();
            return true;
        }
        protected override void OnJumpDown()
        {
            mComboWithJumpCount = 0;
        }
        bool PlayerOperationsSuperT.DoTouchBegin(OperationType ot)
        {
            switch (ot)
            {
                case OperationType.jump:
                    if (mComboWithJumpCount == 0 &&
                        (AnimationType.EANT_Idel == mAnimController.NowAnimType ||
                        AnimationType.EANT_Running == mAnimController.NowAnimType ||
                        AnimationType.EANT_JumpDown == mAnimController.NowAnimType))
                        mJumpHeight = 跳跃高度;
                    else if (mComboWithJumpCount == 1 &&
                        (AnimationType.EANT_Airing == mAnimController.NowAnimType ||
                        AnimationType.EANT_Droping == mAnimController.NowAnimType))
                        mJumpHeight = 跳跃高度 * 0.6f;
                    else if (mComboWithJumpCount == 0 && AnimationType.EANT_Droping == mAnimController.NowAnimType)
                    {
                        mJumpHeight = 跳跃高度 * 0.6f;
                        mComboWithJumpCount = 1;
                    }
                    else
                        break;
                    mComboWithJumpCount++;
                    //mJumpBtnTouched = true;
                    mUpdateCanJump = true;
                    Debug.Log("jump button touch begin");
                    if (AnimationType.EANT_Idel == mAnimController.NowAnimType ||
                        AnimationType.EANT_Running == mAnimController.NowAnimType ||
                        AnimationType.EANT_JumpDown == mAnimController.NowAnimType)
                    {
                        mJumpBtnTouchEndedTimer = 跳跃操作延时;
                    }
                    break;
                case OperationType.attack:
                    if (AnimationType.EANT_Idel == mAnimController.NowAnimType || AnimationType.EANT_Running == mAnimController.NowAnimType)
                        mMovableController.MoveSpeed = 0.0f;
                    mOperationsController.DoAttack();
                    //mMeshPhysicsCollider.ActivePhysics(true);
                    break;
                case OperationType.skill1:
                    mMovableController.MoveSpeed = 0.0f;
                    mOperationsController.DoSkill(1);
                    //mMeshPhysicsCollider.ActivePhysics(true);
                    break;
                case OperationType.skill2:
                    mOperationsController.DoSkill(2);
                    break;
            }
            return true;
        }
        bool PlayerOperationsSuperT.DoTouchEnded(OperationType ot)
        {
            switch (ot)
            {
                case OperationType.jump:
                    //mJumpBtnTouched = false;
                    Debug.Log("jump button touch ended");
                    break;
                case OperationType.attack:
                    break;
                case OperationType.skill1:
                    break;
                case OperationType.skill2:
                    break;
            }
            return true;
        }
        bool PlayerOperationsSuperT.ChangeAnimType(AnimationType animType)
        {
            return ThisAnim.ChangeAnim(animType);
        }
        bool PlayerOperationsSuperT.OnSkillOvered(int id)
        {
            if (ThisMovable.Moving)
                ThisAnim.ChangeAnim(AnimationType.EANT_Running);
            else
                ThisAnim.ChangeAnim(AnimationType.EANT_Idel);
            if(id == 1)
                mMovableController.Deceleration = 技能1附加位移速度;
            return true;
        }
        public PlayerOperationsSuperT ThisOperate { get { return this; } }
#endregion

#region 摄像机需求的参数
        float IH2DCCamera.LockCameraTimer
        {
            get { return mLockCameraTimer; }
        }
        Bounds IH2DCCamera.Bounds
        {
            get { return GetComponent<CharacterController>().bounds; }
        }
        public float LockCameraTimer
        {
            get { return mLockCameraTimer; }
        }
#endregion
        bool mUpdateCanJump = false;
        protected override bool UpdateCanJump
        {
            set { mUpdateCanJump = value; }
            get { return Input.GetButtonDown("Jump") || mUpdateCanJump; }
        }
        // Use this for initialization
        protected override bool AwakeImpl()
        {
            mLocalPlayer = this;
            return ThisOperate.Init();
        }
        // Update is called once per frame
        protected override bool UpdateImpl()
        {
            return ThisOperate.Update();
        }
        override protected bool UpdateMovementImpl(bool wasMoving)
        {
            if (ThisGrivaty.Grounded)
            {
                // Lock camera for short period when transitioning moving & standing still
                mLockCameraTimer += Time.deltaTime;
                if (ThisMovable.Moving != wasMoving)
                    mLockCameraTimer = 0.0f;
            }
            else
            {
                // Lock camera while in air
                if (ThisMovable.Jumping)
                    mLockCameraTimer = 0.0f;
            }
            return true;
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

        float mLockCameraTimer = 0.0f;
    }
}