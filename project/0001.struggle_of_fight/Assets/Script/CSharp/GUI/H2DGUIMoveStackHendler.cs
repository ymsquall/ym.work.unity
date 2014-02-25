using UnityEngine;
using System.Collections;

public class H2DGUIMoveStackController : MonoBehaviour
{
    public H2DGUIControllerPanel mLinkedUI;
	// Use this for initialization
	void Start ()
	{
		mLinkedUI.OnButtonDown += OnMoveStackButtonDown;
		mLinkedUI.OnButtonDrag += OnMoveStackButtonDrag;
		mLinkedUI.OnButtonUp += OnMoveStackButtonUp;
	}
	
	bool OnMoveStackButtonDown(Object sender, System.EventArgs args)
	{
        var mouseArgs = args as H2DGUIControllerPanel.ButtonEventArgs;
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
        var mouseArgs = args as H2DGUIControllerPanel.ButtonEventArgs;
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
        var mouseArgs = args as H2DGUIControllerPanel.ButtonEventArgs;
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
