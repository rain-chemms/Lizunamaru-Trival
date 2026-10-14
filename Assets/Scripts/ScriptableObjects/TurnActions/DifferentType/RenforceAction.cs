using System.Collections;
using UnityEngine;
using System;
using GridObjectSystem.GadgetSystem;
using System.Collections.Generic;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "InTurnAutoAction/Actions/RenforceAction")]
    public class RenforceAction : TurnAction
    {
        [Serializable]
        internal struct OnceRenforce
        {
            public Vector2Int offset;
            public GridObject renforcePrefab;
        }
        [SerializeField] private List<OnceRenforce> renforceList = new List<OnceRenforce>();//一次召唤的援军列表
        public override IEnumerator Excute(Role role)
        {
            yield return base.Excute(role);
            //获取召唤的预制体
            foreach (OnceRenforce renforce in renforceList)
            {
                GridObject prefab = renforce.renforcePrefab;
                if(prefab == null) continue;//忽略空预制体对应的援军
                //在对应的格子召唤物体
                //设置基础属性
                GridObject newRenforce = Instantiate(prefab, BattleBoard.instance?.transform);
                newRenforce?.SetSide((bool)role?.GetSide());//设置新的物体的阵营
                newRenforce?.SetDirection((BattleDirection)role?.GetDirection());
                if (newRenforce != null) newRenforce.transform.position = (Vector3)role?.transform.position;//设置初始位置
                //如果是Gadget
                Gadget gd = newRenforce as Gadget;
                GadgetPositionToRoleSyncer syncer = gd?.GetComponent<GadgetPositionToRoleSyncer>();
                syncer?.SetGapsToRole(renforce.offset);//开启全部同步
                syncer?.SetPosSyncOpen(true);
                syncer?.SetFlySyncOpen(true);
                syncer?.SetDirSyncOpen(true);
                gd?.SetBelongRole(role);
                if (gd != null && !(bool)BattleMessage.instance?.GetGadgetList()?.Contains(gd)) BattleMessage.instance?.GetGadgetList()?.Add(gd);
                //若为玩家类
                Role newRole = newRenforce as Role;
                newRole?.SetGridIndex((Vector2Int)role?.GetGridIndex() + renforce.offset);
                newRole?.SetHp((float)newRole?.GetMaxHp());//设置新的角色的血量
                newRole?.SetID((uint)BattleMessage.instance?.GetSideMaxRoleID((bool)newRole?.GetSide()) + 1);//设置新的角色的ID
                newRole?.SetRoundOperateEnd(true);//设置新的角色已结束本回合的移动行为
                if (newRole != null && !(bool)BattleMessage.instance?.GetRoleList()?.Contains(newRole)) BattleMessage.instance?.GetRoleList()?.Add(newRole);//尝试添加新的角色
                yield return null;
            }
        }
    }
}