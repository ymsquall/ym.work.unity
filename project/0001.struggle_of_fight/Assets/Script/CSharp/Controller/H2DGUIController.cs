using UnityEngine;
using System.Collections;

public class H2DGUIController : MonoBehaviour
{
    public GameMain_MoveStackPanel mLinkedUI;
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
            return true;
        }
        else if (sender == mLinkedUI.mSkill1Button)
        {
            return true;
        }
        else if (sender == mLinkedUI.mSkill2Button)
        {
            return true;
        }
        return false;
    }

    bool OnButtonClicked(Object sender, System.EventArgs args)
    {
        if (sender == mLinkedUI.mAttackButton)
        {
            return true;
        }
        else if (sender == mLinkedUI.mJumpButton)
        {
            return true;
        }
        else if (sender == mLinkedUI.mSkill1Button)
        {
            return true;
        }
        else if (sender == mLinkedUI.mSkill2Button)
        {
            return true;
        }
        return false;
    }
}
