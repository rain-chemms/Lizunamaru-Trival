using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CardSystem.AllCardHub
{ 
    //内心潜能:相关角色:古明地觉
    //2费,保留,消耗
    //抽到这张牌时,抽{0}张牌
    //打出时,选择手中至多 {1} 张能力卡或符卡,将其 {2} 张复制品加入手中
    public class Effective_InnerPotential : Card
    {
        [SerializeField] private uint selectCount = 1;//选择的能力牌的数量
        public uint GetSelectCount() => selectCount;
        public void SetSelectCount(uint count) => selectCount = count;

        [SerializeField] private uint copyCount = 1;//每张牌复制的数量
        public uint GetCopyCount() => copyCount;
        public void SetCopyCount(uint count) => copyCount = count;
        
        [SerializeField] private int drawCount = 1;//抽到这张牌之后抽牌数量
        public int GetDrawCount() => drawCount; 
        public void SetDrawCount(int count) => drawCount = count;


        public override IEnumerator AfterPlay()
        {
            //传入执行函数:弃牌
            HandCardOperator.instance?.SetOperateFunc(CopyCardAndAddToHand);
            //设置Filter
            HandCardOperator.instance?.ClearCardFilter();
            foreach(CardCategory ctg in Enum.GetValues(typeof(CardCategory)))
            {
                //添加所有非Power的类型作为过滤器
                if(ctg != CardCategory.POWER && ctg != CardCategory.SPELL_ATTACK) HandCardOperator.instance?.AddCardCategoryFilter(ctg);
            }
            yield return HandCardOperator.instance?.CallTheHandCardOperator(selectCount,CardOperateCategory.AT_LEAST);
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterDraw()
        {
            yield return BattleMessage.instance.DrawCard(drawCount);
            yield return base.AfterDraw();
        }

        private IEnumerator CopyCardAndAddToHand(Card card)
        {
            if(card == null) yield break;
            List<Card> newCardList = new List<Card>();
            for(int i = 0;i < copyCount; i++)
            {
                Card newCard = Instantiate(card,BattleMessageDisplayer.instance?.transform);//创建一张新的牌,并设置其父物体为战斗面板
                if(newCard != null) newCardList.Add(newCard);    
            }
            foreach(Card c in newCardList)
            {
                yield return BattleMessage.instance?.AddExistCardToHand(c);//将新生成的牌添加到手牌中
            }
            //将选择的卡返回手中
            yield return BattleMessage.instance?.AddExistCardToHand(card);
        }
    }
}
