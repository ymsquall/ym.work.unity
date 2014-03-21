using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public interface IH2DCGravity<T>
    {
        float Gravity { get; }
        bool Grounded { get; }
        bool Init();
        bool Update();
    }
}

