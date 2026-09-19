using System.Collections;
using GridObjectSystem.GadgetSystem.Hakerros;
using UnityEngine;
using GridObjectSystem.RoleSystem;
using GridObjectSystem.GadgetSystem;

namespace CardSystem.AllCardHub
{
    //万宝槌:第一张攻击卡,相关角色:少名针妙丸
    //激活时,在目前控制角色的正前方产生预制体
    //打出时触发一次激活效果
    public class Attack_MiracleMallet : Card
    {
        [SerializeField] private MiracleMallet miracleMalletPrefab;
        public MiracleMallet GetMiracleMalletPrefab() => miracleMalletPrefab;
        
        public override IEnumerator AfterPlay()
        {
            yield return AfterTriggerEffective();
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterTriggerEffective()
        {
            CreateMallet();
            yield return base.AfterTriggerEffective();
        }

        private void CreateMallet()
        {
            Role role = BattleMessage.instance?.GetControlPlayer();
            if (role == null) return;
            Gadget mallet = Instantiate(miracleMalletPrefab);
            //将mallet加入棋盘
            mallet.transform.SetParent(BattleBoard.instance?.transform);
            //加入控制列表中
            if (!(bool)BattleMessage.instance?.GetGadgetList()?.Contains(mallet)) BattleMessage.instance?.GetGadgetList()?.Add(mallet);
            //设置UFO初始位置
            mallet.transform.position = (Vector3)role?.transform.position;
            //依据玩家当前的朝向设置锤子的索引
            Vector2Int append = Vector2Int.zero;
            switch(role?.GetDirection())
            {
                case BattleDirection.LEFT:
                    append = new Vector2Int(-1,0);
                    break;
                case BattleDirection.DOWN:
                    append = new Vector2Int(0,-1);
                    break;
                case BattleDirection.RIGHT:
                    append = new Vector2Int(1,0);
                    break;
                case BattleDirection.UP:
                default:
                    append = new Vector2Int(0,1);
                    break;
            }
            mallet?.SetGridIndex((Vector2Int)role?.GetGridIndex() + append);
            mallet?.SetDirection((BattleDirection)role?.GetDirection());
            //设置mallet的归属玩家
            mallet?.SetBelongRole(role);
            mallet.transform.rotation = (Quaternion)role?.transform.rotation;
            //设置飞行状态
            mallet?.SetFly((bool)role?.IsFly());
            //设置mallet的阵营
            mallet?.SetSide((bool)role?.GetSide());
        }
    }
}
