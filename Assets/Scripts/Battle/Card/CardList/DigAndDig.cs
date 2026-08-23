using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace CardSystem.AllCardHub
{
    // 挖呀挖:(相关角色:姬虫百百世)效果卡
    // 打出后,从抽牌堆中选择两张牌加入手牌中
    // 含有"消耗"关键字
    // 升级后也可以从弃牌堆中选择,并且去除消耗
    public class DigAndDig : Card
    {
        [SerializeField] private uint searchCount = 2;
        public uint GetSearchCount() => searchCount;
        public void SetSearchCount(uint count) => searchCount = count;

        public override IEnumerator AfterPlay()
        {
            //连接选择方法
            CardSelectOperator.instance.SetOperateFunc(AddTheDrawListCardToHand);
            //将所有的抽牌堆中的卡牌加入选择列表中
            List<(Card,Transform)> cpList = new List<(Card,Transform)>();
            foreach(Card card in BattleMessage.instance?.GetDrawCardList())
            {
                if(card == null) continue;
                Transform parent = card.transform.parent;
                cpList.Add((card,parent));
            }
            //调起CardSelectOperator
            yield return CardSelectOperator.instance.CallTheCardSelectOperator(searchCount,CardOperateCategory.AT_LEAST,cpList);
            yield return base.AfterPlay();
        }

        private IEnumerator AddTheDrawListCardToHand(Card card)
        {
            //若当前卡牌在抽牌堆中
            yield return BattleMessage.instance?.GetDrawCardList()?.Remove(card);//移除抽牌堆中的当前卡牌    
            yield return BattleMessage.instance?.AddExistCardToHand(card);//添加到手牌中
            Debug.Log("[DigAndDig]: Excute Card" + card.name);
            //放回原先的父物体中
            card.transform.SetParent(card.transform.parent);
        }
    }
}