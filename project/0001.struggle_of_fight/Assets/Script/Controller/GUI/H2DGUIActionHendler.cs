using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller.OPUI
{
    using PlayerOperationsController = IH2DCOperations<H2DOperationsController>;
    public class H2DGUIActionHendler : MonoBehaviour
    {
        public Object 宿主程序;

        void Awake()
        {
            GameObject cameraObj = GameObject.Find("MainCamera");
            mLinkedUI = cameraObj.GetComponent<H2DGUIControllerPanel>();
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
            mLinkedUI.OnButtonDown += OnButtonDown;
            mLinkedUI.OnButtonClicked += OnButtonClicked;
        }
        bool OnButtonDown(Object sender, System.EventArgs args)
        {
            if (sender == mLinkedUI.mAttackButton)
            {
                mController.DoTouchBegin(OperationType.attack);
                return true;
            }
            else if (sender == mLinkedUI.mJumpButton)
            {
                mController.DoTouchBegin(OperationType.jump);
                return true;
            }
            else if (sender == mLinkedUI.mSkill1Button)
            {
                mController.DoTouchBegin(OperationType.skill1);
                return true;
            }
            else if (sender == mLinkedUI.mSkill2Button)
            {
                mController.DoTouchBegin(OperationType.skill2);
                return true;
            }
            return false;
        }

        bool OnButtonClicked(Object sender, System.EventArgs args)
        {
            if (sender == mLinkedUI.mAttackButton)
            {
                mController.DoTouchEnded(OperationType.attack);
                return true;
            }
            else if (sender == mLinkedUI.mJumpButton)
            {
                mController.DoTouchEnded(OperationType.jump);
                return true;
            }
            else if (sender == mLinkedUI.mSkill1Button)
            {
                mController.DoTouchEnded(OperationType.skill1);
                return true;
            }
            else if (sender == mLinkedUI.mSkill2Button)
            {
                mController.DoTouchEnded(OperationType.skill2);
                return true;
            }
            return false;
        }
        H2DGUIControllerPanel mLinkedUI;
        PlayerOperationsController mController;
    }
}