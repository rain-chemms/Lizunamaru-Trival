using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShootCard : Card
{
    [SerializeField] private Bullet bulletPrefab;
    public Bullet GetBulletPrefab()
    {
        return bulletPrefab;
    }
    public void SetBulletPrefab(Bullet newBulelt)
    {
        this.bulletPrefab = newBulelt;
    }
    //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterPlay()
    {
        /*
            整个方法可以包装未一个从玩家处产生子弹的协程
        */
        base.AfterPlay();
        //调起子弹产生协程并传入参数
        yield return BattleMessage.instance?.GenerateBullet(
            BattleMessage.instance?.GetRole(
                (uint)BattleMessage.instance?.GetControlPlayerID(),
                true
            ),//传入产生的Role信息,包含位置等
            bulletPrefab,//子弹预设体
            (Vector2Int)ConcentratePoint.instance?.GetIndex(),//目标位置
            default
        );
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
