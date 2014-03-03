using UnityEngine;
using System.Collections;

[AddComponentMenu("自定义/横版2D/主角控制器")]
public class Horizontal2DController : MonoBehaviour
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
    public GameObject mWeaponPhysicsObject = null;

    public float mCharacterGravity = 20.0f;
    public float mSpeedSmoothing = 10.0f;
    public float mRotateSpeed = 500.0f;
    public float mTrotAfterSeconds = 3.0f;
    public float mTrotSpeed = 4.0f;
    public float mRunSpeed = 6.0f;
    public float mInAirControlAcceleration = 3.0f;
    public float mJumpHeight = 0.5f;
    public float mJumpRepeatTime = 0.05f;
    public float mJumpTimeout = 0.15f;
    public float mGroundedTimeout = 0.25f;

    public float mJumpAnimSpeed = 1.15f;
    public float mRunAnimSpeed = 1.0f;
    public float mInLandAnimSpeed = 0.1f;
    public bool mCanJump = true;

    public float mAttack1AnimSpeed = 0.5f;
    public float mAttack2AnimSpeed = 0.5f;
    public float mAttack3AnimSpeed = 0.5f;
    public int mAttackComboMaxNum = 3;
    public float mAttackComboTimeout = 1.0f;

    public float mAssaultAnimSpeed = 1.0f;
    public float mAssaultSkillMaxTime = 0.3f;
    public float mAssaultSkillMoveSpeed = 15.0f;

    public float mHelfCutAnimSpeed = 0.3f;

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
        mMoveDirection = transform.TransformDirection(Vector3.right);
        mPlayingAnim = GetComponent<Animation>();
        mMeshPhysicsCollider = mWeaponPhysicsObject.GetComponent<MeshPhysicsCollider>();
        if (!mPlayingAnim)
            Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
        if (!mAnim14_Idel)
        {
            mPlayingAnim = null;
            Debug.Log("No idle animation found. Turning off animations.");
        }
    }

    public static float CalculateJumpVerticalSpeed(float jumpHeight)
    {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
        var thisInstance = GameObject.FindObjectOfType<Horizontal2DController>();
	    return Mathf.Sqrt(2.0f * jumpHeight * thisInstance.mCharacterGravity);
    }
    public bool Grounded
    {
        get
        {
            if ((mCollisionFlags & CollisionFlags.CollidedBelow) != 0)
            {
                return mController.isGrounded;
            }
            return false;
        }
    }
    public bool Moving
    {
        get { return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f; }
    }
    public bool Jumping
    {
        set { mJumping = value; }
        get { return mJumping; }
    }
    public float LockCameraTimer
    {
        get { return mLockCameraTimer; }
    }
    public Animation Animations
    {
        get { return mPlayingAnim; }
    }
    public int AttackComboNum
    {
        get { return mAttackComboNum; }
    }
    public CharacterState ActionState
    {
        get { return mState; }
    }

    public delegate bool SkillEventHandler(Object sender, int skillID);
    public event SkillEventHandler OnSkillBegining;

    public void DoAttack()
    {
        if (CharacterState.Idel == mState || CharacterState.Running == mState)
        {
            mMoveSpeed = 0.0f;
            if (mAttackComboNum >= mAttackComboMaxNum)
                mAttackComboNum = 0;
            int nowNum = (int)CharacterState.Attack01;
            mState = (CharacterState)(nowNum + mAttackComboNum++);
            mAttackComboTimer = mAttackComboTimeout;
            if (mPlayingAnim.IsPlaying(mAnim04_Attack01.name))
                mPlayingAnim.Stop(mAnim04_Attack01.name);
            if (mPlayingAnim.IsPlaying(mAnim05_Attack02.name))
                mPlayingAnim.Stop(mAnim05_Attack02.name);
            if (mPlayingAnim.IsPlaying(mAnim06_Attack03.name))
                mPlayingAnim.Stop(mAnim06_Attack03.name);
            mMeshPhysicsCollider.ActivePhysics(true);
        }
    }
    public void DoJump()
    {
        if (CharacterState.Idel == mState || CharacterState.Running == mState)
        {
            mState = CharacterState.Jumpup;
            mJumping = true;
            mLastJumpButtonTime = Time.time;
            if (mPlayingAnim.IsPlaying(mAnim13_JumpDown.name))
                mPlayingAnim.Stop(mAnim13_JumpDown.name);
        }
    }
    public void DoAssaultSkill()
    {
        if (CharacterState.Idel == mState || CharacterState.Running == mState)
        {
            mMoveSpeed = 0.0f;
            mState = CharacterState.Skill01;
            mAssaultSkillTimer = mAssaultSkillMaxTime;
            if (mPlayingAnim.IsPlaying(mAnim03_Skill01.name))
                mPlayingAnim.Stop(mAnim03_Skill01.name);
            GameObject assaultEffect = GameObject.Find("Effect.Assault");
            if (assaultEffect != null)
            {
                ImageFrameAnim pAnim = assaultEffect.GetComponent<ImageFrameAnim>();
                pAnim.Play();
            }
            mMeshPhysicsCollider.ActivePhysics(true);
        }
    }
    public void DoHelfCutSkill()
    {
        if (CharacterState.Idel == mState || CharacterState.Running == mState)
        {
            mMoveSpeed = 0.0f;
            mState = CharacterState.Skill02;
            if (mPlayingAnim.IsPlaying(mAnim04_Attack01.name))
                mPlayingAnim.Stop(mAnim04_Attack01.name);
            GameObject hmcEffect = GameObject.Find("Effect.HelfMoonCut");
            if (hmcEffect != null)
            {
                ColliderImageFrameAnim pAnim = hmcEffect.GetComponent<ColliderImageFrameAnim>();
                pAnim.Play();
                pAnim.ActiveCollider = true;
            }
        }
    }
    public void OnAnimationOvered(CharacterState state)
    {
        if (mState == state)
        {
            switch (state)
            {
                case CharacterState.Attack01:
                case CharacterState.Attack02:
                case CharacterState.Attack03:
                //case CharacterState.JumpDown:
                    {
                        if (Moving)
                            mState = CharacterState.Running;
                        else
                            mState = CharacterState.Idel;
                    }
                    break;
                case CharacterState.Jumpup:
                    {
                        mState = CharacterState.JumpAir;
                    }
                    break;
            }
            switch (state)
            {
                case CharacterState.Attack01:
                case CharacterState.Attack02:
                case CharacterState.Attack03:
                    mMeshPhysicsCollider.ActivePhysics(false);
                    break;
            }
        }
        else if ((mState == CharacterState.Skill02) && (state == CharacterState.Attack01))
        {
            if (Moving)
                mState = CharacterState.Running;
            else
                mState = CharacterState.Idel;
        }
        else
        {
            Debug.Log("unknow state in frame event");
        }
    }


    void UpdateSmoothedMovementDirection()
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
        var h = Input.GetAxisRaw("Horizontal");
        var wasMoving = mIsMoving;
        mIsMoving = Mathf.Abs(h) > 0.1;
        // Target direction relative to the camera
        var targetDirection = h * right;
        // Grounded controls
        if (Grounded)
        {
            // Lock camera for short period when transitioning moving & standing still
            mLockCameraTimer += Time.deltaTime;
            if (mIsMoving != wasMoving)
                mLockCameraTimer = 0.0f;
            // We store speed and direction seperately,
            // so that when the character stands still we still have a valid forward direction
            // moveDirection is always normalized, and we only update it if there is user input.
            if (targetDirection != Vector3.zero)
            {
                Vector3 oldDir = mMoveDirection;
                //mMoveDirection = Vector3.RotateTowards(mMoveDirection, targetDirection, mRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
                mMoveDirection = targetDirection;
                if (mMoveDirection.z != 0.0f)
                {
                    mMoveDirection.z = 0.0f;
                }
                mMoveDirection = mMoveDirection.normalized;
            }
            // Smooth the speed based on the current target direction
            var curSmooth = mSpeedSmoothing * Time.deltaTime;
            // Choose target speed
            //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
            var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
            // Pick speed modifier
            targetSpeed *= mRunSpeed;
            mState = CharacterState.Running;
            mMoveSpeed = Mathf.Lerp(mMoveSpeed, targetSpeed, curSmooth);
        }
        else
        {
            // In air controls
            // Lock camera while in air
            if (mJumping)
                mLockCameraTimer = 0.0f;
            if (mIsMoving)
                mInAirVelocity += targetDirection.normalized * Time.deltaTime * mInAirControlAcceleration;
        }
    }
    void Start()
    {
    }
    void ApplyGravity()
    {
        // Apply gravity
        var jumpButton = Input.GetButton("Jump");
        // When we reach the apex of the jump we send out a message
        if (mJumping && mVerticalSpeed <= 0.0)
        {
            //jumpingReachedApex = true;
            SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
        }
        if (Grounded)
            mVerticalSpeed = 0.0f;
        else
            mVerticalSpeed -= mCharacterGravity * Time.deltaTime;
    }
    void ApplyJumping()
    {
        // Prevent jumping too fast after each other
        if (mLastJumpTime + mJumpRepeatTime > Time.time)
            return;
        if (Grounded)
        {
            // Jump
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing		
            if (mCanJump && Time.time < mLastJumpButtonTime + mJumpTimeout)
            {
                mVerticalSpeed = CalculateJumpVerticalSpeed(mJumpHeight);
                SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
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
                mPlayingAnim.CrossFade(mAnim02_Jumpup.name, 0.1f);
            }
            else if (mState == CharacterState.JumpAir)
            {
                mPlayingAnim[mAnim12_JumpAir.name].speed = mJumpAnimSpeed;
                mPlayingAnim[mAnim12_JumpAir.name].wrapMode = WrapMode.ClampForever;
				mPlayingAnim.CrossFade(mAnim12_JumpAir.name, 0.1f);
			}
			else if (mState == CharacterState.JumpDown)
            {
                mPlayingAnim[mAnim13_JumpDown.name].speed = mJumpAnimSpeed;
                mPlayingAnim[mAnim13_JumpDown.name].wrapMode = WrapMode.ClampForever;
				mPlayingAnim.CrossFade(mAnim13_JumpDown.name, 0.2f);
			}
			else if (mState == CharacterState.Attack01)
            {
                mPlayingAnim[mAnim04_Attack01.name].speed = mAttack1AnimSpeed;
                mPlayingAnim[mAnim04_Attack01.name].wrapMode = WrapMode.ClampForever;
				mPlayingAnim.CrossFade(mAnim04_Attack01.name, 0.1f);
			}
			else if (mState == CharacterState.Attack02)
            {
                mPlayingAnim[mAnim05_Attack02.name].speed = mAttack2AnimSpeed;
                mPlayingAnim[mAnim05_Attack02.name].wrapMode = WrapMode.ClampForever;
				mPlayingAnim.CrossFade(mAnim05_Attack02.name, 0.1f);
			}
			else if (mState == CharacterState.Attack03)
            {
                mPlayingAnim[mAnim06_Attack03.name].speed = mAttack3AnimSpeed;
                mPlayingAnim[mAnim06_Attack03.name].wrapMode = WrapMode.ClampForever;
				mPlayingAnim.CrossFade(mAnim06_Attack03.name, 0.1f);
			}
			else if (mState == CharacterState.Skill01)
            {
                mPlayingAnim[mAnim03_Skill01.name].speed = mAssaultAnimSpeed;
                mPlayingAnim[mAnim03_Skill01.name].wrapMode = WrapMode.ClampForever;
				mPlayingAnim.CrossFade(mAnim03_Skill01.name, 0.1f);
			}
			else if (mState == CharacterState.Skill02)
            {
                mPlayingAnim[mAnim04_Attack01.name].speed = mHelfCutAnimSpeed;
                mPlayingAnim[mAnim04_Attack01.name].wrapMode = WrapMode.ClampForever;
				mPlayingAnim.CrossFade(mAnim04_Attack01.name, 0.1f);
			}
			else
            {
                if (mController.velocity.sqrMagnitude < 0.1)
                {
                    mPlayingAnim[mAnim14_Idel.name].speed = mInLandAnimSpeed;
					mPlayingAnim.CrossFade(mAnim14_Idel.name, 0.1f);
				}
				else
                {
                    if (mState == CharacterState.Running)
                    {
                        mPlayingAnim[mAnim08_Running.name].speed = Mathf.Clamp(mController.velocity.magnitude, 0.0f, mRunAnimSpeed);
						mPlayingAnim.CrossFade(mAnim08_Running.name, 0.1f);
					}
				}
            }
        }
    }
    void UpdateAssault()
    {
        if (mState == CharacterState.Skill01)
            mAssaultMoveSpeed = mAssaultSkillMoveSpeed;
        else
            mAssaultMoveSpeed = 0.0f;
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            DoJump();
        if (mState == CharacterState.Running || mState == CharacterState.Idel)
            UpdateSmoothedMovementDirection();
        UpdateAssault();
        // Apply gravity
        // - extra power jump modifies gravity
        // - controlledDescent mode modifies gravity
        ApplyGravity();
        // Apply jumping logic
        ApplyJumping();
        // Calculate actual motion
        var movement = mMoveDirection * (mMoveSpeed + mAssaultMoveSpeed) + new Vector3(0, mVerticalSpeed, 0) + mInAirVelocity;
        movement *= Time.deltaTime;
        // Move the controller
        mCollisionFlags = mController.Move(movement);
        // ANIMATION sector
        AnimationSector();
        // Set rotation to the move direction
        if (Grounded)
        {
            transform.rotation = Quaternion.LookRotation(mMoveDirection);
            // We are in jump mode but just became grounded
            //mLastGroundedTime = Time.time;
            mInAirVelocity = Vector3.zero;
            //if (mJumping && mState == CharacterState.JumpAir)
            //{
            //    mJumping = false;
            //    mState = CharacterState.JumpDown;
            //    SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
            //}
            if (mJumping && (mState == CharacterState.JumpAir || mState == CharacterState.JumpDown))
            {
                if (Moving)
                    mState = CharacterState.Running;
                else
                    mState = CharacterState.Idel;
            }
        }
        else
        {
            if (mJumping && mState == CharacterState.JumpAir && mVerticalSpeed < 0.0f)
                mState = CharacterState.JumpDown;
            var xMove = movement;
            xMove.y = 0;
            xMove.z = 0;
            if (xMove.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(xMove);
            }
        }
        mAttackComboTimer -= Time.deltaTime;
        if (mAttackComboTimer <= 0.0f)
            mAttackComboNum = 0;
        if (mState == CharacterState.Skill01)
        {
            mAssaultSkillTimer -= Time.deltaTime;
            if (mAssaultSkillTimer <= 0.0f)
            {
                if (Moving)
                    mState = CharacterState.Running;
                else
                    mState = CharacterState.Idel;
            }
        }
    }

    void FixedUpdate()
    {

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

    // How high do we jump when pressing jump and letting go immediately
    CharacterController mController = null;
    CollisionFlags mCollisionFlags;
    Animation mPlayingAnim = null;
    MeshPhysicsCollider mMeshPhysicsCollider = null;
    CharacterState mState = CharacterState.Idel;
	Vector3 mMoveDirection = Vector3.zero;
	float mMoveSpeed = 0.0f;
	float mVerticalSpeed = 0.0f;
    Vector3 mInAirVelocity = Vector3.zero;
    bool mJumping = false;
    bool mIsMoving = false;
    float mLockCameraTimer = 0.0f;
    float mLastJumpButtonTime = 0.0f;
    float mLastJumpTime = 0.0f;

    int mAttackComboNum = 0;
    float mAttackComboTimer = 0.0f;

    float mAssaultMoveSpeed = 0.0f;
    float mAssaultSkillTimer = 0.0f;
}