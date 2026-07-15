using Unity.VisualScripting;
using UnityEngine;
using System;

//角色脚本,用于控制角色的数据
[RequireComponent(typeof(Rigidbody))]
public class Role : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    public Rigidbody GetRigidBody()
    {
        return rb;
    }
    void Start()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
    }
    [SerializeField] private bool roundOperateEnd = false;//玩家本回合内的操作是否结束
    public bool IsRoundOperateEnd()
    {
        return roundOperateEnd;
    }
    public void SetRoundOperateEnd(bool roundOperateEnd)
    {
        this.roundOperateEnd = roundOperateEnd;
    }

    /*
        有一个协程,时刻依据BattleMessage.instance.isPlayerTrun检测玩家列表中的玩家是否操作结束
        若playerTurn情况下,当前side的所有玩家的操作都结束了,则切换到敌方回合
    */
    [SerializeField] private BattleDirection direction = BattleDirection.RIGHT;//角色的朝向
    public BattleDirection GetDirection()
    {
        return direction;
    }
    public void SetDirection(BattleDirection direction)
    {
        this.direction = direction;
    }
    [SerializeField] private uint id = 0;
    public uint GetID()
    {
        return id;
    }
    public void SetID(uint id)
    {
        this.id = id;    
    }
    [SerializeField] private bool side = true;//角色的阵营
    public void SetSide(bool nowSide)
    {
        side = nowSide;
    }
    public bool GetSide()
    {
        return side;
    }

    [SerializeField] private float hp = 100.0f;//角色的生命值
    public void SetHp(float nowHp)
    {
        hp = nowHp;
    }
    public float GetHp()
    {
        return hp;
    }
    [SerializeField] private uint defend = 0;//角色的防御点数,每1点可以格挡一次伤害
    public uint GetDefend()
    {
        return defend;
    }
    public void SetDefend(uint nowDefend)
    {
        defend = nowDefend;
    }
    [SerializeField] private float maxHp = 100.0f;//角色最大生命值
    public float GetMaxHp()
    {
        return maxHp;
    }
    public void SetMaxHp(float nowMaxHp)
    {
        maxHp = nowMaxHp;
    }
    [NonSerialized] private float speed = 5.0f;//角色当前的速度
    public float GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(float nowSpeed)
    {
        speed = nowSpeed;
    }
    [SerializeField] private bool isFly = false;//是否正在飞行
    public bool IsFly()
    {
        return isFly;
    }
    public void SetFly(bool isFly)
    {
        this.isFly = isFly;
    }
    [SerializeField] private Vector2Int gridIndex = Vector2Int.zero;//玩家当前所处的棋盘格索引 

    public Vector2Int GetGridIndex()
    {
        return new Vector2Int(gridIndex.x,gridIndex.y);
    }

    public void SetGridIndex(Vector2Int index)
    {
        gridIndex = index;
    }

    public void SetGridIndex(int x,int y)
    {
        gridIndex = new Vector2Int(x,y);
    }

    [SerializeField] private uint missBulltetNumber = 0;//擦弹的数目
    public uint GetMissBulletNumber()
    {
        return missBulltetNumber;
    }
    public void SetMissBulletNumber(uint newMissNumber)
    {
        missBulltetNumber = newMissNumber;
    }
    [SerializeField] private float spellPrecent = 0.0f;//符卡充能百分比
    public void SetSpellPrecent(float precent)
    {
        spellPrecent = precent;
    }
    public float GetSpellPrecent()
    {
        return spellPrecent;
    }
    [SerializeField] private uint defendPoint = 0;//防御点数:每一点防御点数可以抵挡一次弹幕或近战的伤害

    //角色委托Action事件
    //方向变化Action
    public event Action directionChangeAction;//角色朝向发生变化时调用的委托
    public Action GetDirectionChangeAction()
    {
        return directionChangeAction;
    }
    
    //角色内部控制器,用于事件检测
    void Update()
    {
        CheckDirectionChange();
    }

    [NonSerialized] private BattleDirection lastDirection = BattleDirection.RIGHT;
    private void CheckDirectionChange()
    {
        if(direction != lastDirection)
        {
            directionChangeAction?.Invoke();//角色朝向发生变化激活委托
            lastDirection = direction;
        }
    }
}
