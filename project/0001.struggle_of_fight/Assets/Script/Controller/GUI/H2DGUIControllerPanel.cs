using UnityEngine;
using System.Collections;
using Assets.Script.Controller;

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
        public Vector2 移动UI位置偏移 = new Vector2(25, 25);
        public Vector2 UI缩放倍数 = Vector2.zero;
        public Rect mRightToBottomRect;
        public Vector2 mAttackBtnPos;
        public Vector2 mJumpBtnPos;
        public Vector2 mSkill1BtnPos;
        public Vector2 mSkill2BtnPos;
        // fps
        public bool 显示FPS = true;
        public float FPS更新间隔 = 0.5f;
        public GameObject 测试刷怪 = null;

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
            Input.multiTouchEnabled = true;
#if UNITY_STANDALONE
            mLastMousePos3D = Input.mousePosition;
#endif
            var btnStyle = mMoveStackBG.customStyles[0] as GUIStyle;
            mBGImageRect = new Rect(移动UI位置偏移.x, Screen.height - btnStyle.normal.background.height * UI缩放倍数.x - 移动UI位置偏移.y,
                                     btnStyle.normal.background.width * UI缩放倍数.x, btnStyle.normal.background.height * UI缩放倍数.y);

            btnStyle = mMoveStackBar.customStyles[0] as GUIStyle;
            mBarButtonOriRect = new Rect(0, 0, btnStyle.normal.background.width * UI缩放倍数.x
                                                , btnStyle.normal.background.height * UI缩放倍数.y);
            mBarButtonOriRect.x = mBGImageRect.x + (mBGImageRect.width - mBarButtonOriRect.width) / 2.0f;
            mBarButtonOriRect.y = mBGImageRect.y + (mBGImageRect.height - mBarButtonOriRect.height) / 2.0f;

            mMoveStakBarLeftLimit = mBGImageRect.x;
            mMoveStakBarRightLimit = mBGImageRect.x + mBGImageRect.width - mBarButtonOriRect.width;

            mRightToBottomRect.x = Screen.width - mRightToBottomRect.width;
            mRightToBottomRect.y = Screen.height - mRightToBottomRect.height;

            btnStyle = mAttackButton.customStyles[0] as GUIStyle;
            mAttackBtnRect = new Rect(mRightToBottomRect.x + mAttackBtnPos.x - (btnStyle.normal.background.width * UI缩放倍数.x - btnStyle.normal.background.width),
                                        mRightToBottomRect.y + mAttackBtnPos.y - (btnStyle.normal.background.height * UI缩放倍数.y - btnStyle.normal.background.height),
                                        btnStyle.normal.background.width * UI缩放倍数.x, btnStyle.normal.background.height * UI缩放倍数.y);

            btnStyle = mJumpButton.customStyles[0] as GUIStyle;
            mJumpBtnRect = new Rect(mRightToBottomRect.x + mJumpBtnPos.x - (btnStyle.normal.background.width * UI缩放倍数.x - btnStyle.normal.background.width),
                                    mRightToBottomRect.y + mJumpBtnPos.y - (btnStyle.normal.background.height * UI缩放倍数.y - btnStyle.normal.background.height),
                                    btnStyle.normal.background.width * UI缩放倍数.x, btnStyle.normal.background.height * UI缩放倍数.y);

            btnStyle = mSkill1Button.customStyles[0] as GUIStyle;
            mSkill1BtnRect = new Rect(mRightToBottomRect.x + mSkill1BtnPos.x - (btnStyle.normal.background.width * UI缩放倍数.x - btnStyle.normal.background.width),
                                        mRightToBottomRect.y + mSkill1BtnPos.y - (btnStyle.normal.background.height * UI缩放倍数.y - btnStyle.normal.background.height),
                                        btnStyle.normal.background.width * UI缩放倍数.x, btnStyle.normal.background.height * UI缩放倍数.y);

            btnStyle = mSkill2Button.customStyles[0] as GUIStyle;
            mSkill2BtnRect = new Rect(mRightToBottomRect.x + mSkill2BtnPos.x - (btnStyle.normal.background.width * UI缩放倍数.x - btnStyle.normal.background.width),
                                        mRightToBottomRect.y + mSkill2BtnPos.y - (btnStyle.normal.background.height * UI缩放倍数.y - btnStyle.normal.background.height),
                                        btnStyle.normal.background.width * UI缩放倍数.x, btnStyle.normal.background.height * UI缩放倍数.y);
            mLastInterval = Time.realtimeSinceStartup;
            mFrames = 0;
        }

        void Update()
        {
#if UNITY_STANDALONE
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
#else
            mBarButtonRect = mBarButtonOriRect;
            H2DPlayerController.LocalPlayer.TouchMoveSpeed = 0.0f;
            for (int i = 0; i < Input.touchCount; ++ i)
            {
                Vector2 pos = Input.touches[i].position;
                pos.y = Screen.height - pos.y;
                if (mBGImageRect.Contains(pos) && Input.touches[i].phase == TouchPhase.Began)
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        mMoveStackTouchID = i;
                        break;
                    }
                }
            }
            if (-1 != mMoveStackTouchID)
            {
                if (Input.touches[mMoveStackTouchID].phase == TouchPhase.Ended || Input.touches[mMoveStackTouchID].phase == TouchPhase.Canceled)
                    mMoveStackTouchID = -1;
                Vector2 pos = Input.touches[mMoveStackTouchID].position;
                pos.y = Screen.height - pos.y;
                mBarButtonRect.x = pos.x - mBarButtonRect.width / 2.0f;
                if (mBarButtonRect.x > mMoveStakBarRightLimit)
                    mBarButtonRect.x = mMoveStakBarRightLimit;
                if (mBarButtonRect.x < mMoveStakBarLeftLimit)
                    mBarButtonRect.x = mMoveStakBarLeftLimit;
                float horDist = (mBarButtonRect.x - mBarButtonOriRect.x) / 20.0f;
                if (horDist > 1.0f)
                    H2DPlayerController.LocalPlayer.TouchMoveSpeed = 1.0f;
                else if (horDist < -1.0f)
                    H2DPlayerController.LocalPlayer.TouchMoveSpeed = -1.0f;
                else
                    H2DPlayerController.LocalPlayer.TouchMoveSpeed = horDist;
            }
