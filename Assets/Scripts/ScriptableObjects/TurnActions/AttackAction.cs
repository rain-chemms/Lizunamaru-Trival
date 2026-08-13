using UnityEngine;
using BulletSystem;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "InTurnAutoAction/Actions/AttackAction")]
    public class AttackAction : TurnAction
    {
        [Serializable]
        internal struct OnceShootData
        {
            public float beforeBulletInterval;//子弹发射之前的间隔时间
            public float afterBulletInterval;//子弹发射之后的间隔时间
            public Vector3 offset;//子弹发射时距离发射者的偏移
            public Vector2Int gridOffset;//角色目标格子的偏移,一般为0
            public Bullet bullet;
            public AttackCategory attackCategory;
        }
        [Header("子弹射击执行列表")]
        [SerializeField] private List<OnceShootData> shootList = new List<OnceShootData>();
        public override IEnumerator Excute(Role role)
        {
            yield return base.Excute(role);
            int cycleTime = 0;
            foreach (OnceShootData shoot in shootList)
            {
                Vector2Int targetIndex = role.GetGridIndex() + shoot.gridOffset;
                switch (shoot.attackCategory)
                {
                    case AttackCategory.DEFAULT:
                        break;
                    case AttackCategory.RANDOM:
                        //依据种子获取随机数选择随机的格子
                        Vector2Int widAndhei = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();//获取棋盘的宽高  
                        int seed = (int)SeedSetter.instance?.GetSeed_Int() + cycleTime + (int)BattleMessage.instance?.GetRound();//获取随机种子
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
                Bullet bt = shoot.bullet;
                //进行射击前的等待
                float beforeWait = shoot.beforeBulletInterval;
                float afterWait = shoot.afterBulletInterval;
                if(beforeWait > 0.0f) yield return new WaitForSeconds(beforeWait);
                //产生具体的子弹
                yield return BattleMessage.instance?.GenerateBullet(
                    role,
                    bt,
                    targetIndex,
                    shoot.offset,
                    true
                );
                //进行射击后的等待
                if(afterWait > 0.0f) yield return new WaitForSeconds(afterWait);
                cycleTime++;
            }
        }
    }
}