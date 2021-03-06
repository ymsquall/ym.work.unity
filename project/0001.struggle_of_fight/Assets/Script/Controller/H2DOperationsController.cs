﻿using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller
{
    using PlayerOperationsInstance = IH2DCOperations<H2DOperationsController>;
    public class H2DOperationsController
    {
        public H2DOperationsController(PlayerOperationsInstance instance)
        {
            mH2DCOperations = instance;
        }
        public bool Init()
        {
            return true;
        }
        public bool Update()
        {
            mAttackComboTimer -= Time.deltaTime;
            if (mAttackComboTimer <= 0.0f)
                mAttackComboNum = 0;
            if (AnimationType.EANT_Skill01 == mH2DCOperations.AnimType)
            {
                mSkill1Timer -= Time.deltaTime;
                if (mSkill1Timer <= 0.0f)
                {
                    mH2DCOperations.OnSkillOvered(1);
                }
            }
            return true;
        }
        public void DoAttack()
        {
            if (AnimationType.EANT_Idel == mH2DCOperations.AnimType || AnimationType.EANT_Running == mH2DCOperations.AnimType)
            {
                if (mAttackComboNum >= mH2DCOperations.AttackComboMaxNum)
                    mAttackComboNum = 0;
                int nowNum = (int)AnimationType.EANT_Attack01;
                mH2DCOperations.ChangeAnimType((AnimationType)(nowNum + mAttackComboNum++));
                mAttackComboTimer = mH2DCOperations.AttackComboTimeout;
            }
            else if(AnimationType.EANT_Airing == mH2DCOperations.AnimType || AnimationType.EANT_Droping == mH2DCOperations.AnimType)
            {
                mH2DCOperations.ChangeAnimType(AnimationType.EANT_AirAttack01);
            }
        }
        public void DoSkill(int skillID)
        {
            if (AnimationType.EANT_Idel == mH2DCOperations.AnimType || AnimationType.EANT_Running == mH2DCOperations.AnimType ||
                AnimationType.EANT_Airing == mH2DCOperations.AnimType || AnimationType.EANT_Droping == mH2DCOperations.AnimType)
            {
                if (skillID == 1)
                {
                    mH2DCOperations.ChangeAnimType(AnimationType.EANT_Skill01);
                    mSkill1Timer = mH2DCOperations.Skill1MaxTime;
                    GameObject assaultEffect = GameObject.Find("Effect.Assault");
                    if (assaultEffect != null)
                    {
                        ImageFrameAnim pAnim = assaultEffect.GetComponent<ImageFrameAnim>();
                        pAnim.Play();
                    }
                }
                else if (skillID == 2)
                {
                    mH2DCOperations.ChangeAnimType(AnimationType.EANT_Skill02);
                    GameObject hmcEffect = GameObject.Find("Effect.HelfMoonCut");
                    if (hmcEffect != null)
                    {
                        ColliderImageFrameAnim pAnim = hmcEffect.GetComponent<ColliderImageFrameAnim>();
                        pAnim.Play();
                        pAnim.ActiveCollider = true;
                    }
                }
            }
        }
        PlayerOperationsInstance mH2DCOperations = null;
        int mAttackComboNum = 0;
        float mAttackComboTimer = 0.0f;
        float mSkill1Timer = 0.0f;
    }
}