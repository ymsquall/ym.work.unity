using UnityEngine;
using System.Collections;

public class GameMain_MoveStackEvent : MonoBehaviour
{
	public GameMain_MoveStackPanel mLinkedUI;
	// Use this for initialization
	void Start ()
	{
		mLinkedUI.OnButtonDown += OnMoveStackButtonDown;
		mLinkedUI.OnButtonDrag += OnMoveStackButtonDrag;
		mLinkedUI.OnButtonUp += OnMoveStackButtonUp;
	}
	
	bool OnMoveStackButtonDown(Object sender, System.EventArgs args)
	{
		var mouseArgs = args as GameMain_MoveStackPanel.ButtonEventArgs;
		if (sender == mLinkedUI.mMoveStackBar)
		{
			return true;
		}
		else if(sender == mLinkedUI.mMoveStackBar)
		{
			return true;
		}
		return false;
	}
	
	bool OnMoveStackButtonDrag(Object sender, System.EventArgs args)
	{
		var mouseArgs = args as GameMain_MoveStackPanel.ButtonEventArgs;
		if (sender == mLinkedUI.mMoveStackBar)
		{
			return true;
		}
		else if(sender == mLinkedUI.mMoveStackBar)
		{
			return true;
		}
		return false;
	}
	
	bool OnMoveStackButtonUp(Object sender, System.EventArgs args)
	{
		var mouseArgs = args as GameMain_MoveStackPanel.ButtonEventArgs;
		if (sender == mLinkedUI.mMoveStackBar)
		{
			return true;
		}
		else if(sender == mLinkedUI.mMoveStackBar)
		{
			return true;
		}
		return false;
	}
}
