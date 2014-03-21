using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public interface IH2DCMovable<T>
    {
        bool Jumping { get; }
        bool Droping { get; }
        bool Moving { get; }
        bool Init();
        bool Update();
    }
}
