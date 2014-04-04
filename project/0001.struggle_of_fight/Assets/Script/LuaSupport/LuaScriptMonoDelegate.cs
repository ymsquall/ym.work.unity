using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniLua;

namespace Assets.Script.LuaSupport
{
    public class LuaScriptMonoDelegate : MonoBehaviour
    {
        protected virtual string LUAEntryFileName { get { return null; } }
        protected virtual string[] MethodNameList() { return null; }
        protected void Awake()
        {
            if (mLuaState == null)
            {
                mLuaState = LuaAPI.NewState();
                mLuaState.L_OpenLibs();
                if(null != LUAEntryFileName && LUAEntryFileName.Length > 0)
                {
                    var status = mLuaState.L_DoFile(LUAEntryFileName);
                    if (status != ThreadStatus.LUA_OK)
                        throw new Exception(mLuaState.ToString(-1));
                    if (!mLuaState.IsTable(-1))
                        throw new Exception("entry main's return value is not a table");
                    string[] funcList = MethodNameList();
                    if (null != funcList)
                    {
                        foreach (string s in funcList)
                        {
                            if(mLuaFunctionRefs.ContainsKey(s))
                                mLuaFunctionRefs.Remove(s);
                            int funcRef = StoreMethod(s);
                            mLuaFunctionRefs.Add(s, funcRef);
                        }
                    }
                    mLuaState.Pop(1);
                }
            }
        }
        private int StoreMethod(string name)
        {
            mLuaState.GetField(-1, name);
            if (!mLuaState.IsFunction(-1))
                throw new Exception(string.Format(
                    "method {0} not found!", name));
            return mLuaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        }
        protected string CallMethod<T>(string[] plist)
        {
            string key = typeof(T).ToString();
            if (!mLuaFunctionRefs.ContainsKey(key))
            {
                return "";
            }
            return CallMethod(mLuaFunctionRefs[key], plist);
        }
        protected string CallMethod(string key, string[] plist)
        {
            if (!mLuaFunctionRefs.ContainsKey(key))
            {
                return "";
            }
            return CallMethod(mLuaFunctionRefs[key], plist);
        }
        private string CallMethod(int funcRef, string[] plist)
        {
            mLuaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
            // insert `traceback' function  
            var b = mLuaState.GetTop();
            mLuaState.PushCSharpFunction(Traceback);
            mLuaState.Insert(b);
            int pCount = 0;
            if(null != plist)
            {
                foreach(string s in plist)
                {
                    mLuaState.PushString(s);
                    pCount++;
                }
            }
            var status = mLuaState.PCall(pCount, 1, b);
            if (status != ThreadStatus.LUA_OK)
                Debug.LogError(mLuaState.ToString(-1));
            // remove `traceback' function
            mLuaState.Remove(b);
            // return value
            int retValue = mLuaState.GetTop();
            return mLuaState.ToString(retValue);
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

        private ILuaState mLuaState;
        Dictionary<string, int> mLuaFunctionRefs = new Dictionary<string,int>();
    }
}