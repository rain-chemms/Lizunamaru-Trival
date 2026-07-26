using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MapSystem{
    //游戏地图:存储所有的节点和对应的路径
    //游戏中地图只能有一个,故使用单例模式
    [RequireComponent(typeof(Canvas))]//地图必须是Canvas
    public class Map : MonoBehaviour
    {
        public static Map instance;
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

        [SerializeField] private Vector2Int playerIndex;//玩家当前的位置
        public Vector2Int GetPlayerPos() => playerIndex;
        public void SetPlayerPos(Vector2Int pos) => playerIndex = pos;
        //节点列表
        [SerializeField] private List<MapNode> nodeList = new List<MapNode>();
        public List<MapNode> GetNodeList() => nodeList;
        public List<MapNode> GetNodeList_Copy() => nodeList?.ToList();
        //地图的链接数据
        [SerializeField] private Dictionary<Vector2Int,List<Vector2Int>> linkData = new Dictionary<Vector2Int, List<Vector2Int>>();
        public Dictionary<Vector2Int,List<Vector2Int>> GetLinkData() => linkData;
        //地图所处的区域
        [SerializeField] private MapAreaCategory mapArea;
        public MapAreaCategory GetMapArea() => mapArea;
        public void SetMapArea(MapAreaCategory area) => mapArea = area;
        [SerializeField] private Vector2Int mapSize;//规定地图的边界大小:x代表列数,y代表这个地图一共有几关
        public Vector2Int GetMapSize() => mapSize;
        public Vector2Int GetMapSize_Copy() => new Vector2Int(mapSize.x,mapSize.y);
        public void SetMapSize(Vector2Int size) => mapSize = size;
        public int GetMapLayerNumber() => mapSize.y;//地图的关卡数,共有几层
        public int GetMapMaxWayNumber() => mapSize.x;//地图最多的路线数
    }
}