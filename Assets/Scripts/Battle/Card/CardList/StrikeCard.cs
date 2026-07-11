using System.Collections;
using UnityEngine;

//卡牌效果:在角色前面一格产生一个剑刃子弹
public class StrikeCard : Card
{
    [SerializeField] private Bullet swordEdge;
    public void SetSwordEdge(Bullet swordEdge)
    {
        this.swordEdge = swordEdge;
    }
    public Bullet GetSwordEdge()
    {
        return swordEdge;
    }
    //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterPlay()
    {
        base.AfterPlay();
        Role player = BattleMessage.instance?.GetRole(
            (uint)BattleMessage.instance?.GetControlPlayerID(),
            true
        );
        if(player == null) yield break;
        BattleDirection dr =player.GetDirection();
        Vector2Int targetIndex = player.GetGridIndex();
        Vector3 offset = Vector3.zero;
        Vector2 _gaps = (Vector2)BattleBoard.instance?.GetGapsOfGrid();
        switch(dr)
        {
            case BattleDirection.DOWN:
                targetIndex.y -= 1;
                offset = new Vector3(0.0f, 0.0f, -_gaps.y);
                break;
            case BattleDirection.LEFT:
                targetIndex.x -= 1;
                offset = new Vector3(-_gaps.x, 0.0f, 0.0f);
                break;
            case BattleDirection.RIGHT:
                targetIndex.x += 1;
                offset = new Vector3(_gaps.x, 0.0f, 0.0f);
                break;
            case BattleDirection.UP:
            default:
                targetIndex.y += 1;
                offset = new Vector3(0.0f, 0.0f, _gaps.y);
                break;
        }
        yield return BattleMessage.instance?.GenerateBullet(
            player,//传入产生的Role信息,包含位置等
            swordEdge,//子弹预设体
            targetIndex,//目标位置
            offset,
            true,
            "Attack"
        );
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
