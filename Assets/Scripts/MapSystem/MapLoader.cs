/*
    重头戏:
        地图生成器,控制并产生新的地图和加载文件中的地图存档
    调用时机:当前新的地图区域到达并开始时调用
*/
using UnityEngine;
using System.Collections.Generic;

namespace MapSystem
{
    //单例模式:使用地图加载器控制地图数据并加载关卡
    public class MapLoader : MonoBehaviour
    {
        public static MapLoader instance;
        void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        //暂时留着,等存档系统优化后在制作
        public void LoadMap()
        {
            return;
        }
        [SerializeField] private MapNode nodePrefab;//地图节点预制体模板
        public MapNode GetNodePrefab() => nodePrefab;
        //依据种子重新生成一张地图
        //需要一个通用性的函数:只要调用它就可以通过种子完全重现出一摸一样的地图信息,即生成Map的连接信息和节点信息
        public void RecurMap(
            int seed/*地图种子*/,
            Vector2Int size/*地图的关卡数和列数信息*/,
            MapAreaCategory area/*当前地图所处的区域*/,
            Vector2Int playerIndex/*玩家所处的地图索引,要选择的接下来的下一层的地图节点*/
        )
        {
            Map map = Map.instance;
            if(map == null) return;
            //清除旧的节点以及连接信息
            map.GetNodeList()?.Clear();
            map.GetLinkData()?.Clear();
            //设置基础数据
            map.SetMapArea(area);
            map.SetMapSize(size);
            map.SetPlayerPos(playerIndex);
            //获取地图节点列表和连接信息的引用
            List<MapNode> mapNodes = map.GetNodeList();
            Dictionary<Vector2Int, List<Vector2Int>> links = map.GetLinkData();//临接表
            /*
                随机生长一个地图
                地图不能存在路径交叉
            */




        }

    }
}