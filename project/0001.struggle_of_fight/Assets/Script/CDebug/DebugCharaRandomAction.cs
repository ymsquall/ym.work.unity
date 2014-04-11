using UnityEngine;
using System.Collections;
using Assets.Script.Controller;

namespace Assets.Script.CDebug
{
    public class DebugCharaRandomAction : MonoBehaviour
    {
        enum ActionType
        {
            move,
            jump,
            attack01,
            attack02,
            attack03,
            skill01,
            skill02,
            max,
        }
        H2DCharacterController mController;
        float mTimer = 0.0f;
        float mLastTimerLimit = 1.0f;
        float mThinkingTime;
        float mThinkDelayTime = 5.0f;
        ActionType mNowAction = ActionType.max;
        public float ThinkDelayTime
        {
            set { mThinkDelayTime = value; }
        }
        void Start()
        {
            mController = transform.GetComponent<H2DCharacterController>();
            mLastTimerLimit = Random.Range(1.0f, 2.0f);
        }
        void Update()
        {
            if (mThinkingTime >= mThinkDelayTime)
            {
                mController.FocusMoveValue = 0.0f;
                mController.FocusJumpEnd = false;
                mNowAction = (ActionType)Random.Range((int)ActionType.move, (int)ActionType.jump + 1);
                DoAction();
                mThinkingTime = 0;
                mTimer = 0;
            }
            if (mTimer >= mLastTimerLimit)
            {
                mTimer = mTimer - mLastTimerLimit;
                mLastTimerLimit = Random.Range(1.0f, 2.0f);
                DoAction();
            }
            mThinkingTime += Time.deltaTime;
            mTimer += Time.deltaTime;
        }
        void DoAction()
        {
            switch (mNowAction)
            {
                case ActionType.move:
                    mController.FocusMoveValue = Random.Range(-2.0f, 2.0f);
                    break;
                case ActionType.jump:
                    mController.FocusJumpBegin = Random.Range(1.0f, 5.0f);
                    break;
            }
        }
    }
}
