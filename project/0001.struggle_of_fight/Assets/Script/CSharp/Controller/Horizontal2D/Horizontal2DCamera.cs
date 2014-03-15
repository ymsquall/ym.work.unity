using UnityEngine;
using System.Collections;

[AddComponentMenu("自定义/横版2D/摄像机跟随主角")]
public class Horizontal2DCamera : MonoBehaviour
{
    public Transform mCameraTransform;
    // The distance in the x-z plane to the target
    public float mDistance = 7.0f;
    // the height we want the camera to be above the target
    public float mHeight = 3.0f;
    public float mAngularSmoothLag = 0.3f;
    public float mAngularMaxSpeed = 15.0f;
    public float mHeightSmoothLag = 0.3f;
    public float mSnapSmoothLag = 0.2f;
    public float mSnapMaxSpeed = 720.0f;
    public float mClampHeadPositionScreenSpace = 0.75f;
    public float mLockCameraTimeout = 0.2f;
    public static float s_DefaultMainCameraViewPointSize = 3.71f;
    public static Vector2 s_DefaultScreenSize = new Vector3(1280,768);

    void Awake ()
    {
	    if(!mCameraTransform && Camera.main)
		    mCameraTransform = Camera.main.transform;
	    if(!mCameraTransform)
        {
		    Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
		    enabled = false;	
	    }
	    mTarget = transform;
	    if (mTarget)
	    {
		    mController = mTarget.GetComponent<Horizontal2DController>();
	    }
	
	    if (mController)
	    {
		    var characterController = mTarget.collider as CharacterController;
            mCenterOffset = characterController.bounds.center - mTarget.position;
		    mHeadOffset = mCenterOffset;
            mHeadOffset.y = characterController.bounds.max.y - mTarget.position.y;
	    }
	    else
            Debug.Log("Please assign a target to the camera that has a ThirdPersonController script attached.");
        Cut(mTarget, mCenterOffset);
        //
        //Vector2 screenScale = new Vector2((float)Screen.width / s_DefaultScreenSize.x, (float)Screen.height / s_DefaultScreenSize.y);
        //Vector2 screenScale = new Vector2((float)Screen.currentResolution.width / s_DefaultScreenSize.x, (float)Screen.currentResolution.height / s_DefaultScreenSize.y);
        //float viewPointSize = s_DefaultMainCameraViewPointSize * screenScale.x;
        //Debug.Log(string.Format("screen size = ({1}, {2}), viewPointSize = {0}", viewPointSize, Screen.currentResolution.width, Screen.currentResolution.height));
        //foreach (var cam in Camera.allCameras)
        //{
        //    cam.aspect = Screen.width / Screen.height;
        //    cam.orthographicSize = viewPointSize;
        //}
    }

    void DebugDrawStuff ()
    {
        Debug.DrawLine(mTarget.position, mTarget.position + mHeadOffset);
    }

    float AngleDistance (float a, float b)
    {
	    a = Mathf.Repeat(a, 360);
	    b = Mathf.Repeat(b, 360);
	    return Mathf.Abs(b - a);
    }

