using UnityEngine;


//角色移动器,专门控制Role的飞行,移动和方向的
[RequireComponent(typeof(Role))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Role role;
    void Start()
    {
        if(role == null) role = GetComponent<Role>();
    }
    //
    public void ChangeRoleDirection(BattleDirection direction)
    {
        role?.SetDirection(direction);
        role.GetComponent<RoleAnimTrigger>()?.TriggerAnim("Rotate");
    }
    //每次移动消耗玩家的Rice/IcePoint
    public void MoveRole(BattleDirection direction,int distance,uint costPoint)
    {
        if(role == null) return;
        if(BattleMessage.instance == null) return;
        if(BattleBoard.instance == null) return;
        bool turn = BattleMessage.instance.IsPlayerTurn();
        //计算相应的开销
        //玩家回合内使用RicePoint移动
        //敌人回合内使用IcePoint移动
        if(turn == role.GetSide())//是当前回合的玩家角色
        {
            if(BattleMessage.instance.GetRicePoint() < costPoint) return;
        }
        else//非玩家回合角色
        {
            if(BattleMessage.instance.GetIcePoint() < costPoint) return;
        }
        //计算目标点
        Vector2Int targetIndex = role.GetGridIndex();
        switch(direction)
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
        //规定了棋盘格最小索引为(0,0),最大为BattleMessage.instance.
        if( targetIndex.x < 0 ||
            targetIndex.y < 0 ||
            targetIndex.x > BattleBoard.instance.GetWidthAndHeight().x - 1 ||
            targetIndex.y > BattleBoard.instance.GetWidthAndHeight().y - 1
        ) return;//超出棋盘边界了
        //未超出边界时执行移动和point消耗
        role.SetGridIndex(targetIndex);
        if(turn == role.GetSide())
        {
            BattleMessage.instance.SetRicePoint(BattleMessage.instance.GetRicePoint() - costPoint);
        }
        else
        {
            BattleMessage.instance.SetIcePoint(BattleMessage.instance.GetIcePoint() - costPoint);
        }
        
        role.GetComponent<RoleAnimTrigger>()?.TriggerAnim("Walk");
    }

    public void SwitchFlyState(uint cost = 0)
    {
        if(role == null) return;
        if(BattleMessage.instance == null) return;
        if(BattleBoard.instance == null) return;
        bool turn = BattleMessage.instance.IsPlayerTurn();
        //计算相应的开销
        //玩家回合内使用RicePoint移动
        //敌人回合内使用IcePoint移动
        if(turn == role.GetSide())//是当前回合的玩家角色
        {
            if(BattleMessage.instance.GetRicePoint() < cost) return;
        }
        else//非玩家回合角色
        {
            if(BattleMessage.instance.GetIcePoint() < cost) return;
        }
        role?.SetFly(!role.IsFly());    
        if(turn == role.GetSide())
        {
            BattleMessage.instance.SetRicePoint(BattleMessage.instance.GetRicePoint() - cost);
        }
        else
        {
            BattleMessage.instance.SetIcePoint(BattleMessage.instance.GetIcePoint() - cost);
        }
        role.GetComponent<RoleAnimTrigger>()?.TriggerAnim("Fly");
    }
}