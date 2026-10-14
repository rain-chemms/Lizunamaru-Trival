--[[
    btGrRole是全局的变量,默认会在XLuaAction的Excute中自动赋值,赋值后可以直接使用
--]]
--[[
local util = require "xlua.util"
local BulletGenerator = CS.LuaGameSystem.BulletGenerator
local Vector3 = CS.UnityEngine.Vector3
local Quaternion = CS.UnityEngine.Quaternion
local WaitForSeconds = CS.UnityEngine.WaitForSeconds

local btGr = BulletGenerator.New()
btGr:SetRole(btGrRole)
btGr:SetBullet("StarBullet")

local round = 720 --子弹要旋转几度
local step = 7.5 --步长
local wait = 0.25 --每次发射后的等待时间

local startDir = Vector3.New(1,0,0).normalized

for i = 0, round, step do 
    local q = Quaternion.Euler(0,i * step,0)
    btGr:GenerateBullet(q * startDir)
    btGr:GenerateBullet(- q * startDir)
    coroutine.yield(WaitForSeconds(wait))
    --GenerateBullet(float force,Vector3 direction,ForceMode forceMode = default,Action<Bullet> selfPostProcess = null,Vector3 posOffset = default,Vector3 scale = default)
end
--]]

--[[
    btGrRole 是全局变量，默认会在 XLuaAction 的 Excute 中自动赋值，赋值后可以直接使用。
    这段代码需要放在一个由 XLua 协程调度器驱动的环境里执行，
    例如通过 util.cs_generator 包装后交给 MonoBehaviour.StartCoroutine 启动。
--]]

print("[DebugBulletGenerate]: Start Generate!")

local util = require "xlua.util"
local GameObject = CS.UnityEngine.GameObject
local BulletGenerator = CS.LuaGameSystem.BulletGenerator
local Vector3 = CS.UnityEngine.Vector3
local Quaternion = CS.UnityEngine.Quaternion
local WaitForSeconds = CS.UnityEngine.WaitForSeconds

print("[DebugBulletGenerate]: Class GetOver!")

local btGr = BulletGenerator()
btGr:SetRole(btGrRole)
btGr:SetBullet("StarBullet")

print("[DebugBulletGenerate]: BTGR Create Over!")

local round = 720        -- 总共旋转多少度
local step = 20         -- 每次增加的角度
local wait = 0.01        -- 每次发射后等待的秒数
local force = 30         -- 子弹发射力度，按实际 API 调整

local startDir = Vector3(1, 0, 0).normalized

print("[DebugBulletGenerate]: Parameter Set Over!")

local co = util.cs_generator(function()
    for angle = 0, round, step do
        local q = Quaternion.Euler(0, angle, 0)

        btGr:GenerateBullet(q * startDir, force)
        btGr:GenerateBullet(q * -startDir, force)

        coroutine.yield(WaitForSeconds(wait))
    end
end)

-- 使用当前的角色启动协程
btGrRole:StartCoroutine(co)