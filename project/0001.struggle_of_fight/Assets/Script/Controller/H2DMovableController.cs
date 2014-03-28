using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerMovableInstance = IH2DCMovable<H2DMovableController>;
    using PlayerGravityInstance = IH2DCGravity<H2DGravityController>;
    using PlayerAnimInstance = IH2DCAnimation<H2DAnimController>;
    public class H2DMovableController
    {
        public H2DMovableController(PlayerMovableInstance instance)
        {
            mPlayerInstance = instance;
        }
        public static float CalculateJumpVerticalSpeed(float jumpHeight, float grivaty)
        {
            return Mathf.Sqrt(2.0f * jumpHeight * grivaty);
        }
        public bool IsMoving
        {
            get { return mIsMoving; }
        }
        public Vector3 FaceDirection
        {
            set { mFaceDirection = value; }
            get { return mFaceDirection; }
        }
        public Vector3 LastMovement
        {
            get { return mLastMovement; }
        }
        public float MoveSpeed
        {
            set { mMoveSpeed = value; }
            get { return mMoveSpeed; }
        }
        public float Deceleration
        {
            set { mDeceleration = value; }
        }
        public bool Init()
        {
            mMoveDirection = mFaceDirection;
            return true;
        }
        public bool UpdateSmoothedMovementDirection(bool grounded, Transform trans)
        {
            var cameraTransform = Camera.main.transform;
            // Forward vector relative to the camera along the x-z plane	
            var forward = cameraTransform.TransformDirection(Vector3.right);
            forward.y = 0;
            forward.z = 0;
            forward = forward.normalized;
            // Right vector relative to the camera
            // Always orthogonal to the forward vector
            var right = cameraTransform.TransformDirection(Vector3.right);
            var h = mPlayerInstance.InputSpeedX;
            mIsMoving = Mathf.Abs(h) > 0.1f;
            if (h > 0.1f)
            {
                mFaceDirection = cameraTransform.TransformDirection(Vector3.right);
            }
            else if (h < -0.1f)
            {
                mFaceDirection = cameraTransform.TransformDirection(Vector3.left);
            }
            // Target direction relative to the camera
            var targetDirection = h * right;
            // Grounded controls
            var grivatyInst = mPlayerInstance as PlayerGravityInstance;
            var animInst = mPlayerInstance as PlayerAnimInstance;
            if (null == grivatyInst || null == animInst)
                return false;
            if (grounded)
            {
                // We store speed and direction seperately,
                // so that when the character stands still we still have a valid forward direction
                // moveDirection is always normalized, and we only update it if there is user input.
                if (targetDirection != Vector3.zero)
                {
                    //Vector3 oldDir = mMoveDirection;
                    //mMoveDirection = Vector3.RotateTowards(mMoveDirection, targetDirection, mRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
                    mMoveDirection = targetDirection;
                    if (mMoveDirection.z != 0.0f)
                        mMoveDirection.z = 0.0f;
                    mMoveDirection = mMoveDirection.normalized;
                }
                // Smooth the speed based on the current target direction
                var curSmooth = mPlayerInstance.SpeedSmoothing * Time.deltaTime;
                // Choose target speed
                //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
                var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
                // Pick speed modifier
                targetSpeed *= mPlayerInstance.SpeedScaleX;
                mMoveSpeed = Mathf.Lerp(mMoveSpeed, targetSpeed, curSmooth);
            }
            else
            {
                // In air controls
                if (mIsMoving)
                    mInAirVelocity += targetDirection.normalized * Time.deltaTime * mPlayerInstance.InAirControlAcceleration;
            }
            return true;
        }
        public float UpdateVerticalMovement(bool grounded, float grivaty)
        {
            if (grounded)
                return CalculateJumpVerticalSpeed(mPlayerInstance.JumpHeight, grivaty);
            return 0.0f;
        }
        public bool Movement(float addHorSpeed, float verticalSpeed, ref Vector3 outPos)
        {
            mLastMovement = mMoveDirection * (mMoveSpeed + addHorSpeed + mDeceleration) + new Vector3(0, verticalSpeed, 0) + mInAirVelocity;
            mLastMovement *= Time.deltaTime;
            mDeceleration -= mDeceleration * Time.deltaTime;
            // 这里先计算好位置，等经过GroundMoveTest之后才知道会不会被地面挡住
            outPos += mLastMovement;
            return true;
        }
        public bool MovementAfter(bool grounded, Vector3 fixedPosition, Transform trans)
        {
            // 经过GroundMoveTest之后如果被挡住会将Movement中计算的位置修正为地面的位置
            mLastMovement = fixedPosition - trans.position;
            // 计算差值后移动过去
            mPlayerInstance.Controller.Move(mLastMovement);
            //trans.position = fixedPosition;
            bool needRota = false;
            if (grounded)
            {
                //mClobberSpeed = Vector3.zero;
                needRota = true;
                // We are in jump mode but just became grounded
                mInAirVelocity = Vector3.zero;
            }
            else
            {
                var xMove = mLastMovement;
                xMove.y = 0;
                xMove.z = 0;
                needRota = xMove.sqrMagnitude > 0.001f;
            }
            if (needRota)
            {
                if (mPlayerInstance.UsedModelFlipX)
                {
                    trans.rotation = Quaternion.LookRotation(Vector3.right);
                    Vector3 scale = trans.localScale;
                    scale.z = mFaceDirection.x * System.Math.Abs(scale.z);
                    trans.localScale = scale;
                }
                else
                    trans.rotation = Quaternion.LookRotation(mFaceDirection);
            }
            return true;
        }
        PlayerMovableInstance mPlayerInstance = null;
        Vector3 mFaceDirection = Vector3.zero;
        Vector3 mMoveDirection = Vector3.zero;
        Vector3 mInAirVelocity = Vector3.zero;
        float mDeceleration = 0.0f;
        float mMoveSpeed = 0.0f;
        bool mIsMoving = false;
        Vector3 mLastMovement = Vector3.zero;
    }
}