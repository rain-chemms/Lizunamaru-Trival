using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//空白卡牌
public class WhiteEmptyCard : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterPlay()
    {
        base.AfterPlay();
        List<Card> cardList = BattleMessage.instance?.GetHandCardList()?.ToList();
        if(cardList == null)
        {
            Debug.LogError("[WhiteEmptyCard]: The HandCardList is null, Please Check!");
            yield return null;
        }
        foreach(Card card in cardList)
        {
            yield return (BattleMessage.instance?.DiscardCard(card));
        }
        //回复相应的麦饭点数
        BattleMessage.instance?.SetRicePoint((uint)BattleMessage.instance?.GetRicePoint() + (uint)cardList?.Count);
        yield return 1.5f;//时间间隔
    }
    public virtual IEnumerator AfterRemoveFromSolt()
    {
        yield return null;
    }
    public virtual IEnumerator AfterTriggerEffective()
    {
        yield return null;
    }
    public virtual IEnumerator AfterRoundEnd()
    {
        yield return null;
    }
    //回合开始时触发
    public virtual IEnumerator AfterRoundStart()
    {
        yield return null;
    }

    //在你的回合丢弃时触发
    public virtual IEnumerator AfterDsicard()
    {
        yield return null;    
    }
    
    //在抽到卡牌时触发
    public virtual IEnumerator AfterDraw()
    {
        yield return null;
    } 
}
