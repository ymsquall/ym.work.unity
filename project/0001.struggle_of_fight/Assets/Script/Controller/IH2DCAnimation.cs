using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public enum AnimationType : byte
    {
        EANT_Idel,          // 待机
        EANT_Walk,          // 走
        EANT_Running,       // 跑
        EANT_Jumpup,        // 起跳
        EANT_Airing,        // 空中
        EANT_Droping,       // 下落
        EANT_JumpDown,      // 落地
        EANT_Skill01,       // 技能1
        EANT_Skill02,       // 技能2
        EANT_Attack01,      // 攻击1
        EANT_Attack02,      // 攻击2
        EANT_Attack03,      // 攻击3
        EANT_AirAttack01,   // 空中攻击1
        EANT_BeAttack,      // 被击
        EANT_Clobber,       // 被击飞
        EANT_Death,         // 死亡
        EANT_Max,
    }
    public interface IH2DCAnimation<T>
    {
        Animation AnimationInst { get; }
        bool Init();
        bool ChangeAnim(AnimationType animType);
        bool Update();
    }
}