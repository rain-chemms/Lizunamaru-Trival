using UnityEngine;
using System.Collections;

public class ShootCard : Card
{
        //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterPlay()
    {
        base.AfterPlay();
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
