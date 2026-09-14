using UnityEngine;
using GridObjectSystem.GadgetSystem;
using System.Collections.Generic;
using System.Collections;
using GridObjectSystem.GadgetSystem.Hakerros;
using GridObjectSystem.RoleSystem;
using System.Linq;
using GlobalSystem;
using GridObjectSystem.GadgetSystem.KeyStones;

namespace CardSystem.AllCardHub
{
    //打出时,在你的上方召唤一块可以抵挡{0}次子弹的要石
    //嵌入卡槽时,所有要石的耐久+{1}并且恢复所有要石耐久,同时消耗{2}点能量,优先减少RicePoint
    //该牌应带有虚无属性
    public class Gadget_KeyStoneOfEndurance : Card
    {
        [SerializeField] private int defendTimes = 10;
        public void SetDefendTimes(int times) => defendTimes = times;
        public int GetDefendTime() => defendTimes;
        [SerializeField] private KeyStone keyStonePrefab;
        public KeyStone GetKeyStonePrefab() => keyStonePrefab;

        [SerializeField] private int recoverEndurace = 5;
        public void SetRecoverEndurace(int recover) => recoverEndurace = recover;  
        public int GetRecoverEndurace() => recoverEndurace;

        [SerializeField] private uint insertCost = 1;
        public void SetInsertCost(uint cost) => insertCost = cost;
        public uint GetInsertCost() => insertCost;

        public override IEnumerator AfterPlay()
        {
            //获取当前控制的角色
            Role role = BattleMessage.instance?.GetControlPlayer();
            if (role == null) yield break;
            KeyStone stone = Instantiate(keyStonePrefab);
            stone.SetDefendTimes(defendTimes);
            stone.SetDefendTimesCounter(0);
            //将要石加入棋盘
            stone.transform.SetParent(BattleBoard.instance?.transform);
            //加入控制列表中
            if (!(bool)BattleMessage.instance?.GetGadgetList()?.Contains(stone)) BattleMessage.instance?.GetGadgetList()?.Add(stone);
            //设置要石初始位置
            stone.transform.position = (Vector3)role?.transform.position;
            //设置要石的归属玩家
            stone.SetBelongRole(role);
            //设置要石阵营
            stone.SetSide((bool)role?.GetSide());
            //依据玩家的方向设置要石的offset
            Vector2Int offset = Vector2Int.zero;
            switch((BattleDirection)role?.GetDirection())
            {
                case BattleDirection.RIGHT:
                    offset = new Vector2Int(-1,0);
                    break;
                case BattleDirection.LEFT:
                    offset = new Vector2Int(1,0);
                    break;
                case BattleDirection.DOWN:
                    offset = new Vector2Int(0,1); 
                    break;
                case BattleDirection.UP:
                default:
                    offset = new Vector2Int(0,-1);
                    break;
            }
            GadgetPositionToRoleSyncer bSyncer = stone.GetComponent<GadgetPositionToRoleSyncer>();
            bSyncer?.SetGapsToRole(offset);
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterInsertToSolt()
        {
            yield return base.AfterInsertToSolt();
            //检测能量是否充足,不足则返回手牌
            BattleMessage bm = BattleMessage.instance;
            uint ricePoint = (uint)bm?.GetRicePoint();
            uint icePoint = (uint)bm?.GetIcePoint();
            uint energy = ricePoint + icePoint;
            if(energy < insertCost) 
            {
                yield return bm?.AddExistCardToHand(this);
                yield break;
            };
            
            //计算能量消耗
            int stillNeed = (int)ricePoint - (int)insertCost;
            int iceRemain = (int)icePoint;
            if(stillNeed < 0)//没减完
            {
                iceRemain += stillNeed;//stillNeed是负数直接加

            }
            bm?.SetRicePoint(stillNeed < 0 ? 0 : (uint)stillNeed);
            bm?.SetIcePoint((uint)iceRemain);

            List<KeyStone> keyStones = FindObjectsByType<KeyStone>(FindObjectsSortMode.None).ToList();
            foreach(KeyStone stone in keyStones)
            {
                if(stone != null)
                {
                    stone.SetDefendTimes(stone.GetDefendTimes() + recoverEndurace);
                    stone.SetDefendTimesCounter(0);
                }   
            }
        }
    }
}