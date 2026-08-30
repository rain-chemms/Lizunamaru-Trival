using System.Collections;

namespace CardSystem.AllCardHub
{
    public class Effective_GadgetSlotIncreaser : Card
    {
        public override IEnumerator AfterPlay()
        {
           //增加卡槽数量
            SerializableDictionary<CardCategory,int> dict = BattleMessage.instance?.GetCardSlotListCardSlotCount(); 
            if(dict.ContainsKey(CardCategory.GADGET))
            {
                dict[CardCategory.GADGET] += 1;//增加卡槽数量
            }
            //刷新卡槽显示
            BattleMessageDisplayer.instance?.GetComponent<BattleCardSlotListController>()?.FreshCardSlotListCount();
            yield return base.AfterPlay();
        }
    }    
}