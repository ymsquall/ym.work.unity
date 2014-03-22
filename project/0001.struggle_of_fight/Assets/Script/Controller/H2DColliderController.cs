using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerColliderInstance = IH2DCCollider<H2DColliderController>;
    public class H2DColliderController
    {
        public H2DColliderController(PlayerColliderInstance instance)
        {
            mH2DCCollider = instance;
        }
        public bool Init()
        {
            return true;
        }
        public bool UpdateCreature(Transform trans)
        {
            mH2DCCollider.GroundCollider.transform.position -= mLastMovement;
            mH2DCCollider.NoGroundCollider.transform.position -= mLastMovement;
            if (mH2DCCollider.Grounded)
                mLastMovement.y = 0.0f;
            trans.position += mLastMovement;
            return true;
        }
        public bool Update(Vector3 movement)
        {
            if (mH2DCCollider.Grounded)
                movement.y = 0.0f;
            mLastMovement = movement;
            //mH2DCCollider.GroundCollider.Raycast
            mH2DCCollider.GroundCollider.transform.position += mLastMovement;
            mH2DCCollider.NoGroundCollider.transform.position += mLastMovement;
            return true;
        }
        PlayerColliderInstance mH2DCCollider = null;
        Vector3 mLastMovement;
    }
}