using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SpellCard_DanceOfStarSparkedWildfire : Card
{
    [SerializeField] private Bullet bulletPrefab;
    public Bullet GetBulletPrefab()
    {
        return bulletPrefab;
    }
    [SerializeField] private uint generateLoopTime = 1;
    public uint GetGenerateLoopTime()
    {
        return generateLoopTime;
    }
    public void SetGenerateLoopTime(uint generateLoopTime)
    {
        this.generateLoopTime = generateLoopTime;
    }
    [SerializeField] private uint generateLineCount = 5;//产生的星星线的条数
    public uint GetGenerateLineCount()
    {
        return generateLineCount;
    }
    public void SetGenerateLineCount(uint generateLineCount)
    {
        this.generateLineCount = generateLineCount;
    }
    [SerializeField] private float startDirection = 0.0f;
    public float GetStartDirection()
    {
        return startDirection;
    }
    public void SetStartDirection(float startDirection)
    {
        this.startDirection = startDirection;
    }
    [SerializeField] private Vector3 interval;
    public void SetInterval(Vector3 interval)
    {
        this.interval = interval;
    }
    public Vector3 GetInterval()
    {
        return interval;
    }
    public Vector3 GetInterval_Copy()
    {
        return new Vector3(interval.x, interval.y, interval.z);
    }
    [SerializeField] private Vector3 intervalAccelerate = new Vector3(0.0f,0.0f,-0.25f);
    public Vector3 GetIntervalAccelerate()
    {
        return intervalAccelerate;
    }
    public Vector3 GetIntervalAccelerate_Copy()
    {
        return new Vector3(intervalAccelerate.x, intervalAccelerate.y, intervalAccelerate.z);
    }
    public void SetIntervalAccelerate(Vector3 intervalAccelerate)
    {
        this.intervalAccelerate = intervalAccelerate;
    }
    //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterPlay()
    {
        base.AfterPlay();
        yield return GenerateStarBullets();
        yield return null;
    }
    public virtual IEnumerator AfterRemoveFromSolt()
    {
        yield return null;
    }
    public override IEnumerator AfterTriggerEffective()
    {
        base.AfterTriggerEffective();
        yield return GenerateStarBullets();
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
    /*
    private IEnumerator GenertaeMasterSpark()
    {
        yield return GetComponent<CardSpellAttackWaker>()?.WakeSpellAttackDisplayer(false);
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
    */
    private IEnumerator GenerateStarBullets()
    {
        Role role = BattleMessage.instance?.GetRole(
            (uint)BattleMessage.instance?.GetControlPlayerID(),
            true
        );
        yield return GetComponent<CardSpellAttackWaker>()?.WakeSpellAttackDisplayer((bool)role?.GetSide());
        //Vector3 ac = new Vector3(0,0,-1.5f);
        for(int i = 0; i < generateLoopTime; i++)
        {
            //ac *= i;
            Vector3 nowInterval = interval * (i+1) + intervalAccelerate * i * i;
            for(int j=0;j<generateLineCount;j++)
            {
                //获取当前的方向
                float nowDirection = startDirection + i * 360.0f / generateLineCount;
                nowInterval = Quaternion.Euler(0.0f,nowDirection,0.0f).normalized * nowInterval;
                yield return GenerateOneBullet(role,nowInterval);
            }
        }
    }

    private IEnumerator GenerateOneBullet(Role role,Vector3 offset)
    {
        yield return BattleMessage.instance?.GenerateBullet(
            role,//传入产生的Role信息,包含位置等
            bulletPrefab,//子弹预设体
            (Vector2Int)ConcentratePoint.instance?.GetIndex(),//目标位置
            offset
        );
    }
}
