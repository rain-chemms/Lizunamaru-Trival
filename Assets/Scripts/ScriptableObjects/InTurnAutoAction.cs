using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//该脚本规定了AI操控的Role在一个回合到达时的行为
[CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "Scriptable Objects/InTurnAutoAction")]
public class InTurnAutoAction : ScriptableObject
{
    internal enum AttackCategory
    {
        RANDOM,//随机方向子弹
        SNIPE,//狙击弹
        DIRECTION,//方向子弹,朝着角色的面向的方向射击    
    }

    [Serializable]
    internal struct MoveAction
    {
        public BattleDirection direction;
        public int distance;
    }
    [Serializable]
    internal struct AttackAction
    {
        public Bullet bullet;
        public AttackCategory attackCategory;
    }

    [Header("是否开启移动行为")]
    [SerializeField] private bool moveCtgOpen = false;
    [SerializeField] private List<MoveAction> moveList = new List<MoveAction>();//Role当前回合得移动列表
    [SerializeField] private bool changeFly = false;//是否改变飞行状态
    
    [Header("是否开启攻击行为")]
    [SerializeField] private bool attackCtgOpen = false;
    [SerializeField] private List<AttackAction> bulletDict = new List<AttackAction>();//子弹及其攻击方式字典

    [Header("是否开启召唤行为")]
    [SerializeField] private bool summonCtgOpen = false;
    //[SerializeField] private List<Gadget> inTurnRenforce = new List<Gadget>();//召唤支援物体
    [Header("是否开启防御行为")]
    [SerializeField] private bool defendCtgOpen = false;
    [SerializeField] private int defendPoint = 0;//获得或失去的防御点数
    //专注于执行Role的行为,不对Role中的状态进行修改
    public IEnumerator ActionExcute(Role role)
    {
        if(role == null) yield break;
        //产生移动和改变飞行状态
        //不对玩家的能量系统产生变化
        if(moveCtgOpen)
        {
            if(changeFly) role.SetFly(!role.IsFly());
            foreach(MoveAction move in moveList)
            {
                Vector2Int target = Vector2Int.zero;
                switch(move.direction)
                {
                    case BattleDirection.LEFT:
                        target.x -= move.distance;
                        break;
                    case BattleDirection.RIGHT:
                        target.x += move.distance;
                        break;
                    case BattleDirection.DOWN:
                        target.y -= move.distance;
                        break;
                    case BattleDirection.UP:
                    default:
                        target.y += move.distance;
                        break;
                }
                //限制offset范围
                target += role.GetGridIndex();
                if(target.x < 0) target.x = 0;
                else if(target.x >= BattleBoard.instance?.GetWidthAndHeight().x) target.x = (int)BattleBoard.instance?.GetWidthAndHeight().x - 1;
                if(target.y < 0) target.y = 0;
                else if(target.y >= BattleBoard.instance?.GetWidthAndHeight().y) target.y = (int)BattleBoard.instance?.GetWidthAndHeight().y - 1;
                //移动
                role.SetGridIndex(target);
            }  
        }
        //攻击
        if(attackCtgOpen)
        {
            int cycleTime = 0;
            foreach(AttackAction attack in bulletDict)
            {
                Vector2Int targetIndex = role.GetGridIndex();
                switch(attack.attackCategory)
                {
                    case AttackCategory.RANDOM:
                        //依据种子获取随机数选择随机的格子
                        Vector2Int widAndhei = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();//获取棋盘的宽高  
                        int seed = (int)SeedSetter.instance?.GetSeed_Int() + cycleTime;//获取随机种子
                        //依据Random类生成int随机数作为棋盘的索引
                        System.Random rng = new System.Random(seed);
                        targetIndex = new Vector2Int(
                            rng.Next(widAndhei.x),
                            rng.Next(widAndhei.y)
                        );
                        break;
                    case AttackCategory.SNIPE:
                        Role tarRole = BattleMessage.instance?.GetNearestEnermy(role);
                        if(tarRole == null) yield break;//没找到目标敌人就不攻击
                        targetIndex = tarRole.GetGridIndex();//获取最近的敌人
                        break;
                    case AttackCategory.DIRECTION:
                    default:
                        targetIndex = role.GetGridIndex();
                        switch(role.GetDirection())
                        {
                            case BattleDirection.UP:
                                targetIndex.y += 1;
                                break;
                            case BattleDirection.DOWN:
                                targetIndex.y -= 1;
                                break;
                            case BattleDirection.LEFT:
                                targetIndex.x -= 1;
                                break;
                            case BattleDirection.RIGHT:
                                targetIndex.x += 1;
                                break;    
                        }    
                        break;
                }
                Bullet bt = attack.bullet;
                //产生具体的子弹
                yield return BattleMessage.instance?.GenerateBullet(
                    role,
                    bt,
                    targetIndex,
                    default,
                    true
                );
                cycleTime++;
            }
        }
        //如果存在召唤行为
        if(summonCtgOpen)
        {
            
        }

        //如果存在防御行为
        if(defendCtgOpen)
        {
            yield return role.GetComponent<RoleDefendGetter>()?.GetOrLoseDefend(defendPoint);
        }

    }
}
