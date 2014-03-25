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
        public Vector3 FixedPosition
        {
            get { return mFixedPosition; }
        }
        public bool Init()
        {
            return true;
        }
        public bool Update(Vector3 movement, Vector3 pos)
        {
            mFixedPosition = pos;
            // ground
            Vector3 gcMinBottomRay = mH2DCCollider.GroundCollider.bounds.min;
            Vector3 gcMaxBottomRay = mH2DCCollider.GroundCollider.bounds.max;
            gcMaxBottomRay.y = gcMinBottomRay.y = 10.0f;
            gcMinBottomRay.z = gcMaxBottomRay.z = mH2DCCollider.GroundCollider.bounds.center.z;
            Vector3 rayDir = mH2DCCollider.GroundCollider.transform.TransformDirection(Vector3.down);
            RaycastHit[] gcMinCastList = Physics.RaycastAll(new Ray(gcMinBottomRay, rayDir), 100.0f, mH2DCCollider.GroundLayerMask.value);
            RaycastHit[] gcMaxCastList = Physics.RaycastAll(new Ray(gcMaxBottomRay, rayDir), 100.0f, mH2DCCollider.GroundLayerMask.value);
            // find single hit ground
            bool minHit = false, maxHit = false;
            RaycastHit minRayHit = new RaycastHit(), maxRayHit = new RaycastHit();
            if (gcMinCastList.Length > 0)
            {
                minHit = true;
                minRayHit = gcMinCastList[0];
                float dist1 = Vector3.Distance(mH2DCCollider.GroundCollider.bounds.min, minRayHit.point);
                for (int i = 0; i < gcMinCastList.Length; ++i)
                {
                    RaycastHit hit = gcMinCastList[i];
                    if (hit.point.y > (mH2DCCollider.GroundCollider.bounds.min.y + 0.1f))
                    {
                        if (i == 0)
                            dist1 = 1000.0f;
                        continue;
                    }
                    float dist2 = Vector3.Distance(mH2DCCollider.GroundCollider.bounds.min, hit.point);
                    if (dist1 > dist2)
                    {
                        dist1 = dist2;
                        minRayHit = hit;
                    }
                }
            }
            if (gcMaxCastList.Length > 0)
            {
                maxHit = true;
                maxRayHit = gcMaxCastList[0];
                float dist1 = Vector3.Distance(mH2DCCollider.GroundCollider.bounds.max, maxRayHit.point);
                for (int i = 0; i < gcMaxCastList.Length; ++ i)
                {
                    RaycastHit hit = gcMaxCastList[i];
                    if (hit.point.y > (mH2DCCollider.GroundCollider.bounds.min.y + 0.1f))
                    {
                        if (i == 0)
                            dist1 = 1000.0f;
                        continue;
                    }
                    float dist2 = Vector3.Distance(mH2DCCollider.GroundCollider.bounds.max, hit.point);
                    if (dist1 > dist2)
                    {
                        dist1 = dist2;
                        maxRayHit = hit;
                    }
                }
            }
            Vector3 groundHeightPos = Vector3.zero;
            if (minHit && maxHit)
            {
                groundHeightPos = minRayHit.point;
                if (minRayHit.point.y > mH2DCCollider.GroundCollider.bounds.min.y)
                    groundHeightPos = maxRayHit.point;
                //groundHeightPos = minRayHit.point.y > maxRayHit.point.y ? minRayHit.point : maxRayHit.point;
            }
            else if (minHit)
                groundHeightPos = minRayHit.point;
            else if (maxHit)
                groundHeightPos = maxRayHit.point;
            if (minHit || maxHit)
            {
                //Debug.Log(string.Format("min={0}\tmovement={1}", mH2DCCollider.GroundCollider.bounds.min.ToString(), mFixedMovement.ToString()));
                float sideY = mH2DCCollider.GroundCollider.bounds.min.y - movement.y;
                if (sideY <= groundHeightPos.y)
                {
                    //Debug.Log(string.Format("sideY={0}\tgroundHeightPos={1}\tFixedPosition={2}",
                    //    sideY, groundHeightPos.ToString(), mFixedPosition.ToString()));
                    mFixedPosition.y = groundHeightPos.y;
                    mH2DCCollider.RayInGround = true;
                }
            }
            else
                mH2DCCollider.RayInGround = false;
            //-- debug
            Debug.DrawLine(gcMinBottomRay, gcMinBottomRay + rayDir * 100.0f, Color.green);
            Debug.DrawLine(gcMaxBottomRay, gcMaxBottomRay + rayDir * 100.0f, Color.green);
            if (minHit)
            {
                GameObject dbgSphere = GameObject.Find("dbgMinCastSphere");
                if (null == dbgSphere)
                {
                    dbgSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
                    dbgSphere.name = "dbgMinCastSphere";
                    dbgSphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    dbgSphere.collider.enabled = false;
                }
                dbgSphere.transform.position = minRayHit.point;
            }
            if (maxHit)
            {
                GameObject dbgSphere = GameObject.Find("dbgMaxCastSphere");
                if (null == dbgSphere)
                {
                    dbgSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
                    dbgSphere.name = "dbgMaxCastSphere";
                    dbgSphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                    dbgSphere.collider.enabled = false;
                }
                dbgSphere.transform.position = maxRayHit.point;
            }
            if (!minHit)
                GameObject.DestroyObject(GameObject.Find("dbgMinCastSphere"));
            if (!maxHit)
                GameObject.DestroyObject(GameObject.Find("dbgMaxCastSphere"));
            //-- debug
            return true;
        }
        PlayerColliderInstance mH2DCCollider = null;
        Vector3 mFixedPosition;
    }
}