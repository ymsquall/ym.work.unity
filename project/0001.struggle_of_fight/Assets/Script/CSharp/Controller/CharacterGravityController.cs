using UnityEngine;
using System.Collections;

public class CharacterGravityController : MonoBehaviour
{
    public float mCharacterGravity = 20.0f;
    public float mGroundedTimeout = 0.25f;
    public bool mCanJump = true;
    public float mJumpHeight = 0.5f;
    public float mJumpRepeatTime = 0.05f;
    public float mJumpTimeout = 0.15f;
    public float mInAirControlAcceleration = 3.0f;
    public float mRunSpeed = 6.0f;
    public Vector3 mClobberMoveSpeed = new Vector3(2.0f, 5.0f, 0.0f);

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
    public static float CalculateJumpVerticalSpeed(float jumpHeight)
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        var thisInstance = GameObject.FindObjectOfType<Horizontal2DController>();
        return Mathf.Sqrt(2.0f * jumpHeight * thisInstance.mCharacterGravity);
    }
    void Awake()
    {
        mController = GetComponent<CharacterController>();
        mFaceDirection = transform.TransformDirection(Vector3.right);
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
    Vector3 DoMoveController()
    {
        // Calculate actual motion
        var movement = mMoveDirection * (mMoveSpeed + mClobberSpeed.x) + new Vector3(0, (mVerticalSpeed + mClobberSpeed.y), 0) + mInAirVelocity;
        movement *= Time.deltaTime;
        // Move the controller
        mCollisionFlags = mController.Move(movement);
        return movement;
    }
	public void DoClobber(float clobberDirX)
    {
        mMoveDirection.x = clobberDirX;
        mMoveDirection.y = mMoveDirection.z = 0.0f;
        mMoveDirection.Normalize();
		mClobberSpeed = mClobberMoveSpeed;
        mFaceDirection.x = -clobberDirX;
        mFaceDirection.Normalize();
    }
	// Update is called once per frame
	void Update ()
    {
        ApplyGravity();
        // Apply jumping logic
        ApplyJumping();
        // move
        var movement = DoMoveController();
        // Set rotation to the move direction
        if (Grounded)
        {
            mClobberSpeed = Vector3.zero;
            transform.rotation = Quaternion.LookRotation(mFaceDirection);
            // We are in jump mode but just became grounded
            //mLastGroundedTime = Time.time;
            mInAirVelocity = Vector3.zero;
        }
        else
        {
            var xMove = movement;
            xMove.y = 0;
            xMove.z = 0;
            if (xMove.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(mFaceDirection);
            }
        }
	}

    // How high do we jump when pressing jump and letting go immediately
    CharacterController mController;
    CollisionFlags mCollisionFlags;
    Vector3 mFaceDirection = Vector3.zero;
    Vector3 mMoveDirection = Vector3.zero;
    float mVerticalSpeed = 0.0f;
    Vector3 mInAirVelocity = Vector3.zero;
    bool mJumping = false;
    float mLastJumpButtonTime = 0.0f;
    float mLastJumpTime = 0.0f;
    float mMoveSpeed = 0.0f;
    Vector3 mClobberSpeed = Vector3.zero;
}
