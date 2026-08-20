using UnityEngine;
using System.Collections;

namespace CardSystem.AllCardHub
{
    // 无意识闪避: (相关角色古明地恋)效果卡
    // 抽到这张卡时,抽1张牌,丢弃这张牌时,角色增加一点能量
    // 消耗时: 同时触发加费和抽牌
    // 无法打出
    
    public class Intuition : Card
    {
        [SerializeField] private uint gainEnergyPoint = 2;
        
        public void SetGainEnergyPoint(uint point) => gainEnergyPoint = point;
        public uint GetGainEnergyPoint() => gainEnergyPoint;

        [SerializeField] private uint drawCardCount = 1;
        public void SetGainCardCount(uint count) => drawCardCount = count;
        public uint GetGainCardCount() => drawCardCount;
        
        public override IEnumerator AfterDiscard()
        {
            yield return base.AfterDiscard();
            yield return GainEnergy();
        }

        public override IEnumerator AfterDraw()
        {
            yield return base.AfterDraw();
            yield return DrawCard();
        }

        public override IEnumerator AfterExhaust()
        {
            yield return base.AfterExhaust();
            yield return GainEnergy();
            yield return DrawCard();
        }

        //功能函数: 获取能量
        private IEnumerator GainEnergy()
        {
            BattleMessage instance = BattleMessage.instance;
            instance?.SetRicePoint((uint)instance?.GetRicePoint() + gainEnergyPoint);
            yield return null;   
        }
        //功能函数: 抽牌
        private IEnumerator DrawCard()
        {
            yield return BattleMessage.instance?.DrawCard((int)drawCardCount);
        }
    }
}