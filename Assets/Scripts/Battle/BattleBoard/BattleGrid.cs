using UnityEngine;


//战斗网格控制器:用于生成和存储战斗网格的序列
//战斗网格:用于显示战斗场景的单个棋盘格
/*
    从Y轴正方向向下看的俯视图
    Vector3.forward
    Z
    ^   [2,0][2,1]......
    |   [1,0][1,1]......
    |   [0,0][0,1]......
    |
    ----------->X Vector3.right
*/

public class BattleGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int index = Vector2Int.zero;//棋盘格索引
    //获取棋盘格索引
    public Vector2Int GetIndex()
    {
        return index;
    }     
    //设置棋盘格索引
    public void SetIndex(Vector2Int index)
    {
        this.index = index;
    }
    void OnDestroy()
    {
        //在销毁时，将grid从棋盘列表中移除
        BattleBoard.instance?.GetBattleGridList()?.Remove(this);
    }
}