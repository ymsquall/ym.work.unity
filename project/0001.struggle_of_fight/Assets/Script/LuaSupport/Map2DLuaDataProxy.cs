using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Script.LuaSupport
{
    public class Map2DLuaDataProxy : LuaScriptMonoDelegate
    {
        public string 场景名 = null;
        public string LUA脚本路径 = "LuaScript/Scenes";
        public string LUA脚本入口文件 = "main.lua";
        const string LUASetVariableFuncName = "LUACSCall_SetSceneVar";
        protected override string LUAEntryFileName
        {
            get
            {
                if (null == 场景名 || 场景名.Length <= 0)
                    场景名 = Application.loadedLevelName;
                return string.Format("{0}/{1}/{2}", LUA脚本路径, 场景名, LUA脚本入口文件);
            }
        }
        protected override string[] MethodNameList()
        {
            List<string> ret = new List<string>(0);
            ret.Add(LUASetVariableFuncName);
            return ret.ToArray();
        }
        void Awake()
        {
            base.Awake();
            if (null == 场景名 || 场景名.Length <= 0)
                场景名 = Application.loadedLevelName;
            CallMethod(LUASetVariableFuncName, new string[] { 场景名 });

        }
        void Update()
        {

        }
    }
}
