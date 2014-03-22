using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public interface IH2DCCollider<T>
    {
        bool Grounded { get; }
        Collider GroundCollider { get; }
        Collider NoGroundCollider { get; }
        bool Init();
        bool Update();
    }
}