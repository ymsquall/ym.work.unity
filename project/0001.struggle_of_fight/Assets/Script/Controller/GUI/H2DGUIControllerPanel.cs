using UnityEngine;
using System.Collections;

namespace Assets.Script.Controller.OPUI
{
    public class H2DGUIControllerPanel : MonoBehaviour
    {
        public GUISkin mMoveStackBG;
        public GUISkin mMoveStackBar;
        public GUISkin mAttackButton;
        public GUISkin mJumpButton;
        public GUISkin mSkill1Button;
        public GUISkin mSkill2Button;
        public Rect mRightToBottomRect;
        public Vector2 mAttackBtnPos;
        public Vector2 mJumpBtnPos;
        public Vector2 mSkill1BtnPos;
        public Vector2 mSkill2BtnPos;

        public class ButtonEventArgs : System.EventArgs
        {
            public ButtonEventArgs(Vector2 pt)
            {
                mousePt = pt;
            }
            public ButtonEventArgs(Vector2 pt, bool inDownBtn)
            {
                mousePt = pt;
                mInDownBtn = inDownBtn;
            }
            public Vector2 mousePt;
            public bool mInDownBtn = false;
        }
        public delegate bool ButtonEventHandler(Object sender, System.EventArgs args);
        public event ButtonEventHandler OnButtonDown;
        public event ButtonEventHandler OnButtonDrag;
        public event ButtonEventHandler OnButtonUp;
        public event ButtonEventHandler OnButtonClicked;

        void Start()
        {
            mLastMousePos3D = Input.mousePosition;

            var btnStyle = mMoveStackBG.customStyles[0] as GUIStyle;
            mBGImageRect = new Rect(0, Screen.height - btnStyle.normal.background.height,
                                     btnStyle.normal.background.width, btnStyle.normal.background.height);

            btnStyle = mMoveStackBar.customStyles[0] as GUIStyle;
            mBarButtonRect = new Rect(0, 0, btnStyle.normal.background.width, btnStyle.normal.background.height);
            mBarButtonRect.x = mBGImageRect.x + (mBGImageRect.width - mBarButtonRect.width) / 2.0f;
            mBarButtonRect.y = mBGImageRect.y + (mBGImageRect.height - mBarButtonRect.height) / 2.0f;

            mRightToBottomRect.x = Screen.width - mRightToBottomRect.width;
            mRightToBottomRect.y = Screen.height - mRightToBottomRect.height;

            btnStyle = mAttackButton.customStyles[0] as GUIStyle;
            mAttackBtnRect = new Rect(mRightToBottomRect.x + mAttackBtnPos.x, mRightToBottomRect.y + mAttackBtnPos.y,
                                      btnStyle.normal.background.width, btnStyle.normal.background.height);

            btnStyle = mJumpButton.customStyles[0] as GUIStyle;
            mJumpBtnRect = new Rect(mRightToBottomRect.x + mJumpBtnPos.x, mRightToBottomRect.y + mJumpBtnPos.y,
                                     btnStyle.normal.background.width, btnStyle.normal.background.height);

            btnStyle = mSkill1Button.customStyles[0] as GUIStyle;
            mSkill1BtnRect = new Rect(mRightToBottomRect.x + mSkill1BtnPos.x, mRightToBottomRect.y + mSkill1BtnPos.y,
                                     btnStyle.normal.background.width, btnStyle.normal.background.height);

            btnStyle = mSkill2Button.customStyles[0] as GUIStyle;
            mSkill2BtnRect = new Rect(mRightToBottomRect.x + mSkill2BtnPos.x, mRightToBottomRect.y + mSkill2BtnPos.y,
                                     btnStyle.normal.background.width, btnStyle.normal.background.height);
        }

