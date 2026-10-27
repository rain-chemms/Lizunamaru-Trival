local util = require "xlua.util"
local GameObject = CS.UnityEngine.GameObject
local BulletGenerator = CS.LuaGameSystem.BulletGenerator
local Vector3 = CS.UnityEngine.Vector3
local Quaternion = CS.UnityEngine.Quaternion
local WaitForSeconds = CS.UnityEngine.WaitForSeconds
local BattleMessage = CS.BattleMessage
local BulletSnipeAppender = CS.BulletSystem.BulletSnipeAppender
local BulletForceAppender = CS.BulletSystem.BulletForceAppender

local ForceMode = CS.UnityEngine.ForceMode

local btGr = BulletGenerator()
btGr:SetGenerateObject(btGrRole)
btGr:SetBulletSync("IceCrystal") --操作是异步的需要等待一小会

local btNumber = 12 -- 一圈的子弹数目
local force = 12.5
local haltTime = 1.5
local reForce = 10;

local function AddSniper(bullet)
    if not bullet or not bullet.gameObject then
        print("AddSniper: bullet 或 bullet.gameObject 为空")
        return
    end

    local rb = bullet:GetRigidBody()
    local sniper = bullet.gameObject:AddComponent(typeof(BulletSnipeAppender))
    local forceApd = bullet.gameObject:AddComponent(typeof(BulletForceAppender))
    forceApd.AppendForce = -0.35
    forceApd.AppendPreFrame = true
    if not sniper then
        print("AddSniper: AddComponent 返回 nil，BulletSnipeAppender 可能不是 MonoBehaviour")
        return
    end
    
    if not forceApd then
        print("AddSniper: AddComponent 返回 nil，BulletForceAppender 可能不是 MonoBehaviour")
        return
    end
    
    

    sniper:SetOpenHalt(true)
    sniper:SetHaltTime(haltTime)
    -- 关键：用 bullet 自己的 GameObject 起一个独立协程来等时间
    bullet:StartCoroutine(util.cs_generator(function()
        coroutine.yield(WaitForSeconds(haltTime))
        if bullet then
            forceApd.enabled = false
            bullet:SetForce(reForce)
        end
    end))
end

local co = util.cs_generator(function()
    local gap = 360 / btNumber
    for i = 0, (btNumber - 1), 1 do --产生一圈子弹
        local q = Quaternion.Euler(0,gap * i, 0)
        btGr:GenerateBullet(q * Vector3.forward, force,ForceMode.Force,AddSniper)
    end
end)

btGrRole:StartCoroutine(co)