using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerMovableInstance = IH2DCMovable<H2DMovableController>;
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
        PlayerMovableInstance mPlayerInstance = null;
        Vector3 mFaceDirection = Vector3.zero;
        Vector3 mMoveDirection = Vector3.zero;
        Vector3 mInAirVelocity = Vector3.zero;
        float mMoveSpeed = 0.0f;
    }
}