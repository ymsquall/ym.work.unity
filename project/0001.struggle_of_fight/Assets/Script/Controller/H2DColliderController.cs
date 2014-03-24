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
            //mH2DCCollider.GroundCollider.transform.position -= mLastMovement;
            //mH2DCCollider.NoGroundCollider.transform.position -= mLastMovement;
            //if (mH2DCCollider.Grounded)
            //    mLastMovement.y = 0.0f;
            //trans.position += mLastMovement;
            return true;
        }
        public bool Update(Vector3 movement)
        {
            if (mH2DCCollider.Grounded)
                movement.y = 0.0f;
            mLastMovement = movement;
            Vector3 gcMinBottomRay = mH2DCCollider.GroundCollider.bounds.min;
            Vector3 gcMaxBottomRay = mH2DCCollider.GroundCollider.bounds.max;
            gcMaxBottomRay.y = gcMinBottomRay.y;
            gcMinBottomRay.z = gcMaxBottomRay.z = mH2DCCollider.GroundCollider.bounds.center.z;
            Vector3 rayDir = mH2DCCollider.GroundCollider.transform.TransformDirection(Vector3.down);
            Debug.DrawLine(gcMinBottomRay, gcMinBottomRay + rayDir * 100.0f, Color.green);
            Debug.DrawLine(gcMaxBottomRay, gcMaxBottomRay + rayDir * 100.0f, Color.green);
            RaycastHit gcMinCast, gcMaxCast;
            bool minRayHit = Physics.Raycast(new Ray(gcMinBottomRay, rayDir), out gcMinCast, 100.0f, mH2DCCollider.GroundLayerMask);
            bool maxRayHit = Physics.Raycast(new Ray(gcMaxBottomRay, rayDir), out gcMaxCast, 100.0f, mH2DCCollider.GroundLayerMask);
            if (minRayHit)
            {
                GameObject dbgSphere = GameObject.Find("dbgMinCastSphere");
                if (null == dbgSphere)
                {
                    dbgSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
                    dbgSphere.name = "dbgMinCastSphere";
                    dbgSphere.collider.enabled = false;
                }
                dbgSphere.transform.position = gcMinCast.point;
            }
            if (maxRayHit)
            {
                GameObject dbgSphere = GameObject.Find("dbgMaxCastSphere");
                if (null == dbgSphere)
                {
                    dbgSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
                    dbgSphere.name = "dbgMaxCastSphere";
                    dbgSphere.collider.enabled = false;
                }
                dbgSphere.transform.position = gcMaxCast.point;
            }
            if (!minRayHit && !maxRayHit)
            {
                GameObject.DestroyObject(GameObject.Find("dbgMinCastSphere"));
                GameObject.DestroyObject(GameObject.Find("dbgMaxCastSphere"));
            }
            //mH2DCCollider.GroundCollider.transform.position += mLastMovement;
            //mH2DCCollider.NoGroundCollider.transform.position += mLastMovement;
            return true;
        }
        PlayerColliderInstance mH2DCCollider = null;
        Vector3 mLastMovement;
    }
}