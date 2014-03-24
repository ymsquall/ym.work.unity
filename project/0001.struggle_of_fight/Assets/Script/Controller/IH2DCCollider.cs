using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public interface IH2DCCollider<T>
    {
        bool Grounded { get; }
        LayerMask GroundLayerMask { get; }
        Collider GroundCollider { get; }
        Collider NoGroundCollider { get; }
        bool Init();
        bool Update();
    }
}