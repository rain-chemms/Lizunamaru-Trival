using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//战斗棋盘:全局只能由一个,使用单例模式
//棋盘的朝向与Unity自身的坐标系统一致
public class BattleBoard : MonoBehaviour
{
    public static BattleBoard instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    
    [SerializeField] private List<BattleGrid> battleGridList = new List<BattleGrid>();
    public List<BattleGrid> GetBattleGridList()
    {
        return battleGridList;
    }
    
    [SerializeField] private Vector2 gapsOfGrid = Vector2.one;//格子之间的间隔
    public Vector2 GetGapsOfGrid()
    {
        return gapsOfGrid;
    }
    public void SetGapsOfGrid(Vector2 gapsOfGrid)
    {
        this.gapsOfGrid = gapsOfGrid;
    }
    [SerializeField] private Vector3 grid00LocalPosition = Vector3.zero;//格子00的相对于棋盘的位置
    public Vector3 GetGrid00LocalPosition()
    {
        return grid00LocalPosition;
    }
    public void SetGrid00LocalPosition(Vector3 grid00LocalPosition)
    {
        this.grid00LocalPosition = grid00LocalPosition;
    }
    //格子的索引默认是从0.0开始的,索引为负数的时候表示格子非正常需要清除
    [SerializeField] private Vector2Int widthAndHeight = Vector2Int.one;//棋盘的宽高
    public Vector2Int GetWidthAndHeight()
    {
        return widthAndHeight;
    }
    public void SetWidthAndHeight(Vector2Int widthAndHeight)
    {
        this.widthAndHeight = widthAndHeight;
    }
    void Start()
    {
        //默认将所有子节点的地图格子加入列表中
        battleGridList = GetComponentsInChildren<BattleGrid>().ToList();
    }
}
