--[[
//产生子弹的基础示例
local round = 720        -- 总共旋转多少度
local step = 20         -- 每次增加的角度
local wait = 0.01        -- 每次发射后等待的秒数
local force = 30         -- 子弹发射力度，按实际 API 调整

local startDir = Vector3(1, 0, 0).normalized
local co = util.cs_generator(function()
    for angle = 0, round, step do
        local q = Quaternion.Euler(0, angle, 0)

        btGr:GenerateBullet(q * startDir, force)
        btGr:GenerateBullet(q * -startDir, force)

        coroutine.yield(WaitForSeconds(wait))
    end
end)
]] 
--[[
    琪露诺第一回合的行动的冰晶射击
--]] 
print("[Cirno_Action1_IceCrystalShoot]: Start Generate!")

local util = require "xlua.util"
local GameObject = CS.UnityEngine.GameObject
local BulletGenerator = CS.LuaGameSystem.BulletGenerator
local Vector3 = CS.UnityEngine.Vector3
local Quaternion = CS.UnityEngine.Quaternion
local WaitForSeconds = CS.UnityEngine.WaitForSeconds
local BattleMessage = CS.BattleMessage

print("[Cirno_Action1_IceCrystalShoot]: Init Class!")

local btGr = BulletGenerator()
btGr:SetRole(btGrRole)
btGr:SetBulletSync("IceCrystal") --操作是异步的需要等待一小会

print("[Cirno_Action1_IceCrystalShoot]: BTGR Create Over!")

local targetDir = BattleMessage.instance:GetNearestEnermyDirection(btGrRole) -- 获取最近的敌方玩家
local startBtNumber = 3 -- 初始产生子弹数
local loopTime = 6 -- 循环6次

local force = 15;
local forceLow = 2;
local gapOfRot = 7.5; -- 每颗子弹差的角度
local wait = 1

----[[
local co = util.cs_generator(function()
    --coroutine.yield(WaitForSeconds(wait)) --启动协程等待子弹预制体加载
    local focl = force
    for i = 0, loopTime - 1, 1 do -- 产生子弹loopTime行
        local lp = startBtNumber + i - 1
        local center = lp / 2 -- 计算中心索引
        for j = 0, lp, 1 do -- 产生每一行子弹
            local q = Quaternion.Euler(0, gapOfRot * (j - center), 0)
            btGr:GenerateBullet(q * targetDir, focl)
        end
        focl = focl - forceLow
    end
end)
btGrRole:StartCoroutine(co)
--]]

--[[
local round = 720        -- 总共旋转多少度
local step = 20         -- 每次增加的角度
local wait = 0.05        -- 每次发射后等待的秒数
local force = 5         -- 子弹发射力度，按实际 API 调整

local startDir = Vector3(1, 0, 0).normalized
local co = util.cs_generator(function()
    for angle = 0, round, step do
        local q = Quaternion.Euler(0, angle, 0)

        btGr:GenerateBullet(q * startDir, force)
        btGr:GenerateBullet(q * -startDir, force)

        coroutine.yield(WaitForSeconds(wait))
    end
end)
--]]

-- 使用当前的角色启动协程

