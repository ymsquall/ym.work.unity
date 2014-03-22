using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public enum H2DCCollideSelecterType : byte
    {
        无,
        地面,
        非地面,
    }
    public interface IH2DCreatureCollideSelecter
    {
        bool OnH2DCCollisionEnter(Collision collisionInfo, H2DCCollideSelecterType type);
        bool OnH2DCCollisionExit(Collision collisionInfo, H2DCCollideSelecterType type);
        bool OnH2DCCollisionStay(Collision collisionInfo, H2DCCollideSelecterType type);
    }
}