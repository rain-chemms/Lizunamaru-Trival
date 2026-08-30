using System.Collections;

namespace CardSystem.AllCardHub
{
    public class Effective_AttackSlotIncreaser : Card
    {
        public override IEnumerator AfterPlay()
        {
           //增加卡槽数量
            SerializableDictionary<CardCategory,int> dict = BattleMessage.instance?.GetCardSlotListCardSlotCount(); 
            if(dict.ContainsKey(CardCategory.ATTACK))
            {
                dict[CardCategory.ATTACK] += 1;//增加卡槽数量
            }
            //刷新卡槽显示
            BattleMessageDisplayer.instance?.GetComponent<BattleCardSlotListController>()?.FreshCardSlotListCount();
            yield return base.AfterPlay();
        }
    }    
}