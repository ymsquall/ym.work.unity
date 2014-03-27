using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerGravitySuperT = IH2DCGravity<H2DGravityController>;
    using PlayerMovableSuperT = IH2DCMovable<H2DMovableController>;
    using PlayerAnimSuperT = IH2DCAnimation<H2DAnimController>;
    using PlayerColliderSuperT = IH2DCCollider<H2DColliderController>;
    using PlayerColliderSelecterT = IH2DCreatureCollideSelecter;
    using PlayerOperationsSuperT = IH2DCOperations<H2DOperationsController>;
    public class H2DPlayerController : MonoBehaviour,
                                       PlayerGravitySuperT,
                                       PlayerMovableSuperT,
                                       PlayerAnimSuperT,
                                       PlayerColliderSuperT,
                                       PlayerColliderSelecterT,
                                       PlayerOperationsSuperT,
                                       IH2DCCamera
    {
#region 重力相关
        public float 重力 = 30.0f;
        H2DGravityController mGravityController;
        float PlayerGravitySuperT.Gravity
        {
            get { return 重力; }
        }
        bool PlayerGravitySuperT.Grounded
        {
            get { return mInGrounded || (mAnimController.NowAnimType == AnimationType.EANT_Skill01); }
        }
        bool PlayerGravitySuperT.Init()
        {
            mGravityController = new H2DGravityController(ThisGrivaty);
            return mGravityController.Init();
        }
        bool PlayerGravitySuperT.Update()
        {
            return mGravityController.Update();
        }
        PlayerGravitySuperT ThisGrivaty { get { return this; } }
#endregion

#region 位移相关
        public float 跑动速度 = 6.0f;
        public float 位移平滑速度 = 10.0f;
        public float 空中加速度 = 3.0f;
        public float 跳跃高度 = 1.0f;
        // 由于跳跃判断是在Update中，有时感觉已经落地，经由反射神经下意识按跳可能出现帧差，导致这次跳跃按键不起跳，这样感受不好
        // 所以从玩家抬起跳跃按纽后延迟一段时间保证跳跃标记能够保留到下一帧，这样就不会出现跳跃按钮空按的情况了
        public float 跳跃操作延时 = 0.2f;
        float mJumpBtnTouchEndedTimer = 0.0f;
        H2DMovableController mMovableController;
        bool PlayerMovableSuperT.Jumping
        {
            get
            {
                return mGravityController.VerticalSpeed > 0.0f;
            }
        }
        bool PlayerMovableSuperT.Droping
        {
            get
            {
                return !ThisMovable.Jumping && !ThisGrivaty.Grounded;
            }
        }
        bool PlayerMovableSuperT.Moving
        {
            get { return mMovableController.IsMoving; }
        }
        float PlayerMovableSuperT.SpeedScaleX
        {
            get{ return 跑动速度; }
        }
        float PlayerMovableSuperT.InputSpeedX
        {
            get
            {
                if (mAnimController.NowAnimType != AnimationType.EANT_Idel &&
                    mAnimController.NowAnimType != AnimationType.EANT_Running)
                    return 0.0f;
                return Input.GetAxisRaw("Horizontal");
            }
        }
        float PlayerMovableSuperT.SpeedSmoothing
        {
            get { return 位移平滑速度; }
        }
        float PlayerMovableSuperT.InAirControlAcceleration
        {
            get { return 空中加速度; }
        }
        float PlayerMovableSuperT.JumpHeight
        {
            get { return 跳跃高度; }
        }
        CharacterController PlayerMovableSuperT.Controller
        {
            get { return GetComponent<CharacterController>(); }
        }
        bool PlayerMovableSuperT.Init()
        {
            mMovableController = new H2DMovableController(ThisMovable);
            mMovableController.FaceDirection = transform.TransformDirection(Vector3.right);
            transform.rotation = Quaternion.LookRotation(mMovableController.FaceDirection);
            return mMovableController.Init();
        }
        bool PlayerMovableSuperT.Update()
        {
            bool wasMoving = ThisMovable.Moving;
            if (!mMovableController.UpdateSmoothedMovementDirection(ThisGrivaty.Grounded, transform))
                return false;
            // apply gravity
            if (!mGravityController.Update())
                return false;
            // apply jump
            if ((ThisGrivaty.Grounded && !ThisMovable.Jumping && !ThisMovable.Droping) && 
                (Input.GetButtonDown("Jump") || mUpdateCanJump))
            {
                mGravityController.VerticalSpeed += mMovableController.UpdateVerticalMovement(ThisGrivaty.Grounded, ThisGrivaty.Gravity);
                mInGrounded = false;
                ThisAnim.ChangeAnim(AnimationType.EANT_Jumpup);
            }
            if (ThisMovable.Droping)
                ThisAnim.ChangeAnim(AnimationType.EANT_Droping);
            if (mAnimController.NowAnimType == AnimationType.EANT_Droping && ThisGrivaty.Grounded)
                ThisAnim.ChangeAnim(AnimationType.EANT_JumpDown);
            float addedSpeed = 0;
            if (mAnimController.NowAnimType == AnimationType.EANT_Skill01)
                addedSpeed = 技能1附加位移速度;
            // 移动流程：先Movement计算好目标位置但不移动过去，用GroundMoveTest（双脚射线查询）测试目标位置会不会被地面挡住
            // 如果测试结果会被目标挡住则修正位置为地面位置并设置Grounded标志位true
            // 最后通过MovementAfter移动到目标位置（修复后的）
            // 因为GroundMoveTest每帧都会调用，所以不存在Grounded设为ture后走到空中无法感知的问题
            Vector3 outPos = transform.position;
            if (!mMovableController.Movement(addedSpeed, mGravityController.VerticalSpeed, ref outPos))
                return false;
            mInGrounded = mColliderController.GroundMoveTest(ThisMovable.Droping, ref outPos);
            if (!mMovableController.MovementAfter(ThisGrivaty.Grounded, outPos, transform))
                return false;
            if (ThisGrivaty.Grounded)
            {
                // Lock camera for short period when transitioning moving & standing still
                mLockCameraTimer += Time.deltaTime;
                if (ThisMovable.Moving != wasMoving)
                    mLockCameraTimer = 0.0f;
                if (ThisMovable.Moving)
                    ThisAnim.ChangeAnim(AnimationType.EANT_Running);
                else if (mAnimController.NowAnimType != AnimationType.EANT_Attack01 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Attack02 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Attack03 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Skill01 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Skill02)
                    ThisAnim.ChangeAnim(AnimationType.EANT_Idel);
            }
            else
            {
                // Lock camera while in air
                if (ThisMovable.Jumping)
                    mLockCameraTimer = 0.0f;
            }
            if (mInGrounded)
                mMovableController.Deceleration = 0;
            return true;
        }
        PlayerMovableSuperT ThisMovable { get { return this; } }
#endregion

#region 动作相关
        public AnimationClip    动作_待机;
        public float            动作速度_待机;
        public float            混合速度_待机;
        public WrapMode         混合模式_待机;
        public AnimationClip    动作_走;
        public float            动作速度_走;
        public float            混合速度_走;
        public WrapMode         混合模式_走;
        public AnimationClip    动作_跑;
        public float            动作速度_跑;
        public float            混合速度_跑;
        public WrapMode         混合模式_跑;
        public AnimationClip    动作_起跳;
        public float            动作速度_起跳;
        public float            混合速度_起跳;
        public WrapMode         混合模式_起跳;
        public AnimationClip    动作_空中;
        public float            动作速度_空中;
        public float            混合速度_空中;
        public WrapMode         混合模式_空中;
        public AnimationClip    动作_下落;
        public float            动作速度_下落;
        public float            混合速度_下落;
        public WrapMode         混合模式_下落;
        public AnimationClip    动作_落地;
        public float            动作速度_落地;
        public float            混合速度_落地;
        public WrapMode         混合模式_落地;
        public AnimationClip    动作_技能1;
        public float            动作速度_技能1;
        public float            混合速度_技能1;
        public WrapMode         混合模式_技能1;
        public AnimationClip    动作_技能2;
        public float            动作速度_技能2;
        public float            混合速度_技能2;
        public WrapMode         混合模式_技能2;
        public AnimationClip    动作_攻击1;
        public float            动作速度_攻击1;
        public float            混合速度_攻击1;
        public WrapMode         混合模式_攻击1;
        public AnimationClip    动作_攻击2;
        public float            动作速度_攻击2;
        public float            混合速度_攻击2;
        public WrapMode         混合模式_攻击2;
        public AnimationClip    动作_攻击3;
        public float            动作速度_攻击3;
        public float            混合速度_攻击3;
        public WrapMode         混合模式_攻击3;
        public AnimationClip    动作_空中攻击1;
        public float            动作速度_空中攻击1;
        public float            混合速度_空中攻击1;
        public WrapMode         混合模式_空中攻击1;
        public AnimationClip    动作_被击;
        public float            动作速度_被击;
        public float            混合速度_被击;
        public WrapMode         混合模式_被击;
        public AnimationClip    动作_被击飞;
        public float            动作速度_被击飞;
        public float            混合速度_被击飞;
        public WrapMode         混合模式_被击飞;
        public AnimationClip    动作_死亡;
        public float            动作速度_死亡;
        public float            混合速度_死亡;
        public WrapMode         混合模式_死亡;
        H2DAnimController mAnimController;
        Animation PlayerAnimSuperT.AnimationInst
        {
            get
            {
                return GetComponentInChildren<Animation>();
            }
        }
        // 实现动画接口的初始化方法
        bool PlayerAnimSuperT.Init()
        {
            mAnimController = new H2DAnimController(ThisAnim);
            mAnimController.SetAnimClip(AnimationType.EANT_Idel, 动作_待机, 动作速度_待机, 混合速度_待机, 混合模式_待机);
            mAnimController.SetAnimClip(AnimationType.EANT_Walk, 动作_走, 动作速度_走, 混合速度_走, 混合模式_走);
            mAnimController.SetAnimClip(AnimationType.EANT_Running, 动作_跑, 动作速度_跑, 混合速度_跑, 混合模式_跑);
            mAnimController.SetAnimClip(AnimationType.EANT_Jumpup, 动作_起跳, 动作速度_起跳, 混合速度_起跳, 混合模式_起跳);
            mAnimController.SetAnimClip(AnimationType.EANT_Airing, 动作_空中, 动作速度_空中, 混合速度_空中, 混合模式_空中);
            mAnimController.SetAnimClip(AnimationType.EANT_Droping, 动作_下落, 动作速度_下落, 混合速度_下落, 混合模式_下落);
            mAnimController.SetAnimClip(AnimationType.EANT_JumpDown, 动作_落地, 动作速度_落地, 混合速度_落地, 混合模式_落地);
            mAnimController.SetAnimClip(AnimationType.EANT_Skill01, 动作_技能1, 动作速度_技能1, 混合速度_技能1, 混合模式_技能1);
            mAnimController.SetAnimClip(AnimationType.EANT_Skill02, 动作_技能2, 动作速度_技能2, 混合速度_技能2, 混合模式_技能2);
            mAnimController.SetAnimClip(AnimationType.EANT_Attack01, 动作_攻击1, 动作速度_攻击1, 混合速度_攻击1, 混合模式_攻击1);
            mAnimController.SetAnimClip(AnimationType.EANT_Attack02, 动作_攻击2, 动作速度_攻击2, 混合速度_攻击2, 混合模式_攻击2);
            mAnimController.SetAnimClip(AnimationType.EANT_Attack03, 动作_攻击3, 动作速度_攻击3, 混合速度_攻击3, 混合模式_攻击3);
            mAnimController.SetAnimClip(AnimationType.EANT_AirAttack01, 动作_空中攻击1, 动作速度_空中攻击1, 混合速度_空中攻击1, 混合模式_空中攻击1);
            mAnimController.SetAnimClip(AnimationType.EANT_BeAttack, 动作_被击, 动作速度_被击, 混合速度_被击, 混合模式_被击);
            mAnimController.SetAnimClip(AnimationType.EANT_Clobber, 动作_被击飞, 动作速度_被击飞, 混合速度_被击飞, 混合模式_被击飞);
            mAnimController.SetAnimClip(AnimationType.EANT_Death, 动作_死亡, 动作速度_死亡, 混合速度_死亡, 混合模式_死亡);
            return true;
        }
        bool PlayerAnimSuperT.ChangeAnim(AnimationType animType)
        {
            mAnimController.NowAnimType = animType;
            return true;
        }
        bool PlayerAnimSuperT.OnAnimOvered(AnimationType animType)
        {
            if (AnimationType.EANT_JumpDown == animType)
                return true;
            AnimationType nowAnimType = mAnimController.NowAnimType;
            if (nowAnimType == animType)
            {
                switch (animType)
                {
                    case AnimationType.EANT_Attack01:
                    case AnimationType.EANT_Attack02:
                    case AnimationType.EANT_Attack03:
                        {
                            if (ThisMovable.Moving)
                                ThisAnim.ChangeAnim(AnimationType.EANT_Running);
                            else
                                ThisAnim.ChangeAnim(AnimationType.EANT_Idel);
                            //mMeshPhysicsCollider.ActivePhysics(false);
                        }
                        break;
                    case AnimationType.EANT_Jumpup:
                        ThisAnim.ChangeAnim(AnimationType.EANT_Airing);
                        break;
                }
            }
            else if ((nowAnimType == AnimationType.EANT_Skill02) &&
                    (animType == AnimationType.EANT_Attack01))
            {
                if (ThisMovable.Moving)
                    ThisAnim.ChangeAnim(AnimationType.EANT_Running);
                else
                    ThisAnim.ChangeAnim(AnimationType.EANT_Idel);
            }
            else
            {
                Debug.Log(string.Format("未知的动画播放完成事件{0}", animType.ToString()));
                return false;
            }
            return true;
        }
        bool PlayerAnimSuperT.Update()
        {
            mAnimController.Update();
            return true;
        }
        PlayerAnimSuperT ThisAnim { get { return this; } }
        
#endregion

#region 碰撞相关
        public Collider 碰撞层_地面 = null;
        //public Collider 碰撞层_非地面 = null;
        public LayerMask 地面层掩码 = 0;
        H2DColliderController mColliderController;
        Collider PlayerColliderSuperT.GroundCollider
        {
            get { return 碰撞层_地面; }
        }
        LayerMask PlayerColliderSuperT.GroundLayerMask
        {
            get { return 地面层掩码; }
        }
        bool PlayerColliderSuperT.Init()
        {
            mColliderController = new H2DColliderController(ThisCollider);
            return true;
        }
        bool PlayerColliderSuperT.Update()
        {
            return true;
        }
        public PlayerColliderSuperT ThisCollider { get { return this; } }
#endregion

#region 碰撞选择器实现
        bool PlayerColliderSelecterT.OnH2DCCollisionEnter(Collision collisionInfo, H2DCCollideSelecterType type)
        {
            //Debug.Log(string.Format("OnH2DCCollisionEnter:{0}", type.ToString()));
            switch(type)
            {
                case H2DCCollideSelecterType.地面:
                    mInGrounded = true;
                    break;
                case H2DCCollideSelecterType.非地面:
                    break;
            }
            return true;
        }
        bool PlayerColliderSelecterT.OnH2DCCollisionExit(Collision collisionInfo, H2DCCollideSelecterType type)
        {
            //Debug.Log(string.Format("OnH2DCCollisionEnter:{0}", type.ToString()));
            switch(type)
            {
                case H2DCCollideSelecterType.地面:
                    mInGrounded = false;
                    break;
                case H2DCCollideSelecterType.非地面:
                    break;
            }
            return true;
        }
        bool PlayerColliderSelecterT.OnH2DCCollisionStay(Collision collisionInfo, H2DCCollideSelecterType type)
        {
            //Debug.Log(string.Format("OnH2DCCollisionEnter:{0}", type.ToString()));
            return true;
        }
        bool mInGrounded = false;
#endregion

#region 玩家操作相关
        public int 普攻最大连击数 = 3;
        public float 普攻连击超时时间 = 1.0f;
        public float 技能1持续时间 = 0.5f;
        public float 技能1附加位移速度 = 8.0f;
        bool mJumpBtnTouched = false;
        bool mUpdateCanJump = false;
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
            if (!mJumpBtnTouched)
            {
                mJumpBtnTouchEndedTimer -= Time.deltaTime;
                if (mJumpBtnTouchEndedTimer <= 0.0f)
                    mUpdateCanJump = false;
            }
            mOperationsController.Update();
            return true;
        }
        bool PlayerOperationsSuperT.DoTouchBegin(OperationType ot)
        {
            switch (ot)
            {
                case OperationType.jump:
                    mJumpBtnTouched = true;
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
                    mJumpBtnTouched = false;
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

        // Use this for initialization
        void Awake()
        {
            ThisGrivaty.Init();
            ThisMovable.Init();
            ThisAnim.Init();
            ThisCollider.Init();
            ThisOperate.Init();
        }
        // Update is called once per frame
        void Update()
        {
            //ThisGrivaty.Update();
            ThisMovable.Update();
            ThisAnim.Update();
            ThisOperate.Update();
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