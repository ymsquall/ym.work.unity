using System;
using UnityEngine;
using System.Collections;
using UniLua;  

public class LuaScriptController : MonoBehaviour
{
    public string LUA脚本入口 = "LuaScript/main.lua";
    private ILuaState mLuaState;
    private int mAwakeRef;
    private int mStartRef;
    private int mUpdateRef;
    private int mLateUpdateRef;
    private int mFixedUpdateRef;

    void Awake()
    {
        Debug.Log("LuaScriptController Awake");
        if (mLuaState == null)
        {
            mLuaState = LuaAPI.NewState();
            mLuaState.L_OpenLibs();
            var status = mLuaState.L_DoFile(LUA脚本入口);
            if (status != ThreadStatus.LUA_OK)
                throw new Exception(mLuaState.ToString(-1));
            if (!mLuaState.IsTable(-1))
                throw new Exception("entry main's return value is not a table");

            mAwakeRef = StoreMethod("awake");
            mStartRef = StoreMethod("start");
            mUpdateRef = StoreMethod("update");
            mLateUpdateRef = StoreMethod("late_update");
            mFixedUpdateRef = StoreMethod("fixed_update");
            mLuaState.Pop(1);
            Debug.Log("Lua Init Done");
        }
        CallMethod(mAwakeRef);
    }
    IEnumerator Start()
    {
        CallMethod(mStartRef);
        // -- sample code for loading binary Asset Bundles --------------------  
        String s = "file:///" + Application.streamingAssetsPath + "/testx.unity3d";
        WWW www = new WWW(s);
        yield return www;
        if (www.assetBundle.mainAsset != null)
        {
            TextAsset cc = (TextAsset)www.assetBundle.mainAsset;
            var status = mLuaState.L_LoadBytes(cc.bytes, "test");
            if (status != ThreadStatus.LUA_OK)
                throw new Exception(mLuaState.ToString(-1));
            status = mLuaState.PCall(0, 0, 0);
            if (status != ThreadStatus.LUA_OK)
                throw new Exception(mLuaState.ToString(-1));
            Debug.Log("---- call done ----");
        }
    }
    void Update()
    {
        CallMethod(mUpdateRef);
    }
    void LateUpdate()
    {
        CallMethod(mLateUpdateRef);
    }
    void FixedUpdate()
    {
        CallMethod(mFixedUpdateRef);
    }
    private int StoreMethod(string name)
    {
        mLuaState.GetField(-1, name);
        if (!mLuaState.IsFunction(-1))
            throw new Exception(string.Format(
                "method {0} not found!", name));
        return mLuaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
    }
    private void CallMethod(int funcRef)
    {
        mLuaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        // insert `traceback' function  
        var b = mLuaState.GetTop();
        mLuaState.PushCSharpFunction(Traceback);
        mLuaState.Insert(b);
        var status = mLuaState.PCall(0, 0, b);
        if (status != ThreadStatus.LUA_OK)
            Debug.LogError(mLuaState.ToString(-1));
        // remove `traceback' function  
        mLuaState.Remove(b);
    }
    private static int Traceback(ILuaState lua)
    {
        var msg = lua.ToString(1);
        if (msg != null)
            lua.L_Traceback(lua, msg, 1);
        else if (!lua.IsNoneOrNil(1))
        {
            // is there an error object?  
            // try its `tostring' metamethod  
            if (!lua.L_CallMeta(1, "__tostring"))
                lua.PushString("(no error message)");
        }
        return 1;
    }  
}
