using UnityEngine;
using System.Collections;
using GridObjectSystem.GadgetSystem;
using System.Collections.Generic;
using System.Linq;
using GridObjectSystem;
using GridObjectSystem.GadgetSystem.YinyangJades;


namespace CardSystem.AllCardHub
{
    //卡牌: 热到融化(相关角色,琪露诺)
    public class Effective_HotToMelt : Card
    {
        [SerializeField] private uint exhaustPowerSlotCount = 1;
        [SerializeField] private int drawCount = 2;
        [SerializeField] private uint getIcePoint = 1;
        public override IEnumerator AfterPlay()
        {
            //消耗一个能力卡槽,抽两张牌,获得1点IcePoint
            yield return ExhaustThePowerSlot(exhaustPowerSlotCount);
            //增加寒冰点数
            BattleMessage.instance?.SetIcePoint((uint)BattleMessage.instance?.GetIcePoint() + getIcePoint);
            //抽牌
            yield return BattleMessage.instance.DrawCard(drawCount);
            yield return base.AfterPlay();
        }

        private IEnumerator ExhaustThePowerSlot(uint number)
        {
            SerializableDictionary<CardCategory,int> dict = BattleMessage.instance?.GetCardSlotListCardSlotCount();//获取卡槽数量
            ///*
            if((bool)dict?.ContainsKey(CardCategory.POWER))
            {
                dict[CardCategory.POWER] -= (int)number;
                if(dict[CardCategory.GADGET] < 0) dict[CardCategory.GADGET] = 0;
            }
            //*/
            /*用Gadget进行测试
            if((bool)dict?.ContainsKey(CardCategory.GADGET))
            {
                dict[CardCategory.GADGET] -= (int)number;
                if(dict[CardCategory.GADGET] < 0) dict[CardCategory.GADGET] = 0;
            }
            //*/
            yield return BattleMessageDisplayer.instance?.GetComponent<BattleCardSlotListController>()?.FreshCardSlotListCount();//刷新卡槽显示
        }
    }
}