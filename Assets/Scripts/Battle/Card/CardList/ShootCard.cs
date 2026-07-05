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
        //获取玩家脚本
        Role player = BattleMessage.instance?.GetRole(
            (uint)BattleMessage.instance?.GetControlPlayerID(),
            true
        );
        if (player == null)
        {
            Debug.LogError("[ShootCard]: Control Player is Null, Please Check!");
            yield return null;
        }
        //获取玩家选择的射击地块索引
        Vector2Int index = (Vector2Int)ConcentratePoint.instance?.GetIndex();
        //依据选择的地块和玩家当前的高度执行射击
        //获取对应索引的棋盘格的XZ坐标
        List<BattleGrid> grids = BattleBoard.instance?.GetBattleGridList();
        BattleGrid grid = null;
        foreach (BattleGrid g in grids)
        {
            if (g == null) continue;
            if (g.GetIndex().x == index.x && g.GetIndex().y == index.y)
            {
                grid = g;
                break;
            }
        }
        Vector3 target = new Vector3(
            grid == null ? 0.0f : (float)grid?.transform.position.x,
            player == null ? 0.0f : (float)player?.transform.position.y,
            grid == null ? 0.0f : (float)grid?.transform.position.z
        );
        Vector3 direction = (target - (Vector3)player?.transform.position).normalized;
        //产生子弹实体并设置其方向和初始位置
        Bullet bt = Instantiate(bulletPrefab, target, Quaternion.identity);
        if (bt != null)
        {
            bt.SetSide(player.GetSide());//设置子弹的阵营
            bt.transform.position = (Vector3)player?.transform.position;
            bt.SetDirection(direction);//设置子弹的方向
        }
        //控制玩家动画播放
        RoleAnimTrigger animTrigger = player?.GetComponent<RoleAnimTrigger>();
        animTrigger?.TriggerAnim("Skill");
        AnimatorStateInfo stateInfo = (AnimatorStateInfo)((Animator)animTrigger?.GetComponent<Animator>())?.GetCurrentAnimatorStateInfo(0); 
        yield return (float)stateInfo.normalizedTime * (float)stateInfo.length;
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
