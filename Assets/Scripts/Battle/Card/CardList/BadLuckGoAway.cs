using UnityEngine;
using CardSystem;
using System.Collections;

// 霉运厄运快转开:(相关角色键山雏)效果卡
// 抽三张卡牌并丢去一张卡牌
namespace CardSystem.AllCardHub
{
    public class BadLuckGoAway : Card
    {
        [SerializeField] private uint drawCardCount = 3;
        [SerializeField] private uint discardCardCount = 1;
        public override IEnumerator AfterPlay()
        {
            //抽三张卡牌
            yield return BattleMessage.instance?.DrawCard((int)drawCardCount);
            //传入执行函数:弃牌
            HandCardOperator.instance.SetOperateFunc(DiscardCard);
            //激活手牌执行器
            yield return HandCardOperator.instance?.CallTheHandCardOperator(discardCardCount,CardOperateCategory.AT_LEAST);
        }
    
        private IEnumerator DiscardCard(Card card)
        {
            yield return BattleMessage.instance?.DiscardCard(card);
        }
    }
}
