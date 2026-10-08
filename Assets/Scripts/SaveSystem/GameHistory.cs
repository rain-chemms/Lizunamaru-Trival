using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace GameSaveSystem
{
    //玩家游玩的历史数据,记录玩家在当前的档案下多局的游玩记录
    [Serializable]
    public class GameHistory
    {
        [Serializable]
        public struct HistoryData
        {
            [SerializeField] public long startTime;//这局游戏开始的时间
            [SerializeField] public double duration;//这局的游戏时长 
            [SerializeField] public string seed;//游戏种子
            [SerializeField] public List<string> cardHub;//玩家的卡组
            [SerializeField] public string selectRole;//玩家选择的角色,代表角色名,可以通过这个获取角色的初始数据
            [SerializeField] public MapAreaCategory endMapArea;//玩家所处的地图区域
            [SerializeField] public Vector2Int endMapIndex;//玩家所处的地图索引
            [SerializeField] public float endMaxHp;//最大生命值
            [SerializeField] public float endHp;//当前的生命值
            [SerializeField] public uint endCoins;//金币数 
            public HistoryData(GamePlayData sourceData)
            {
                startTime = sourceData.startTime;
                duration = sourceData.duration;
                seed = sourceData.seed;
                cardHub = sourceData.cardHub.ToList();
                selectRole = sourceData.selectRole;
                endMapArea = sourceData.mapArea;
                endMapIndex = sourceData.mapIndex;
                endMaxHp = sourceData.maxHp;
                endHp = sourceData.currentHp;
                endCoins = sourceData.coins;
            }
        }

        [SerializeField] public List<HistoryData> historys;
    }
}