using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardSystem.CardPoolSystem;
using System.Linq;

namespace CardSystem.AllCardHub
{
    //相关角色:古明地恋
    //打出时:从{0}张完全随机的卡牌中选择最多{1}张加入手牌并且获得它们的费用总和,它具有消耗,虚无
    public class Effective_BrilliantIdeas : Card
    {
        [SerializeField] private uint selectNumber = 2;
        public uint GetSelectNumber() => selectNumber;
        public void SetSelectNumber(uint num) => selectNumber = num;
 
        [SerializeField] private uint checkNumber = 5;
        public uint GetCheckNumber() => checkNumber;
        public void SetCheckNumber(uint num) => checkNumber = num;

        [SerializeField] private static int playTime = 0;
        public static int GetPlayTime() => playTime;

        public override IEnumerator AfterPlay()
        {
            //生成待选卡组
            List<Card> willSelectCard = new List<Card>();
            for(int i = 0;i < checkNumber;i++)
            {
                Card prefab = CardPoolManager.instance?.GetRandomCardFromPool(i + playTime,null);//完全随机的选择卡牌
                if(prefab == null) continue;
                //生成卡牌的实体
                Card cardEntity = Instantiate(prefab,BattleMessageDisplayer.instance?.transform);
                willSelectCard.Add(cardEntity);
            }
            //连接选择方法
            CardSelectOperator.instance.SetOperateFunc(AddCardToHandAndAddKeyWordsAndAddRicePoint);
            //将所有的抽牌堆中的卡牌加入选择列表中
            List<(Card,Transform)> cpList = new List<(Card,Transform)>();
            foreach(Card card in willSelectCard)
            {
                if(card == null) continue;
                Transform parent = card.transform.parent;
                cpList.Add((card,parent));
            }
            //调起CardSelectOperator
            yield return CardSelectOperator.instance?.CallTheCardSelectOperator(selectNumber,CardOperateCategory.AT_MOST,cpList);
            //将没有被选中加入到牌组中的牌直接删除
            foreach(Card card in willSelectCard?.ToList())
            {
                if(card == null) continue;
                if(!(bool)BattleMessage.instance?.IsCardInHand(card)
                && !(bool)BattleMessage.instance?.IsCardInDiscardStack(card)
                && !(bool)BattleMessage.instance?.IsCardInDrawStack(card)
                && !(bool)BattleMessage.instance?.IsCardInExhaustStack(card)
                && !(bool)BattleMessage.instance?.IsCardInSlot(card))//不是战斗系统可管理的卡牌
                {
                    Destroy(card.gameObject);
                }
            }
            playTime ++;
            yield return base.AfterPlay();
        }

        private IEnumerator AddCardToHandAndAddKeyWordsAndAddRicePoint(Card card)
        {
            if(card == null) yield break;
            if(!(bool)card.GetCardKeyWords()?.Contains(CardKeyWord.EXHAUST)) card.AddCardKeyWord(CardKeyWord.EXHAUST);
            if(!(bool)card.GetCardKeyWords()?.Contains(CardKeyWord.ETHEREAL)) card.AddCardKeyWord(CardKeyWord.ETHEREAL);
            BattleMessage.instance?.SetRicePoint((uint)BattleMessage.instance?.GetRicePoint() + card.GetRiceCost());
            yield return BattleMessage.instance?.AddExistCardToHand(card);//添加到手牌中        
            //放回原先的父物体中
            card.transform.SetParent(card.transform.parent);    
        }
        
    }
}
