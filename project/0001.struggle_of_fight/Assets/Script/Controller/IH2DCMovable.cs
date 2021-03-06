﻿using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public interface IH2DCMovable<T>
    {
        bool Jumping { get; }
        bool Droping { get; }
        bool Moving { get; }
        float SpeedScaleX { get; }
        float InputSpeedX { get; }
        float SpeedSmoothing { get; }
        float InAirControlAcceleration { get; }
        float JumpHeight { get; }
        bool UsedModelFlipX { get; }
        CharacterController Controller { get; }
        bool Init();
        bool Update();
    }
}