    void Apply (Transform dummyTarget, Vector3 dummyCenter)
    {
	    // Early out if we don't have a target
	    if (!mController)
		    return;
        var targetCenter = mTarget.position + mCenterOffset;
        //var targetHead = mTarget.position + mHeadOffset;
        DebugDrawStuff();
	    // Calculate the current & target rotation angles
        var originalTargetAngle = mTarget.eulerAngles.y;
	    var currentAngle = mCameraTransform.eulerAngles.y;
	    // Adjust real target angle when camera is locked
	    var targetAngle = originalTargetAngle; 
	    // When pressing Fire2 (alt) the camera will snap to the target direction real quick.
	    // It will stop snapping when it reaches the target
	    if (Input.GetButton("Fire2"))
		    mSnap = true;
        if (mSnap)
        {
            // We are close to the target, so we can stop snapping now!
            if (AngleDistance(currentAngle, originalTargetAngle) < 3.0)
                mSnap = false;
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref mAngleVelocity, mSnapSmoothLag, mSnapMaxSpeed);
        }
        else
        {
            // Normal camera motion
            if (mController.LockCameraTimer < mLockCameraTimeout)
            {
                targetAngle = currentAngle;
            }

            // Lock the camera when moving backwards!
            // * It is really confusing to do 180 degree spins when turning around.
            if (AngleDistance(currentAngle, targetAngle) > 160/* && mController.IsMovingBackwards ()*/)
                targetAngle += 180;
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref mAngleVelocity, mAngularSmoothLag, mAngularMaxSpeed);
        }
	    // When jumping don't move camera upwards but only down!
        //if (mController.Jumping)
        //{
        //    // We'd be moving the camera upwards, do that only if it's really high
        //    var newTargetHeight = targetCenter.y + mHeight;
        //    if (newTargetHeight < mTargetHeight || newTargetHeight - mTargetHeight > 5)
        //        mTargetHeight = targetCenter.y + mHeight;
        //}
        //else
        {
            // When walking always update the target height
		    mTargetHeight = targetCenter.y + mHeight;
	    }
	    // Damp the height
	    var currentHeight = mCameraTransform.position.y;
	    currentHeight = Mathf.SmoothDamp (currentHeight, mTargetHeight, ref mHeightVelocity, mHeightSmoothLag);
	    // Convert the angle into a rotation, by which we then reposition the camera
	    var currentRotation = Quaternion.Euler (0, 0, 0);
	    // Set the position of the camera on the x-z plane to:
	    // distance meters behind the target
        var cameraPosition = mCameraTransform.position;
        cameraPosition = targetCenter;
        cameraPosition += currentRotation * Vector3.back * mDistance;
	    // Set the height of the camera
        cameraPosition.y = currentHeight;
        mCameraTransform.position = cameraPosition;
	    // Always look at the target
	    //SetUpRotation(targetCenter, targetHead);
    }

    void LateUpdate ()
    {
	    Apply (transform, Vector3.zero);
    }

    void Cut (Transform dummyTarget, Vector3 dummyCenter)
    {
	    var oldHeightSmooth = mHeightSmoothLag;
	    var oldSnapMaxSpeed = mSnapMaxSpeed;
	    var oldSnapSmooth = mSnapSmoothLag;

        mSnapMaxSpeed = 10000;
        mSnapSmoothLag = 0.001f;
        mHeightSmoothLag = 0.001f;
	
	    mSnap = true;
	    Apply (transform, Vector3.zero);

	    mHeightSmoothLag = oldHeightSmooth;
	    mSnapMaxSpeed = oldSnapMaxSpeed;
	    mSnapSmoothLag = oldSnapSmooth;
    }

    void SetUpRotation (Vector3 centerPos, Vector3 headPos)
    {
	    // Now it's getting hairy. The devil is in the details here, the big issue is jumping of course.
	    // * When jumping up and down we don't want to center the guy in screen space.
	    //  This is important to give a feel for how high you jump and avoiding large camera movements.
	    //   
	    // * At the same time we dont want him to ever go out of screen and we want all rotations to be totally smooth.
	    //
	    // So here is what we will do:
	    //
	    // 1. We first find the rotation around the y axis. Thus he is always centered on the y-axis
	    // 2. When grounded we make him be centered
	    // 3. When jumping we keep the camera rotation but rotate the camera to get him back into view if his head is above some threshold
	    // 4. When landing we smoothly interpolate towards centering him on screen
	    var cameraPos = mCameraTransform.position;
	    var offsetToCenter = centerPos - cameraPos;
	
	    // Generate base rotation only around y-axis
	    var yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));

	    var relativeOffset = Vector3.forward * mDistance + Vector3.down * mHeight;
        mCameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);

	    // Calculate the projected center position and top position in world space
        var centerRay = mCameraTransform.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1));
        var topRay = mCameraTransform.camera.ViewportPointToRay(new Vector3(0.5f, mClampHeadPositionScreenSpace, 1));

	    var centerRayPos = centerRay.GetPoint(mDistance);
        var topRayPos = topRay.GetPoint(mDistance);
	
	    var centerToTopAngle = Vector3.Angle(centerRay.direction, topRay.direction);
	
	    var heightToAngle = centerToTopAngle / (centerRayPos.y - topRayPos.y);

	    var extraLookAngle = heightToAngle * (centerRayPos.y - centerPos.y);
	    if (extraLookAngle < centerToTopAngle)
	    {
		    extraLookAngle = 0;
	    }
	    else
	    {
		    extraLookAngle = extraLookAngle - centerToTopAngle;
            mCameraTransform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
	    }
    }

    Vector3 GetCenterOffset()
    {
	    return mCenterOffset;
    }

    private Transform mTarget;
    private Vector3 mHeadOffset = Vector3.zero;
    private Vector3 mCenterOffset = Vector3.zero;
    private float mHeightVelocity = 0.0f;
    private float mAngleVelocity = 0.0f;
    private bool mSnap = false;
    private Horizontal2DController mController;
    private float mTargetHeight = 100000.0f; 
}