using UnityEngine;
using System.Collections;

public class SpellCard_MasterSpark : Card
{
    [SerializeField] private Bullet bulletPrefab;
    //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterPlay()
    {
        base.AfterPlay();
        yield return GenertaeMasterSpark();
        yield return null;
    }
    public virtual IEnumerator AfterRemoveFromSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterTriggerEffective()
    {
        base.AfterTriggerEffective();
        yield return GenertaeMasterSpark();
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

    private IEnumerator GenertaeMasterSpark()
    {
        Role role = BattleMessage.instance?.GetRole(
            (uint)BattleMessage.instance?.GetControlPlayerID(),
            true
        );
        yield return GetComponent<CardSpellAttackWaker>()?.WakeSpellAttackDisplayer((bool)role?.GetSide());
        yield return BattleMessage.instance?.GenerateBullet(
            role,//传入产生的Role信息,包含位置等
            bulletPrefab,//子弹预设体
            (Vector2Int)ConcentratePoint.instance?.GetIndex(),//目标位置
            default
        );
    }
}
