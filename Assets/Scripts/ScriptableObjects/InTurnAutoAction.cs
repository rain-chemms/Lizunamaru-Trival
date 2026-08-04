using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BulletSystem;
using GridObjectSystem.GadgetSystem;
using System.Linq;
using VfxDisplaySystem;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
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

        [Serializable]
        internal struct RenforceAction
        {
            public Vector2Int offset;
            public GridObject renforcePrefab;
        }

        [Header("是否开启移动行为")]
        [SerializeField] private bool moveCtgOpen = false;
        [SerializeField] private List<MoveAction> moveList = new List<MoveAction>();//Role当前回合得移动列表
        [SerializeField] private bool changeFly = false;//是否改变飞行状态

        [Header("是否开启攻击行为")]
        [SerializeField] private bool attackCtgOpen = false;
        [SerializeField] private List<AttackAction> bulletDict = new List<AttackAction>();//子弹及其攻击方式字典
        [Header("是否显示Vfx")]
        [SerializeField] private bool openVfx = false;
        [SerializeField] List<string> vfxList = new List<string>();//要显示的Vfx名称
        [Header("是否显示符卡界面")]
        [SerializeField] private bool openSpellDisplay = false;
        [SerializeField] private bool leftOrRightSD = false;//是在左侧还是右侧进行符卡的显示
        [SerializeField] private Sprite spellSprite;//符卡的人物立绘Sprite
        [SerializeField] private string spellTextKey = "Spell_Debug";//符卡的文本的本地化键值
        [SerializeField] private string searchTable = "SpellDisplayTexts";//搜索本地字典

        [Header("是否开启召唤行为")]
        [SerializeField] private bool summonCtgOpen = false;
        [SerializeField] private List<RenforceAction> inTurnRenforce = new List<RenforceAction>();//召唤支援物体行为
        [Header("是否开启防御行为")]
        [SerializeField] private bool defendCtgOpen = false;
        [SerializeField] private int defendPoint = 0;//获得或失去的防御点数
        //专注于执行Role的行为,不对Role中的状态进行修改
        public IEnumerator ActionExcute(Role role)
        {
            if (role == null) yield break;
            //优先检测并先显示符卡
            if (openVfx)
            {
                foreach (string vfxName in vfxList)
                {
                    yield return VfxDisplayer.instance?.DisplayVfx(vfxName,false,0.0f);
                }
            }
            if (openSpellDisplay)
            {
                yield return SpellAttackDisplayer.instance?.WakeDisplayer(spellSprite, leftOrRightSD, spellTextKey, searchTable);
            }
            //产生移动和改变飞行状态
            //不对玩家的能量系统产生变化
            if (moveCtgOpen)
            {
                if (changeFly) role.SetFly(!role.IsFly());
                foreach (MoveAction move in moveList)
                {
                    Vector2Int target = Vector2Int.zero;
                    switch (move.direction)
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
                    if (target.x < 0) target.x = 0;
                    else if (target.x >= BattleBoard.instance?.GetWidthAndHeight().x) target.x = (int)BattleBoard.instance?.GetWidthAndHeight().x - 1;
                    if (target.y < 0) target.y = 0;
                    else if (target.y >= BattleBoard.instance?.GetWidthAndHeight().y) target.y = (int)BattleBoard.instance?.GetWidthAndHeight().y - 1;
                    //移动
                    role.SetGridIndex(target);
                    role.SetDirection(move.direction);//设置移动方向
                }
            }
            //攻击
            if (attackCtgOpen)
            {
                int cycleTime = 0;
                foreach (AttackAction attack in bulletDict)
                {
                    Vector2Int targetIndex = role.GetGridIndex();
                    switch (attack.attackCategory)
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
                            if (tarRole == null) yield break;//没找到目标敌人就不攻击
                            targetIndex = tarRole.GetGridIndex();//获取最近的敌人
                            break;
                        case AttackCategory.DIRECTION:
                        default:
                            targetIndex = role.GetGridIndex();
                            switch (role.GetDirection())
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
            if (summonCtgOpen)
            {
                //循环产生召唤
                foreach(RenforceAction inTurnRenforce in inTurnRenforce.ToList())
                {
                    //获取召唤的预制体
                    GridObject prefab =  inTurnRenforce.renforcePrefab;
                    Vector2Int offset = inTurnRenforce.offset;
                    //在对应的格子召唤物体
                    //设置基础属性
                    GridObject newRenforce = Instantiate(prefab,BattleBoard.instance?.transform);
                    newRenforce?.SetSide((bool)role?.GetSide());//设置新的物体的阵营
                    newRenforce?.SetDirection((BattleDirection)role?.GetDirection());
                    if(newRenforce!=null) newRenforce.transform.position = (Vector3)role?.transform.position;//设置初始位置
                    
                    //如果是Gadget
                    Gadget gd = newRenforce as Gadget;
                    GadgetPositionToRoleSyncer syncer = gd?.GetComponent<GadgetPositionToRoleSyncer>();
                    syncer?.SetGapsToRole(offset);//开启全部同步
                    syncer?.SetPosSyncOpen(true);
                    syncer?.SetFlySyncOpen(true);
                    syncer?.SetDirSyncOpen(true);
                    gd?.SetBelongRole(role);
                    if(gd!=null && !(bool)BattleMessage.instance?.GetGadgetList()?.Contains(gd)) BattleMessage.instance?.GetGadgetList()?.Add(gd);
                    
                    //若为玩家类
                    Role newRole = newRenforce as Role;
                    newRole?.SetGridIndex((Vector2Int)role?.GetGridIndex() + offset);
                    newRole?.SetHp((float)newRole?.GetMaxHp());//设置新的角色的血量
                    newRole?.SetID((uint)BattleMessage.instance?.GetSideMaxRoleID((bool)newRole?.GetSide()) + 1);//设置新的角色的ID
                    newRole?.SetRoundOperateEnd(true);//设置新的角色已结束本回合的移动行为
                    if(newRole!=null && !(bool)BattleMessage.instance?.GetRoleList()?.Contains(newRole)) BattleMessage.instance?.GetRoleList()?.Add(newRole);//尝试添加新的角色
                }
            }

            //如果存在防御行为
            if (defendCtgOpen)
            {
                yield return role.GetComponent<RoleDefendGetter>()?.GetOrLoseDefend(defendPoint);
            }

        }
    }
}