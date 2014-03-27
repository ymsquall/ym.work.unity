using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    public enum OperationType
    {
        jump,
        attack,
        skill1,
        skill2,
    }
    public interface IH2DCOperations<T>
    {
        AnimationType AnimType { get; }
        int AttackComboMaxNum { get; }
        float AttackComboTimeout { get; }
        float Skill1MaxTime { get; }
        bool Init();
        bool Update();
        bool DoTouchBegin(OperationType ot);
        bool DoTouchEnded(OperationType ot);
        bool ChangeAnimType(AnimationType animType);
        bool OnSkillOvered(int id);
    }
}

