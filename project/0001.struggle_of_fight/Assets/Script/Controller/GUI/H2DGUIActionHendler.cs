using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller.OPUI
{
    using PlayerOperationsController = IH2DCOperations<H2DOperationsController>;
    public class H2DGUIActionHendler : MonoBehaviour
    {
        public Object 宿主程序;
        public H2DGUIControllerPanel 关联UI;

        void Awake()
        {
            if (null == 宿主程序)
            {
                Debug.LogError("H2DGUIActionHendler的宿主程序设置非法！");
                return;
            }
            Component componet = GetComponent(宿主程序.name);
            if (null == componet)
            {
                Debug.LogError("H2DGUIActionHendler的宿主程序设置非法！");
                return;
            }
            mController = componet as PlayerOperationsController;
            if (null == mController)
            {
                Debug.LogError("H2DGUIActionHendler的宿主程序必须继承自IH2DCOperations<H2DOperationsController>！");
                return;
            }
            关联UI.OnButtonDown += OnButtonDown;
            关联UI.OnButtonClicked += OnButtonClicked;
        }
        bool OnButtonDown(Object sender, System.EventArgs args)
        {
            if (sender == 关联UI.mAttackButton)
            {
                mController.DoTouchBegin(OperationType.attack);
                return true;
            }
            else if (sender == 关联UI.mJumpButton)
            {
                mController.DoTouchBegin(OperationType.jump);
                return true;
            }
            else if (sender == 关联UI.mSkill1Button)
            {
                mController.DoTouchBegin(OperationType.skill1);
                return true;
            }
            else if (sender == 关联UI.mSkill2Button)
            {
                mController.DoTouchBegin(OperationType.skill2);
                return true;
            }
            return false;
        }

        bool OnButtonClicked(Object sender, System.EventArgs args)
        {
            if (sender == 关联UI.mAttackButton)
            {
                mController.DoTouchEnded(OperationType.attack);
                return true;
            }
            else if (sender == 关联UI.mJumpButton)
            {
                mController.DoTouchEnded(OperationType.jump);
                return true;
            }
            else if (sender == 关联UI.mSkill1Button)
            {
                mController.DoTouchEnded(OperationType.skill1);
                return true;
            }
            else if (sender == 关联UI.mSkill2Button)
            {
                mController.DoTouchEnded(OperationType.skill2);
                return true;
            }
            return false;
        }
        PlayerOperationsController mController;
    }
}