#endif
            if (显示FPS)
            {
                ++mFrames;
                float timeNow = Time.realtimeSinceStartup;
                if (timeNow > mLastInterval + FPS更新间隔)
                {
                    mFPS = mFrames / (timeNow - mLastInterval);
                    mFrames = 0;
                    mLastInterval = timeNow;
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
#if !UNITY_STANDALONE
            for(int i = 0; i < Input.touchCount; ++ i)
            {
                if (Input.touches[i].phase != TouchPhase.Began)
                    continue;
                Vector2 pos = Input.touches[i].position;
                pos.y = Screen.height - pos.y;
                if (mBGImageRect.Contains(pos))
                    continue;
                if (mAttackBtnRect.Contains(pos))
                {
                    if (null != OnButtonDown)
                    {
                        if (OnButtonDown(mAttackButton, new ButtonEventArgs(new Vector2(pos.x, pos.y))))
                            break;
                    }
                }
                if (mJumpBtnRect.Contains(pos))
                {
                    if (null != OnButtonDown)
                    {
                        if (OnButtonDown(mJumpButton, new ButtonEventArgs(new Vector2(pos.x, pos.y))))
                            break;
                    }
                }
                if (mSkill1BtnRect.Contains(pos))
                {
                    if (null != OnButtonDown)
                    {
                        if (OnButtonDown(mSkill1Button, new ButtonEventArgs(new Vector2(pos.x, pos.y))))
                            break;
                    }
                }
                if (mSkill2BtnRect.Contains(pos))
                {
                    if (null != OnButtonDown)
                    {
                        if (OnButtonDown(mSkill2Button, new ButtonEventArgs(new Vector2(pos.x, pos.y))))
                            break;
                    }
                }
            }
#endif
            if (显示FPS)
            {
                string debugText = string.Format("FPS:{0}, 生物数量:{1}", mFPS.ToString("f2"), mCreatureCount);
                GUIStyle bb = new GUIStyle();
                bb.normal.background = null;    //这是设置背景填充的
                bb.normal.textColor = new Color(1, 0, 0);   //设置字体颜色的
                bb.fontSize = 30;       //当然，这是字体颜色
                GUI.Label(new Rect(0, 0, 200, 20), debugText, bb);
            }
            // test
            if (null != 测试刷怪)
            {
                Rect rc = new Rect(Screen.width - (mAttackBtnRect.width / UI缩放倍数.x) * 2 + 20, 10, (mAttackBtnRect.width / UI缩放倍数.x), (mAttackBtnRect.height / UI缩放倍数.y));
                if (GUI.Button(rc, "", mAttackButton.GetStyle(mAttackButton.name)))
                {
                    if (!btnClicked)
                    {
                        GameObject objClone = MonoBehaviour.Instantiate(测试刷怪) as GameObject;
                        objClone.name = "_" + 测试刷怪.name;
                        objClone.transform.localPosition = new Vector3(
                                Random.Range(0.0f, (float)Screen.width) / 200.0f,
                                Random.Range((Screen.height / 1.5f), (float)Screen.height / 3.0f) / 200.0f,
                                测试刷怪.transform.localPosition.z
                            );
                        H2DCharacterController ctrl = objClone.GetComponent<H2DCharacterController>();
                        ctrl.FaceDirection = Vector3.right;
                        CDebug.DebugCharaRandomAction dbgAct = objClone.AddComponent<CDebug.DebugCharaRandomAction>();
                        mCreatureCount++;
                    }
                }
                rc = new Rect(Screen.width - (mAttackBtnRect.width / UI缩放倍数.x) + 10, 10, (mAttackBtnRect.width / UI缩放倍数.x), (mAttackBtnRect.height / UI缩放倍数.y));
                if (GUI.Button(rc, "", mAttackButton.GetStyle(mAttackButton.name)))
                {
                    if (!btnClicked && mCreatureCount > 2)
                    {
                        GameObject objClone = GameObject.Find("_" + 测试刷怪.name);
                        MonoBehaviour.DestroyObject(objClone);
                        --mCreatureCount;
                    }
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
        private Rect mBarButtonOriRect;
        private Rect mBarButtonRect;
        private Rect mAttackBtnRect;
        private Rect mJumpBtnRect;
        private Rect mSkill1BtnRect;
        private Rect mSkill2BtnRect;
        float mMoveStakBarLeftLimit = 0.0f;
        float mMoveStakBarRightLimit = 0.0f;
#if UNITY_STANDALONE
        private Object mMouseDownButton = null;
        private static Vector3 mLastMousePos3D;
#endif
        int mMoveStackTouchID = -1;
        // show fps
        float mLastInterval;
        int mFrames = 0;
        float mFPS;
        int mCreatureCount = 2;
    }
}