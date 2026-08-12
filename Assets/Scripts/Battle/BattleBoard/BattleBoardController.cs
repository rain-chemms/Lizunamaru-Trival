using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapSystem;

//战斗网格控制器:用于控制棋盘的数据
[RequireComponent(typeof(BattleBoard))]
public class BattleBoardController : MonoBehaviour
{
    
    [SerializeField] private BattleBoard battleBoard;
    public static BattleBoardController instance;
    //记录每种场景的不同种类棋盘格
    [SerializeField] private BattleGrid defaultGridPrefab;
    [SerializeField] private float gridInitYOffset = -1.0f;
    [SerializeField] private SerializableDictionary<MapAreaCategory,BattleGrid> gridPrefabDict = new SerializableDictionary<MapAreaCategory,BattleGrid>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void OnEnable()
    {
        {//尝试自动获取
            if(battleBoard == null) battleBoard = GetComponent<BattleBoard>();//尝试从脚本中获取
            if(battleBoard == null) battleBoard = BattleBoard.instance;//尝试从BattleBoard单例中获取
        }
        DestroyOutOfBoundaryGrids();
        DestroyRepeatBattleGrid();
        for(int i = 0; i < battleBoard?.GetWidthAndHeight().x; i++)
        {
            for(int j =0;j< battleBoard?.GetWidthAndHeight().y; j++)
            {
                TryFillTheEmptyGrid(new Vector2Int(i,j));
            }
        }
    }

    void Update()
    {
        FreshBattleBoardGridsPosition();
    }
    //清空棋盘格
    public void DestoryAllGrids()
    {
        if(battleBoard == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard is null, please check the Scene Nodes!");
            return;
        }
        if(battleBoard.GetBattleGridList() == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard's <BattleGridList> is null, please check the Scene Nodes!");
        }

        foreach(BattleGrid battleGrid in battleBoard.GetBattleGridList())
        {
            if(battleGrid == null) continue;//忽略空格
            BattleBoard.instance?.GetBattleGridList()?.Remove(battleGrid);//尝试移除的格子    
            Destroy(battleGrid.gameObject);
        }
    }

    //清空超出边界的格子
    public void DestroyOutOfBoundaryGrids()
    {
        if(battleBoard == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard is null, please check the Scene Nodes!");
            return;
        }
        if(battleBoard.GetBattleGridList() == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard's <BattleGridList> is null, please check the Scene Nodes!");
            return;
        }
        foreach(BattleGrid battleGrid in battleBoard.GetBattleGridList())
        {
            if(battleGrid == null) continue;//忽略空格
            bool isOutOfBoundary = false;
            if(battleGrid.GetIndex().x < 0 || battleGrid.GetIndex().x >= battleBoard.GetWidthAndHeight().x) isOutOfBoundary = true;
            if(battleGrid.GetIndex().y < 0 || battleGrid.GetIndex().y >= battleBoard.GetWidthAndHeight().y) isOutOfBoundary = true;
            if(isOutOfBoundary)
            {
                BattleBoard.instance?.GetBattleGridList()?.Remove(battleGrid);//尝试移除的格子
                Destroy(battleGrid.gameObject);
            }
        }
    }

    //清除棋盘格上的某个格子
    public void DestroyBattleGrid(Vector2Int tarIdx)
    {
        if(battleBoard == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard is null, please check the Scene Nodes!");
            return;
        }
        if(battleBoard.GetBattleGridList() == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard's <BattleGridList> is null, please check the Scene Nodes!");
            return;
        }
        foreach(BattleGrid battleGrid in battleBoard.GetBattleGridList())
        {
            if(battleGrid == null) continue;//忽略空格
            //清除所有目标索引的格子
            if(battleGrid.GetIndex().x == tarIdx.x && battleGrid.GetIndex().y == tarIdx.y)
            {
                BattleBoard.instance?.GetBattleGridList()?.Remove(battleGrid);//尝试移除的格子
                Destroy(battleGrid.gameObject);
            }
        }
    }

    //清除重复索引的格子
    public void DestroyRepeatBattleGrid()
    {
        if(battleBoard == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard is null, please check the Scene Nodes!");
            return;
        }
        if(battleBoard.GetBattleGridList() == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard's <BattleGridList> is null, please check the Scene Nodes!");
            return;
        }
        List<Vector2Int> repeatIndex = new List<Vector2Int>();
        foreach(BattleGrid battleGrid in battleBoard.GetBattleGridList().ToList())
        {
            if(battleGrid == null) continue;//忽略空格
            Vector2Int index = battleGrid.GetIndex();
            //检测索引
            bool isRepeat = false;
            foreach(Vector2Int idx in repeatIndex)
            {
                //检测到重复的地块
                if(idx.x == index.x && idx.y == index.y)
                {
                    BattleBoard.instance?.GetBattleGridList()?.Remove(battleGrid);//尝试移除重复的格子
                    Destroy(battleGrid.gameObject);//销毁格子
                    isRepeat = true;
                    break;
                }
            }
            if(!isRepeat) repeatIndex.Add(index);//非重复项添加索引
        }
    }
    
    //填上一个空缺的格子,创建对应的格子实体
    public void TryFillTheEmptyGrid(Vector2Int tarIdx)
    {
        //确保索引在棋盘内
        if(tarIdx.x < 0 || tarIdx.x >= battleBoard.GetWidthAndHeight().x) return;
        if(tarIdx.y < 0 || tarIdx.y >= battleBoard.GetWidthAndHeight().y) return;
        foreach(BattleGrid grid in battleBoard?.GetBattleGridList_Copy())
        {
            if(grid.GetIndex().x == tarIdx.x && grid.GetIndex().y == tarIdx.y) return;//检测到目标索引已经存在
        }
        MapAreaCategory area = Map.instance.GetMapArea();
        BattleGrid prefab = null;
        gridPrefabDict.TryGetValue(area, out prefab);
        if(prefab == null) prefab = defaultGridPrefab;
        BattleGrid newGrid = Instantiate(prefab, battleBoard?.transform);//以棋盘格为父物体
        //加入控制列表
        battleBoard?.GetBattleGridList()?.Add(newGrid);
        //设置初始位置
        newGrid.transform.localPosition = (Vector3)battleBoard?.GetGrid00LocalPosition() +
            new Vector3((float)battleBoard?.GetGapsOfGrid().x * tarIdx.x,
            gridInitYOffset,
            (float)battleBoard?.GetGapsOfGrid().y * tarIdx.y
        );
        //设置目标索引
        newGrid?.SetIndex(tarIdx);
    }    

    //刷新目前棋盘内格子的位置
    public void FreshBattleBoardGridsPosition()
    {
        if(battleBoard == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard is null, please check the Scene Nodes!");
            return;
        }
        if(battleBoard.GetBattleGridList() == null) 
        {
            Debug.LogError("[BattleBoardController]:The Control BattleBoard's <BattleGridList> is null, please check the Scene Nodes!");
            return;
        }
        foreach(BattleGrid battleGrid in battleBoard.GetBattleGridList())
        {
            if(battleGrid == null) continue;//忽略空格
            //设置所有棋盘格为棋盘的子物体
            battleGrid.transform.SetParent(battleBoard.transform);
            //设置相对坐标位置
            battleGrid.transform.localPosition = battleBoard.GetGrid00LocalPosition() +
                battleGrid.GetIndex().x * battleBoard.GetGapsOfGrid().x * Vector3.right +
                battleGrid.GetIndex().y * battleBoard.GetGapsOfGrid().y * Vector3.forward;
        }   
    }
}
