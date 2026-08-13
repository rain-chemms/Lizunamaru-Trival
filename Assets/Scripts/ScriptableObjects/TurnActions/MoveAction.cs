using UnityEngine;
using System.Collections;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "InTurnAutoAction/Actions/MoveAction")]
    public class MoveAction : TurnAction
    {
        [SerializeField] protected float beforeMoveInterval;
        [SerializeField] protected float afterMoveInterval;
        [SerializeField] protected BattleDirection direction;
        [SerializeField] protected int distance;
        [SerializeField] protected bool changeFly;//是否改变飞行状态
        public override IEnumerator Excute(Role role)
        {
            yield return base.Excute(role);
            Vector2Int target = Vector2Int.zero;
            switch (direction)
            {
                case BattleDirection.LEFT:
                    target.x -= distance;
                    break;
                case BattleDirection.RIGHT:
                    target.x += distance;
                    break;
                case BattleDirection.DOWN:
                    target.y -= distance;
                    break;
                case BattleDirection.UP:
                default:
                    target.y += distance;
                    break;
            }
            //限制offset范围
            target += role.GetGridIndex();
            if (target.x < 0) target.x = 0;
            else if (target.x >= BattleBoard.instance?.GetWidthAndHeight().x) target.x = (int)BattleBoard.instance?.GetWidthAndHeight().x - 1;
            if (target.y < 0) target.y = 0;
            else if (target.y >= BattleBoard.instance?.GetWidthAndHeight().y) target.y = (int)BattleBoard.instance?.GetWidthAndHeight().y - 1;
            //移动
            yield return new WaitForSeconds(beforeMoveInterval);
            if (changeFly) role.SetFly(!role.IsFly());
            role.SetGridIndex(target);
            role.SetDirection(direction);//设置移动方向
            yield return new WaitForSeconds(afterMoveInterval);
        }
    }
}