using UnityEngine;
using GlobalSystem;
using System.Linq;

namespace GridObjectSystem.RoleSystem.PlayerSystem
{
    //角色移动器,专门控制Role的飞行,移动和方向的
    //提供方法给RoleMoverController
    //当一个格子不存在的时候只能飞过去
    [RequireComponent(typeof(Role))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Role role;
        void Start()
        {
            if (role == null) role = GetComponent<Role>();
        }
        //
        public void ChangeRoleDirection(BattleDirection direction)
        {
            role?.SetDirection(direction);
            role.GetComponent<AnimTrigger>()?.TriggerAnim("Rotate");
        }
        //每次移动消耗玩家的Rice/IcePoint
        public void MoveRole(BattleDirection direction, int distance, uint costPoint)
        {
            if (role == null) return;
            if (BattleMessage.instance == null) return;
            if (BattleBoard.instance == null) return;
            bool turn = BattleMessage.instance.IsPlayerTurn();
            //计算相应的开销
            //玩家回合内使用RicePoint移动
            //敌人回合内使用IcePoint移动
            if (BattleMessage.instance.GetIcePoint() < costPoint) return;
            //计算目标点
            Vector2Int targetIndex = role.GetGridIndex();
            switch (direction)
            {
                case BattleDirection.LEFT:
                    targetIndex.x -= distance;
                    break;
                case BattleDirection.RIGHT:
                    targetIndex.x += distance;
                    break;
                case BattleDirection.DOWN:
                    targetIndex.y -= distance;
                    break;
                case BattleDirection.UP:
                default:
                    targetIndex.y += distance;
                    break;
            }

            Vector2Int boardSize = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();
            //规定了棋盘格最小索引为(0,0),最大为BattleMessage.instance.
            if (targetIndex.x < 0 ||
                targetIndex.y < 0 ||
                targetIndex.x > boardSize.x - 1 ||
                targetIndex.y > boardSize.y - 1
            ) return;//超出棋盘边界了
            
            //限制物体移动的范围为棋盘内
            //targetIndex.x = Mathf.Clamp(targetIndex.x,0,boardSize.x - 1);
            //targetIndex.y = Mathf.Clamp(targetIndex.y,0,boardSize.y - 1);

            //检测当前目标的格子是否存在
            BattleGrid targetGrid = BattleBoard.instance.GetBattleGridList_Copy().Where(x => x.GetIndex() == targetIndex).FirstOrDefault();
            if(targetGrid == null && !(bool)role?.IsFly()) return ;//目标格子不存在或是存在障碍的时候只能飞过去
            
            //未超出边界时执行移动和point消耗
            //费用使用修改:移动只能消耗IcePoint
            role.SetGridIndex(targetIndex);
            BattleMessage.instance.SetIcePoint(BattleMessage.instance.GetIcePoint() - costPoint);
            role.GetComponent<AnimTrigger>()?.TriggerAnim("Walk");
        }

        public void SwitchFlyState(uint cost = 0)
        {
            if (role == null) return;
            if (BattleMessage.instance == null) return;
            if (BattleBoard.instance == null) return;
            bool turn = BattleMessage.instance.IsPlayerTurn();
            //计算相应的开销
            //使用IcePoint进行飞行切换:但是飞行的切换消耗初始值是0
            if (BattleMessage.instance.GetIcePoint() < cost) return;
            //若当前格子不存在则不能切换飞行状态
            Vector2Int index = role.GetGridIndex();
            BattleGrid targetGrid = BattleBoard.instance.GetBattleGridList_Copy().Where(x => x.GetIndex() == index).FirstOrDefault();
            if(targetGrid == null) return;//当前位置不能切换飞行状态

            role?.SetFly(!role.IsFly());
            BattleMessage.instance.SetIcePoint(BattleMessage.instance.GetIcePoint() - cost);
            role.GetComponent<AnimTrigger>()?.TriggerAnim("Fly");
        }

    }
}