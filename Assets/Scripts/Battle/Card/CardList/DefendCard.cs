using System.Collections;
using UnityEngine;

public class DefendCard : Card
{
    [SerializeField] private int gainDefendPoint = 1;
    //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterPlay()
    {
        base.AfterPlay();
        //让玩家获取格挡值
        BattleMessage.instance?.GetRole(
            (uint)BattleMessage.instance?.GetControlPlayerID(),
            true
        )?.GetComponent<RoleDefendGetter>()?.GetDefend(gainDefendPoint);
        yield return null;
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
    public virtual IEnumerator AfterDiscard()
    {
        yield return null;    
    }
    
    //在抽到卡牌时触发
    public virtual IEnumerator AfterDraw()
    {
        yield return null;
    } 
}
