using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public interface IH2DCCamera
    {
        float LockCameraTimer { get; }
        Bounds Bounds { get; }
    }
}
