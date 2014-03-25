﻿using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerGravitySuperT = IH2DCGravity<H2DGravityController>;
    using PlayerMovableSuperT = IH2DCMovable<H2DMovableController>;
    using PlayerAnimSuperT = IH2DCAnimation<H2DAnimController>;
    using PlayerColliderSuperT = IH2DCCollider<H2DColliderController>;
    using PlayerColliderSelecterT = IH2DCreatureCollideSelecter;
    public class H2DPlayerController : MonoBehaviour,
                                       PlayerGravitySuperT,
                                       PlayerMovableSuperT,
                                       PlayerAnimSuperT,
                                       PlayerColliderSuperT,
                                       PlayerColliderSelecterT,
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
        PlayerGravitySuperT ThisGrivaty { get { return this; } }
#endregion

#region 位移相关
        public float 跑动速度 = 6.0f;
        public float 位移平滑速度 = 10.0f;
        public float 空中加速度 = 3.0f;
        public float 跳跃高度 = 1.0f;
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
            get{ return Input.GetAxisRaw("Horizontal"); }
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
            return mGravityController.Init();
        }
        bool PlayerMovableSuperT.Update()
        {
            bool wasMoving = ThisMovable.Moving;
            if (!mMovableController.UpdateSmoothedMovementDirection(ThisGrivaty.Grounded, transform))
                return false;
            if (ThisGrivaty.Grounded)
            {
                // Lock camera for short period when transitioning moving & standing still
                mLockCameraTimer += Time.deltaTime;
                if (ThisMovable.Moving != wasMoving)
                    mLockCameraTimer = 0.0f;
                ThisAnim.ChangeAnim(AnimationType.EANT_Running);
            }
            else
            {
                // Lock camera while in air
                if (ThisMovable.Jumping)
                    mLockCameraTimer = 0.0f;
            }
            // apply gravity
            if (!mGravityController.Update())
                return false;
            // apply jump
            if (ThisGrivaty.Grounded && Input.GetButtonDown("Jump"))
            {
                mGravityController.VerticalSpeed += mMovableController.UpdateVerticalMovement(ThisGrivaty.Grounded, ThisGrivaty.Gravity);
                mInGrounded = false;
            }
            // movement
            Vector3 outPos = transform.position;
            if (!mMovableController.Movement(0.0f, mGravityController.VerticalSpeed, ref outPos))
                return false;
            mInGrounded = mColliderController.GroundMoveTest(ThisMovable.Droping, ref outPos);
            if (!mMovableController.MovementAfter(ThisGrivaty.Grounded, outPos, transform))
                return false;
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
                //GameObject model = GetComponentInChildren<Animation>();
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
        bool PlayerColliderSuperT.RayInGround
        {
            set
            {
                mRayInGround = value;
                if (mRayInGround)
                    mGravityController.VerticalSpeed = 0.0f;
            }
            get { return mRayInGround; }
        }
        Collider PlayerColliderSuperT.GroundCollider
        {
            get { return 碰撞层_地面; }
        }
        //Collider PlayerColliderSuperT.NoGroundCollider
        //{
        //    get { return 碰撞层_非地面; }
        //}
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
        bool mRayInGround = false;
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
        }
        // Update is called once per frame
        void Update()
        {
            //ThisGrivaty.Update();
            ThisMovable.Update();
            ThisAnim.Update();
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