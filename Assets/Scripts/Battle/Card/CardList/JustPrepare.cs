using UnityEngine;
using System.Collections;

namespace CardSystem.AllCardHub
{
    //只是准备一下:(相关角色 十六夜咲夜)效果卡
    //先抽两张牌,然后弃掉两张牌
    public class JustPrepare : Card
    {
        [SerializeField] private uint drawCardCount = 2;
        [SerializeField] private uint discardCardCount = 2;
        public override IEnumerator AfterPlay()
        {
            yield return base.AfterPlay();
            //抽牌
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