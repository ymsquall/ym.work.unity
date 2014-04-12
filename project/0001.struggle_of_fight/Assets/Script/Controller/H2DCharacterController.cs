using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerGravitySuperT = IH2DCGravity<H2DGravityController>;
    using PlayerMovableSuperT = IH2DCMovable<H2DMovableController>;
    using PlayerAnimSuperT = IH2DCAnimation<H2DAnimController>;
    using PlayerColliderSuperT = IH2DCCollider<H2DColliderController>;
    using PlayerColliderSelecterT = IH2DCharacterCollideSelecter;
    public class H2DCharacterController : MonoBehaviour,
                                       PlayerGravitySuperT,
                                       PlayerMovableSuperT,
                                       PlayerAnimSuperT,
                                       PlayerColliderSuperT,
                                       PlayerColliderSelecterT
    {
#region 重力相关
        public float 重力 = 30.0f;
        protected H2DGravityController mGravityController;
        float PlayerGravitySuperT.Gravity
        {
            get { return 重力; }
        }
        bool PlayerGravitySuperT.Grounded
        {
            get { return mInGrounded; }
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
        protected PlayerGravitySuperT ThisGrivaty { get { return this; } }
#endregion

#region 位移相关
        public float 跑动速度 = 6.0f;
        public float 位移平滑速度 = 10.0f;
        public float 空中加速度 = 3.0f;
        public float 跳跃高度 = 1.0f;
        public float 技能1附加位移速度 = 8.0f;
        public bool 使用模型镜像 = true;
        protected float mJumpHeight = 1.0f;
        protected float mFocusMoveValue = 0.0f;
        protected H2DMovableController mMovableController;
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
                return !ThisMovable.Jumping && (mGravityController.VerticalSpeed < 0.0f) && !ThisGrivaty.Grounded;
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
                return mFocusMoveValue;
            }
        }
        public float FocusMoveValue
        {
            set { mFocusMoveValue = value; }
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
            get { return mJumpHeight; }
        }
        bool PlayerMovableSuperT.UsedModelFlipX
        {
            get { return 使用模型镜像; }
        }
        CharacterController PlayerMovableSuperT.Controller
        {
            get { return GetComponent<CharacterController>(); }
        }
        public Vector3 FaceDirection
        {
            set { mMovableController.FaceDirection = value; }
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
            if (!mMovableController.UpdateSmoothedMovementDirection(ThisGrivaty.Grounded,
                mAnimController.NowAnimType == AnimationType.EANT_Airing, mAnimController.NowAnimType == AnimationType.EANT_Droping, transform))
                return false;
            // apply gravity
            if (!mGravityController.Update())
                return false;
            // apply jump
            if (UpdateCanJump)
            {
                mGravityController.VerticalSpeed = mMovableController.UpdateVerticalMovement(ThisGrivaty.Grounded, ThisGrivaty.Gravity);
                ThisAnim.ChangeAnim(AnimationType.EANT_Jumpup);
                mInGrounded = false;
                UpdateCanJump = false;
                if (mFocusJump)
                    mFocusJump = false;
            }
            if (mAnimController.NowAnimType == AnimationType.EANT_Skill01)
            {
                mGravityController.VerticalSpeed = 0.0f;
            }
            else if (ThisMovable.Droping && 
                mAnimController.NowAnimType != AnimationType.EANT_Skill02 &&
                mAnimController.NowAnimType != AnimationType.EANT_AirAttack01)
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
            mAnimController.RunAnimSpeedScale = Mathf.Abs(ThisMovable.InputSpeedX);
            if (ThisGrivaty.Grounded)
            {
                if (ThisMovable.Moving)
                    ThisAnim.ChangeAnim(AnimationType.EANT_Running);
                else if (mAnimController.NowAnimType != AnimationType.EANT_Attack01 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Attack02 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Attack03 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Skill01 &&
                            mAnimController.NowAnimType != AnimationType.EANT_Skill02 &&
                            mAnimController.NowAnimType != AnimationType.EANT_AirAttack01)
                    ThisAnim.ChangeAnim(AnimationType.EANT_Idel);
                OnJumpDown();
            }
            UpdateMovementImpl(wasMoving);
            if (mInGrounded)
                mMovableController.Deceleration = 0;
            return true;
        }
        virtual protected bool UpdateMovementImpl(bool wasMoving) { return true; }
        protected virtual void OnJumpDown() { }
        protected PlayerMovableSuperT ThisMovable { get { return this; } }
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
        protected H2DAnimController mAnimController;
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
                    case AnimationType.EANT_AirAttack01:
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
            else if (animType == AnimationType.EANT_AirAttack01)
            {
                if (ThisMovable.Moving)
                    ThisAnim.ChangeAnim(AnimationType.EANT_Running);
                else
                    ThisAnim.ChangeAnim(AnimationType.EANT_Idel);
            }
            else
            {
                Debug.Log(string.Format("未知的动画播放完成事件{0}, 当前动作{1}", animType.ToString(), nowAnimType.ToString()));
                return false;
            }
            return true;
        }
        bool PlayerAnimSuperT.Update()
        {
            mAnimController.Update();
            return true;
        }
        protected PlayerAnimSuperT ThisAnim { get { return this; } }
        
#endregion

#region 碰撞相关
        public Collider 碰撞层_地面 = null;
        //public Collider 碰撞层_非地面 = null;
        public LayerMask 地面层掩码 = 0;
        protected H2DColliderController mColliderController;
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
        protected bool mInGrounded = false;
#endregion
        protected bool mFocusJump = false;
        protected virtual bool UpdateCanJump
        {
            set { }
            get { return mFocusJump; }
        }
        public bool FocusJumpEnd
        {
            set { mFocusJump = false; mJumpHeight = 跳跃高度; }
        }
        public float FocusJumpBegin
        {
            set { mFocusJump = true;  mJumpHeight = value; }
        }
        // Use this for initialization
        void Awake()
        {
            ThisGrivaty.Init();
            ThisMovable.Init();
            ThisAnim.Init();
            ThisCollider.Init();
            AwakeImpl();
        }
        protected virtual bool AwakeImpl() { return true; }
        // Update is called once per frame
        void Update()
        {
            ThisMovable.Update();
            ThisAnim.Update();
            UpdateImpl();
        }
        protected virtual bool UpdateImpl() { return true; }
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
        }
        void OnTriggerEnter(Collider other)
        {

        }
        void OnCollisionEnter(Collision collisionInfo)
        {

        }
    }
}