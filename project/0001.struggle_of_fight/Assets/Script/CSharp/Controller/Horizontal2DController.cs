using UnityEngine;
using System.Collections;

[AddComponentMenu("Extern/Horizontal2DController")]
public class Horizontal2DController : MonoBehaviour
{
    public AnimationClip mAnim02_Jumpup;
    public AnimationClip mAnim03_Take;
    public AnimationClip mAnim04_Attack01;
    public AnimationClip mAnim05_Attack02;
    public AnimationClip mAnim06_Attack03;
    public AnimationClip mAnim07_AirAttack;
    public AnimationClip mAnim08_Running;
    public AnimationClip mAnim09_BeAttack;
    public AnimationClip mAnim10_Death;
    public AnimationClip mAnim11_Take;
    public AnimationClip mAnim12_JumpAir;
    public AnimationClip mAnim13_JumpDown;
    public AnimationClip mAnim14_Idel;

    public float mCharacterGravity = 20.0f;
    public float mMoveSpeed = 0.0f;
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
    public float mInLandAnimSpeed = 1.0f;
    public bool mCanJump = true;

    // How high do we jump when pressing jump and letting go immediately
    public CollisionFlags mCollisionFlags;

    enum CharacterState : byte
    {
        Jumpup = 2,
        Take03 = 3,
        Attack01 = 4,
        Attack02 = 5,
        Attack03 = 6,
        AirAttack = 7,
        Running = 8,
        BeAttack = 9,
        Death = 10,
        Take11 = 11,
        JumpAir = 12,
        JumpDown = 13,
        Idel = 14
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
        get { return (mCollisionFlags & CollisionFlags.CollidedBelow) != 0; }
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

    void Awake()
    {
        mMoveDirection = transform.TransformDirection(Vector3.right);
        mPlayingAnim = GetComponent<Animation>();
        if (!mPlayingAnim)
            Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
        if (!mAnim14_Idel)
        {
            mPlayingAnim = null;
            Debug.Log("No idle animation found. Turning off animations.");
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
    void AnimationSector(CharacterController controller)
    {
        // ANIMATION sector
        if (mPlayingAnim)
        {
            if (mState == CharacterState.Jumpup)
            {
                mPlayingAnim[mAnim12_JumpAir.name].speed = mJumpAnimSpeed;
                mPlayingAnim[mAnim12_JumpAir.name].wrapMode = WrapMode.ClampForever;
                mPlayingAnim.CrossFade(mAnim12_JumpAir.name);
            }
            else
            {
                if (controller.velocity.sqrMagnitude < 0.1)
                {
                    mPlayingAnim[mAnim14_Idel.name].speed = 10.0f;
                    mPlayingAnim.CrossFade(mAnim14_Idel.name);
                }
                else
                {
                    if (mState == CharacterState.Running)
                    {
                        mPlayingAnim[mAnim08_Running.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, mRunAnimSpeed);
                        mPlayingAnim.CrossFade(mAnim08_Running.name);
                    }
                }
            }
        }
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            mLastJumpButtonTime = Time.time;
        }
        UpdateSmoothedMovementDirection();
        // Apply gravity
        // - extra power jump modifies gravity
        // - controlledDescent mode modifies gravity
        ApplyGravity();
        // Apply jumping logic
        ApplyJumping();
        // Calculate actual motion
        var movement = mMoveDirection * mMoveSpeed + new Vector3(0, mVerticalSpeed, 0) + mInAirVelocity;
        movement *= Time.deltaTime;
        // Move the controller
        var controller = GetComponent<CharacterController>();
        mCollisionFlags = controller.Move(movement);
        // ANIMATION sector
        AnimationSector(controller);
        // Set rotation to the move direction
        if (Grounded)
        {
            transform.rotation = Quaternion.LookRotation(mMoveDirection);
            // We are in jump mode but just became grounded
            //mLastGroundedTime = Time.time;
            mInAirVelocity = Vector3.zero;
            if (mJumping)
            {
                mJumping = false;
                SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            var xMove = movement;
            xMove.y = 0;
            xMove.z = 0;
            if (xMove.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(xMove);
            }
        }
    }

    void FixedUpdate()
    {

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

    }

    private Animation mPlayingAnim = null;
    private CharacterState mState = CharacterState.Idel;
    private Vector3 mMoveDirection = Vector3.zero;
    private float mVerticalSpeed = 0.0f;
    private Vector3 mInAirVelocity = Vector3.zero;
    private bool mJumping = false;
    private bool mIsMoving = false;
    private float mLockCameraTimer = 0.0f;
    float mLastJumpButtonTime = 0.0f;
    float mLastJumpTime = 0.0f;
}