local InputControl  	= require "LuaScript.Scenes.TestUniLua.input_control"
local SceneMgr      	= require "LuaScript.Scenes.TestUniLua.scene_mgr"
local UnityEngine       = require "LuaScript.Library.unity_engine"
local GameObject        = UnityEngine.GameObject
local MeshFilter        = UnityEngine.MeshFilter
local Resources         = UnityEngine.Resources
local Mesh              = UnityEngine.Mesh
local Vector3           = UnityEngine.Vector3
local MeshRenderer      = UnityEngine.MeshRenderer
local Material          = UnityEngine.Material

local function awake()
    print("---- awake ----")
end

local function start()
    print("---- start ----")
    SceneMgr.init_scene()
end

local function update()
    InputControl.update_input()
end

local function late_update()
end

local function fixed_update()
end

return
{
    awake           = awake,
    start           = start,
    update          = update,
    late_update     = late_update,
    fixed_update    = fixed_update,
}
