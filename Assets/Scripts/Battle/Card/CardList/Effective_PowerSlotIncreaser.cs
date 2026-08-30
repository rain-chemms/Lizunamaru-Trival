using System.Collections;

namespace CardSystem.AllCardHub
{
    public class Effective_PowerSlotIncreaser : Card
    {
        public override IEnumerator AfterPlay()
        {
           //增加卡槽数量
            SerializableDictionary<CardCategory,int> dict = BattleMessage.instance?.GetCardSlotListCardSlotCount(); 
            if(dict.ContainsKey(CardCategory.POWER))
            {
                dict[CardCategory.POWER] += 1;//增加卡槽数量
            }
            //刷新卡槽显示
            yield return BattleMessageDisplayer.instance?.GetComponent<BattleCardSlotListController>()?.FreshCardSlotListCount();
            yield return base.AfterPlay();
        }
    }    
}