        void Update()
        {
            Vector3 mousePt3 = Input.mousePosition;
            Vector2 mousePt2 = new Vector2(mousePt3.x, Screen.height - mousePt3.y);
            bool mouseDown = Input.GetMouseButtonDown(0);
            bool mouseUp = Input.GetMouseButtonUp(0);
            if (mouseUp)
            {
                bool buttonUped = false;
                if (mBGImageRect.Contains(mousePt2))
                {
                    if (null != OnButtonUp && OnButtonUp(mMoveStackBG, new ButtonEventArgs(mousePt2, mMouseDownButton == mMoveStackBG)))
                        buttonUped = true;
                }
                if (mAttackBtnRect.Contains(mousePt2))
                {
                    if (null != OnButtonUp && OnButtonUp(mAttackButton, new ButtonEventArgs(mousePt2, mMouseDownButton == mAttackButton)))
                        buttonUped = true;
                }
                if (mJumpBtnRect.Contains(mousePt2))
                {
                    if (null != OnButtonUp && OnButtonUp(mJumpButton, new ButtonEventArgs(mousePt2, mMouseDownButton == mJumpButton)))
                        buttonUped = true;
                }
                if (mSkill1BtnRect.Contains(mousePt2))
                {
                    if (null != OnButtonUp && OnButtonUp(mSkill1Button, new ButtonEventArgs(mousePt2, mMouseDownButton == mSkill1Button)))
                        buttonUped = true;
                }
                if (mSkill2BtnRect.Contains(mousePt2))
                {
                    if (null != OnButtonUp && OnButtonUp(mSkill2Button, new ButtonEventArgs(mousePt2, mMouseDownButton == mSkill2Button)))
                        buttonUped = true;
                }
                if (!buttonUped && (null != OnButtonUp))
                    OnButtonUp(mMouseDownButton, new ButtonEventArgs(mousePt2));
                mMouseDownButton = null;
            }
            if (mouseDown)
            {
                bool isMouseMove = mLastMousePos3D != mousePt3;
                if (mBGImageRect.Contains(mousePt2))
                {
                    mMouseDownButton = mMoveStackBar;
                    if (isMouseMove && (null != OnButtonDrag))
                        OnButtonDrag(mMoveStackBG, new ButtonEventArgs(mousePt2));
                    if ((null != OnButtonDown) && OnButtonDown(mMoveStackBG, new ButtonEventArgs(mousePt2)))
                        return;
                }
                if (mAttackBtnRect.Contains(mousePt2))
                {
                    mMouseDownButton = mMoveStackBar;
                    if (isMouseMove && (null != OnButtonDrag))
                        OnButtonDrag(mAttackButton, new ButtonEventArgs(mousePt2));
                    if ((null != OnButtonDown) && OnButtonDown(mAttackButton, new ButtonEventArgs(mousePt2)))
                        return;
                }
                if (mJumpBtnRect.Contains(mousePt2))
                {
                    mMouseDownButton = mMoveStackBar;
                    if (isMouseMove && (null != OnButtonDrag))
                        OnButtonDrag(mJumpButton, new ButtonEventArgs(mousePt2));
                    if ((null != OnButtonDown) && OnButtonDown(mJumpButton, new ButtonEventArgs(mousePt2)))
                        return;
                }
                if (mSkill1BtnRect.Contains(mousePt2))
                {
                    mMouseDownButton = mMoveStackBar;
                    if (isMouseMove && (null != OnButtonDrag))
                        OnButtonDrag(mSkill1Button, new ButtonEventArgs(mousePt2));
                    if ((null != OnButtonDown) && OnButtonDown(mSkill1Button, new ButtonEventArgs(mousePt2)))
                        return;
                }
                if (mSkill2BtnRect.Contains(mousePt2))
                {
                    mMouseDownButton = mMoveStackBar;
                    if (isMouseMove && (null != OnButtonDrag))
                        OnButtonDrag(mSkill2Button, new ButtonEventArgs(mousePt2));
                    if ((null != OnButtonDown) && OnButtonDown(mSkill2Button, new ButtonEventArgs(mousePt2)))
                        return;
                }
            }
        }

        void OnGUI()
        {
            GUI.Button(mBGImageRect, "", mMoveStackBG.GetStyle(mMoveStackBG.name));
            GUI.Button(mBarButtonRect, "", mMoveStackBar.GetStyle(mMoveStackBar.name));
            bool btnClicked = false;
            if (GUI.Button(mAttackBtnRect, "", mAttackButton.GetStyle(mAttackButton.name)))
            {
                if (!btnClicked && (null != OnButtonClicked))
                {
                    Vector3 mousePt = Input.mousePosition;
                    btnClicked = OnButtonClicked(mAttackButton, new ButtonEventArgs(new Vector2(mousePt.x, mousePt.y)));
                }
            }
            if (GUI.Button(mJumpBtnRect, "", mJumpButton.GetStyle(mJumpButton.name)))
            {
                if (!btnClicked && (null != OnButtonClicked))
                {
                    Vector3 mousePt = Input.mousePosition;
                    btnClicked = OnButtonClicked(mJumpButton, new ButtonEventArgs(new Vector2(mousePt.x, mousePt.y)));
                }
            }
            if (GUI.Button(mSkill1BtnRect, "", mSkill1Button.GetStyle(mSkill1Button.name)))
            {
                if (!btnClicked && (null != OnButtonClicked))
                {
                    Vector3 mousePt = Input.mousePosition;
                    btnClicked = OnButtonClicked(mSkill1Button, new ButtonEventArgs(new Vector2(mousePt.x, mousePt.y)));
                }
            }
            if (GUI.Button(mSkill2BtnRect, "", mSkill2Button.GetStyle(mSkill2Button.name)))
            {
                if (!btnClicked && (null != OnButtonClicked))
                {
                    Vector3 mousePt = Input.mousePosition;
                    btnClicked = OnButtonClicked(mSkill2Button, new ButtonEventArgs(new Vector2(mousePt.x, mousePt.y)));
                }
            }
        }

        void OnMouseDown()
        {
        }
        void OnMouseDrag()
        {
        }
        void OnMouseUp()
        {
        }

        private Rect mBGImageRect;
        private Rect mBarButtonRect;
        private Rect mAttackBtnRect;
        private Rect mJumpBtnRect;
        private Rect mSkill1BtnRect;
        private Rect mSkill2BtnRect;
        private Object mMouseDownButton = null;
        private static Vector3 mLastMousePos3D;
    }
}