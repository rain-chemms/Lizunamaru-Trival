using System.Collections;
using UnityEngine;

namespace CardSystem.AllCardHub
{
    //相关角色:藤原妹红 
    //消耗1张手牌,抽{0}张牌
    public class Effective_Flame : Card
    {
        [SerializeField] private int drawCount;
        public int GetDrawCount() => drawCount;
        public void SetDrawCount(int count) => drawCount = count;

        public override IEnumerator AfterPlay()
        {
            //传入执行函数:烧牌
            HandCardOperator.instance?.SetOperateFunc(ExhaustCard);
            //设置Filter为空
            HandCardOperator.instance?.ClearCardFilter();
            //激活手牌执行器,数量为1
            yield return HandCardOperator.instance?.CallTheHandCardOperator(1,CardOperateCategory.AT_LEAST);
            yield return BattleMessage.instance.DrawCard(drawCount);
            yield return base.AfterPlay();
        }

        private IEnumerator ExhaustCard(Card card)
        {
            yield return BattleMessage.instance.ExhaustCard(card);
        }
    }
}
