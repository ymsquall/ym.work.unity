using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerGravityInstance = IH2DCGravity<H2DGravityController>;
    public class H2DGravityController
    {
        public H2DGravityController(PlayerGravityInstance instance)
        {
            mH2DCGravity = instance;
        }
        public float VerticalSpeed
        {
            set { mVerticalSpeed = value; }
            get { return mVerticalSpeed; }
        }
        public bool Init()
        {
            return true;
        }
        public bool Update()
        {
            if (null == mH2DCGravity)
                return false;
            if (mH2DCGravity.Grounded && mVerticalSpeed < 0.0f)
                mVerticalSpeed = 0.0f;
            else
                mVerticalSpeed -= mH2DCGravity.Gravity * Time.deltaTime;
            return true;
        }
        PlayerGravityInstance mH2DCGravity = null;
        float mVerticalSpeed = 0.0f;
    }
}