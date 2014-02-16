using UnityEngine;
using System.Collections;

public class GameMain_ActionEvent : MonoBehaviour
{
	public GameMain_MoveStackPanel mLinkedUI;
	// Use this for initialization
	void Start ()
	{
		mLinkedUI.OnButtonClicked += OnActionButtonClicked;

	}
	bool OnActionButtonClicked(Object sender, System.EventArgs args)
	{
		var mouseArgs = args as GameMain_MoveStackPanel.ButtonEventArgs;
		if (sender == mLinkedUI.mAttackButton)
		{
			return true;
		}
		else if(sender == mLinkedUI.mJumpButton)
		{
			return true;
		}
		else if(sender == mLinkedUI.mSkill1Button)
		{
			return true;
		}
		else if(sender == mLinkedUI.mSkill2Button)
		{
			return true;
		}
		return false;
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}
