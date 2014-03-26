using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public interface IH2DCCollider<T>
    {
        LayerMask GroundLayerMask { get; }
        Collider GroundCollider { get; }
        bool Init();
        bool Update();
    }
}