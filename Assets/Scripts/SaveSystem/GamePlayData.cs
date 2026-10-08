using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSaveSystem
{
    //玩家的游玩数据,便于复现和重新载入关卡
    [Serializable]
    public class GamePlayData
    {
        //当前这一关的状态,仅当当前关卡为战斗类关卡时生效
        public enum LevelBattleState
        {
            LEVEL_BATTLE_OVER,//直接生成战斗之后的奖励
            LEVEL__BATTLE_START//战斗刚刚开始,依据当前关卡索引对应的战斗生成敌人
        }
        [SerializeField] public LevelBattleState battleState;//仅当当前关卡是战斗关卡时生效
        [SerializeField] public long startTime;//这局游戏开始的时间
        [SerializeField] public double duration;//这局的游戏时长
        [SerializeField] public string seed;//游戏种子
        [SerializeField] public List<string> cardHub;//玩家的卡组    
        [SerializeField] public string selectRole;//玩家选择的角色,代表角色名,可以通过这个获取角色的初始数据
        [SerializeField] public float maxHp;//最大生命值
        [SerializeField] public float currentHp;//当前的生命值
        [SerializeField] public uint coins;//金币数
        [SerializeField] public MapAreaCategory mapArea;//玩家所处的地图区域
        [SerializeField] public Vector2Int mapIndex;//玩家所处的地图索引
    }
}