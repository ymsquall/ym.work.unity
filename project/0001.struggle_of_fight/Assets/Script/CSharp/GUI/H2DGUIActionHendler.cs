using UnityEngine;
using System.Collections;

public class H2DGUIActionHendler : MonoBehaviour
{
    public H2DGUIControllerPanel mLinkedUI;
    public Horizontal2DController mController;

    void Awake()
    {
        mLinkedUI.OnButtonDown += OnButtonDown;
        mLinkedUI.OnButtonClicked += OnButtonClicked;
    }
    bool OnButtonDown(Object sender, System.EventArgs args)
    {
        if (sender == mLinkedUI.mAttackButton)
        {
            mController.DoAttack();
            return true;
        }
        else if (sender == mLinkedUI.mJumpButton)
        {
            mController.DoJump();
            return true;
        }
        else if (sender == mLinkedUI.mSkill1Button)
        {
            mController.DoAssaultSkill();
            return true;
        }
        else if (sender == mLinkedUI.mSkill2Button)
        {
            mController.DoHelfCutSkill();
            return true;
        }
        return false;
    }

    bool OnButtonClicked(Object sender, System.EventArgs args)
    {
        if (sender == mLinkedUI.mAttackButton)
        {
            mController.DoAttack();
            return true;
        }
        else if (sender == mLinkedUI.mJumpButton)
        {
            mController.DoJump();
            return true;
        }
        else if (sender == mLinkedUI.mSkill1Button)
        {
            mController.DoAssaultSkill();
            return true;
        }
        else if (sender == mLinkedUI.mSkill2Button)
        {
            mController.DoHelfCutSkill();
            return true;
        }
        return false;
    }
}
