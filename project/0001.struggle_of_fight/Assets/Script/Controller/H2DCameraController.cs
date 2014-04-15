using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public class H2DCameraController : MonoBehaviour
    {
        public Object 宿主程序;
        // The distance in the x-z plane to the target
        public float mDistance = 7.0f;
        // the height we want the camera to be above the target
        public float mHeight = 3.0f;
        public float mAngularSmoothLag = 0.3f;
        public float mAngularMaxSpeed = 15.0f;
        public float mHeightSmoothLag = 0.3f;
        public float 水平平滑延迟 = 0.1f;
        public float mSnapSmoothLag = 0.2f;
        public float mSnapMaxSpeed = 720.0f;
        public float mClampHeadPositionScreenSpace = 0.75f;
        public float mLockCameraTimeout = 0.2f;
        public Collider 摄像机锁定左边界;
        public Collider 摄像机锁定右边界;
        public Collider 摄像机锁定上边界;
        public Collider 摄像机锁定下边界;
        public static float s_DefaultMainCameraViewPointSize = 3.71f;
        public static Vector2 s_DefaultScreenSize = new Vector3(1280, 768);
        void Awake()
        {
            mCamera = GameObject.Find("MainCamera").camera;
            if (!mCameraTransform && mCamera)
                mCameraTransform = mCamera.transform;
            if (!mCameraTransform)
            {
                Debug.Log("Please assign a camera to the H2DCamera script.");
                enabled = false;
            }
            Component componet = transform.GetComponent(宿主程序.name);
            if (null == componet)
            {
                Debug.LogError("H2DCameraController的宿主程序设置非法！");
                return;
            }
            mController = componet as IH2DCCamera;
            if (null != mController)
            {
                mCenterOffset = mController.Bounds.center - transform.position;
                mHeadOffset = mCenterOffset;
                mHeadOffset.y = mController.Bounds.max.y - transform.position.y;
            }
            else
                Debug.Log("Please assign a target to the camera that has a H2DCamera script attached.");
            Cut(transform, mCenterOffset);
        }
        void DebugDrawStuff()
        {
            Debug.DrawLine(transform.position, transform.position + mHeadOffset);
        }
        float AngleDistance(float a, float b)
        {
            a = Mathf.Repeat(a, 360);
            b = Mathf.Repeat(b, 360);
            return Mathf.Abs(b - a);
        }
        void Apply(Transform dummyTarget, Vector3 dummyCenter)
        {
            // Early out if we don't have a target
            if (null == mController)
                return;
            var targetCenter = transform.position + mCenterOffset;
            //var targetHead = mTarget.position + mHeadOffset;
            DebugDrawStuff();
            // Calculate the current & target rotation angles
            var originalTargetAngle = transform.eulerAngles.y;
            var currentAngle = mCameraTransform.eulerAngles.y;
            // Adjust real target angle when camera is locked
            var targetAngle = originalTargetAngle;
            // Normal camera motion
            if (mController.LockCameraTimer < mLockCameraTimeout)
                targetAngle = currentAngle;
            // Lock the camera when moving backwards!
            // * It is really confusing to do 180 degree spins when turning around.
            if (AngleDistance(currentAngle, targetAngle) > 160/* && mController.IsMovingBackwards ()*/)
                targetAngle += 180;
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref mAngleVelocity, mAngularSmoothLag, mAngularMaxSpeed);
            mTargetHeight = targetCenter.y + mHeight;
            // Damp the height
            var currentHeight = mCameraTransform.position.y;
            currentHeight = Mathf.SmoothDamp(currentHeight, mTargetHeight, ref mHeightVelocity, mHeightSmoothLag);
            var currentWidth = mCameraTransform.position.x;
            currentWidth = Mathf.SmoothDamp(currentWidth, targetCenter.x, ref mWidthVelocity, 水平平滑延迟);
            // Convert the angle into a rotation, by which we then reposition the camera
            var currentRotation = Quaternion.Euler(0, 0, 0);
            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            var cameraPosition = mCameraTransform.position;
            cameraPosition = targetCenter;
            cameraPosition += currentRotation * Vector3.back * mDistance;
            // Set the height of the cameraa
            cameraPosition.y = currentHeight;
            cameraPosition.x = currentWidth;
            //mCameraTransform.position = cameraPosition;
            // bound locket
            float cameraH2WScale = mCamera.pixelWidth / mCamera.pixelHeight;
            float leftEdge = 摄像机锁定左边界.bounds.max.x;
            float rightEdge = 摄像机锁定右边界.bounds.min.x;
            float topEdge = 摄像机锁定上边界.bounds.min.y;
            float bottomEdge = 摄像机锁定下边界.bounds.max.y;
            float camLeftEdge = cameraPosition.x - (mCamera.orthographicSize * cameraH2WScale);
            float camRightEdge = cameraPosition.x + (mCamera.orthographicSize * cameraH2WScale);
            float camTopEdge = cameraPosition.y + mCamera.orthographicSize;
            float camBottomEdge = cameraPosition.y - mCamera.orthographicSize;
            if (camLeftEdge < leftEdge)
                cameraPosition.x = leftEdge + (mCamera.orthographicSize * cameraH2WScale);
            else if (camRightEdge > rightEdge)
                cameraPosition.x = rightEdge - (mCamera.orthographicSize * cameraH2WScale);
            if (camBottomEdge < bottomEdge)
                cameraPosition.y = bottomEdge + mCamera.orthographicSize;
            else if (camTopEdge > topEdge)
                cameraPosition.y = topEdge - mCamera.orthographicSize;
            mCameraTransform.position = cameraPosition;
        }
        void LateUpdate()
        {
            Apply(transform, Vector3.zero);
        }
        void Cut(Transform dummyTarget, Vector3 dummyCenter)
        {
            var oldHeightSmooth = mHeightSmoothLag;
            var oldSnapMaxSpeed = mSnapMaxSpeed;
            var oldSnapSmooth = mSnapSmoothLag;

            mSnapMaxSpeed = 10000;
            mSnapSmoothLag = 0.001f;
            mHeightSmoothLag = 0.001f;

            //mSnap = true;
            Apply(transform, Vector3.zero);

            mHeightSmoothLag = oldHeightSmooth;
            mSnapMaxSpeed = oldSnapMaxSpeed;
            mSnapSmoothLag = oldSnapSmooth;
        }

        void SetUpRotation(Vector3 centerPos, Vector3 headPos)
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
        Camera mCamera;
        Transform mCameraTransform;
        private Vector3 mHeadOffset = Vector3.zero;
        private Vector3 mCenterOffset = Vector3.zero;
        private float mHeightVelocity = 0.0f;
        private float mWidthVelocity = 0.0f;
        private float mAngleVelocity = 0.0f;
        //private bool mSnap = false;
        private IH2DCCamera mController = null;
        private float mTargetHeight = 100000.0f;
    }
}