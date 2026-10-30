/*
    重头戏:
        地图生成器,控制并产生新的地图和加载文件中的地图存档
    调用时机:当前新的地图区域到达并开始时调用
*/
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using MapSystem.MapNodeSystem;

namespace MapSystem
{
    //单例模式:使用地图加载器控制地图数据并加载关卡
    public class MapLoader : MonoBehaviour
    {
        public static MapLoader instance;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [SerializeField] private MapNode nodePrefab;//地图节点预制体模板
        public MapNode GetNodePrefab() => nodePrefab;
        [SerializeField] private const int maxJumpDis = 2;//规定两层节点之间最大的跨越距离,不可以变
        //依据相关信息重现一张地图
        //需要一个通用性的函数:只要调用它就可以通过种子完全重现出一摸一样的地图信息,即生成Map的连接信息和节点信息
        public void RecurMap(
            int seed/*地图种子*/,
            Vector2Int size/*地图的关卡数和列数信息*/,
            MapAreaCategory area/*当前地图所处的区域*/,
            Vector2Int playerIndex/*玩家所处的地图索引,要选择的接下来的下一层的地图节点*/
        )
        {
            Map map = Map.instance;
            if (map == null) return;
            //清除旧的节点以及连接信息
            foreach (MapNode node in map.GetNodeList())
            {
                Destroy(node.gameObject);
            }
            map.GetNodeList()?.Clear();
            map.GetLinkData()?.Clear();
            
            //设置基础数据
            map.SetMapArea(area);
            map.SetMapSize(size);
            map.SetPlayerPos(playerIndex);
            //获取地图节点列表和连接信息的引用
            List<MapNode> mapNodes = map.GetNodeList();
            SerializableDictionary<Vector2Int, List<Vector2Int>> links = map.GetLinkData();//临接表
            /*
                随机生长一个地图
                地图不能存在路径交叉
            */
            int effectLoop = 0;
            int ptrStrX = -1;
            //初始化随机数生成器
            System.Random random = new System.Random(seed + (int)area);//随机数要跟随地图区域进行变化
            /*
                注意点,看好了
                1.
                    当地图竖着放的时候
                    x代表的是关卡的一共有多少列
                    y代表的才是当前处于第几层
                2.
                    地图处于层数的一半的时候,一定会产生一个奖励
                    地图的-1层为每张地图的开始位置,要生成一个为CHAPTER_START的地图节点,而且是特制的
                    地图的size.y层(顶层上的一层)为每张地图的Boss战,也是特制的
            */
            while (effectLoop < size.x)//循环地图列数次
            {
                //每两次选择不同的节点,依据种子随机选取起始点,初始的节点的layer位置是0
                int str = random.Next(0, size.x);//生成[0,列数-1]范围的整数
                if (ptrStrX == str) continue;//本次循环无效
                else ptrStrX = str;
                bool haveNode = false;
                foreach (MapNode node in mapNodes.ToList())
                {
                    //确保数据有效
                    if (node == null)
                    {
                        mapNodes.Remove(node);
                        continue;
                    }
                    Vector2Int idx = node.GetIndex();
                    if (idx.x == str && idx.y == 0)
                    {
                        haveNode = true;
                        break;
                    }
                }

                //生成节点时可用的种类
                List<MapNodeCategory> availableCategory = Enum.GetValues(typeof(MapNodeCategory))
                        .Cast<MapNodeCategory>()
                        .Distinct()
                        .Where(x => x != MapNodeCategory.CHAPTER_START && x!= MapNodeCategory.BATTLE_BOSS)//排除起始关卡和Boss战
                        .ToList();
                int si = availableCategory.Count;
                    
                if (!haveNode)//创建一个新的节点
                {
                    MapNode node = Instantiate(nodePrefab);

                    //随机化节点的类型
                    //这里面的类型要排除初始关卡和Boss战
                    node.SetCategory(availableCategory[random.Next(si)]);//随机设置节点类型

                    node.SetIndex(new Vector2Int(str, 0));//设置节点的索引
                    node.GetComponent<MapNodeSpriteSetter>()?.FreshNodeOutSight();//尝试刷新外观
                    mapNodes.Add(node);
                }

                //每次生长一棵树
                int layer = 0;//初始层数为0
                while(layer < size.y - 1)//此时已经确定初始的节点了
                {
                    //随机选取下一层的一个点范围内的点
                    int min = 0;//最小限制,去左侧点中最大的节点
                    int max = size.x - 1;//最大限制,去左侧点中最小的节点
                    //确定最小范围和最大范围
                    foreach (KeyValuePair<Vector2Int, List<Vector2Int>> kv in links)
                    {
                        Vector2Int key = kv.Key;
                        List<Vector2Int> value = kv.Value;
                        //点必须是同层的
                        if (key.y == layer)
                        {
                            //查找所有大于str的点,获取max    
                            if (key.x > str)
                            {
                                //遍历所有连接的下层节点
                                foreach (Vector2Int data in value.ToList())
                                {
                                    //确保是下一层的节点且路径小于max值的
                                    if (data.y == (layer + 1) && data.x < max)
                                    {
                                        max = data.x;
                                    }
                                }
                            }
                            //查找所有小于str的点,获取min
                            if (key.x < str)
                            {
                                //遍历所有连接的下层节点
                                foreach (Vector2Int data in value.ToList())
                                {
                                    //确保是下一层的节点且路径小于max值的
                                    if (data.y == (layer + 1) && data.x > min)
                                    {
                                        min = data.x;
                                    }
                                }
                            }   
                        }
                    }
                    //在有效范围内随机选择下一层的一个节点
                    Vector2Int nextLayerIndex = new Vector2Int(
                        random.Next(min, max + 1),
                        layer + 1
                    );
                    //若不存在当前索引的节点则创建一个新的节点
                    bool haveNode_Inner = false;
                    foreach(MapNode node in mapNodes.ToList())
                    {
                        //确保数据有效
                        if(node == null) 
                        {
                            mapNodes.Remove(node);
                            continue;
                        }
                        Vector2Int idx = node.GetIndex();
                        if(idx.x == nextLayerIndex.x && idx.y == nextLayerIndex.y)
                        {
                            haveNode_Inner = true;
                            break;
                        }
                    }
                    if(!haveNode_Inner)//创建一个新的节点
                    {
                        MapNode node = Instantiate(nodePrefab);
                        //随机化节点的类型
                        node.SetCategory(availableCategory[random.Next(si)]);//随机设置节点类型
                        node.SetIndex(nextLayerIndex);//设置下一层节点的索引
                        //尝试刷新外观
                        node.GetComponent<MapNodeSpriteSetter>()?.FreshNodeOutSight();
                        mapNodes.Add(node);
                    }
                    
                    //存入连接数据
                    Vector2Int strIdx = new Vector2Int(str,layer);
                    if(links.TryGetValue(strIdx,out List<Vector2Int> lk))
                    {
                        if(lk == null) lk = new List<Vector2Int>();
                        if(!lk.Contains(nextLayerIndex)) lk.Add(nextLayerIndex);
                    }
                    else//不存在连接数据时,新添键值对
                    {
                        links.Add(strIdx,new List<Vector2Int>() { nextLayerIndex });
                    }
                    /*
                    //添加连接数据
                    foreach (KeyValuePair<Vector2Int, List<Vector2Int>> kv in links)
                    {
                        //获取节点
                        Vector2Int key = kv.Key;
                        List<Vector2Int> value = kv.Value;
                        //加入新的连接数据
                        if (key.x == str && key.y == layer)
                        {
                            value.Add(nextLayerIndex);
                        }
                    }
                    */
                    //切换str的位置到下一个节点处
                    str = nextLayerIndex.x;
                    layer = nextLayerIndex.y;
                }
                effectLoop++;
            }

            ///*
            //起始关卡和Boss战关卡的生成
            //1.起始关卡
            MapNode startNode = Instantiate(nodePrefab);
            startNode.SetCategory(MapNodeCategory.CHAPTER_START);
            Vector2Int startNodeIndex = new Vector2Int(size.x / 2, -1);
            startNode.SetIndex(startNodeIndex);//要让其位x向于中心
            startNode.GetComponent<MapNodeSpriteSetter>()?.FreshNodeOutSight();//尝试刷新外观
            mapNodes.Add(startNode);

            List<Vector2Int> appendToStart = new List<Vector2Int>();
            foreach(MapNode n in mapNodes?.ToList())//获取所有索引为0的节点的mapIndex
            {
                if(n == null) continue;
                Vector2Int idx = n.GetIndex();
                if(idx.y == 0)//这是第一关
                {
                    appendToStart.Add(idx);
                }
            }
            links.Add(startNodeIndex,appendToStart.ToList());//添加连接
            //*/

            ///*
            //2.关底Boss战
            MapNode endNode = Instantiate(nodePrefab);
            endNode.SetCategory(MapNodeCategory.BATTLE_BOSS);
            Vector2Int endNodeIndex = new Vector2Int(size.x / 2, size.y);
            endNode.SetIndex(endNodeIndex);//要让其位x向于中心
            endNode.GetComponent<MapNodeSpriteSetter>()?.FreshNodeOutSight();//尝试刷新外观
            mapNodes.Add(endNode);
            //获取所有处于Boss战之前的节点,将他们连接上Boss节点
            foreach(MapNode n in mapNodes?.ToList())
            {
                if(n == null) continue;
                Vector2Int idx = n.GetIndex();
                if(idx.y == (endNodeIndex.y - 1))//是最后一关
                {
                    links.Add(idx,new List<Vector2Int>{ endNodeIndex });
                } 
            }
            //*/

            //尝试刷新地图的显示
            map.GetComponent<MapNodeListPositionSetter>()?.FreshTheMapNodePosition();
            map.GetComponent<MapNodePathDisplayer>()?.FreshMapPath();//先刷新路径,让图标在路径之上
        
            //格式化输出调试
            Debug.Log("[MapLoader]: New Map Node Index List Number: " + mapNodes.Count);
            int number = 0;
            foreach (KeyValuePair<Vector2Int, List<Vector2Int>> kvp in links)
            {
                foreach (Vector2Int idx in kvp.Value)
                {
                    Debug.Log("[MapLoader]: Map Link: <" + kvp.Key.x + "," + kvp.Key.y + "> -> <" + idx.x + "," + idx.y + ">");
                    number++;
                }
            }
            Debug.Log("[MapLoader]: New Map Links Numbre: " + number);
        }

        //测试一下
        ///*
        void Start()
        {
            RecurMap(
                (int)SeedSetter.instance?.GetSeed_Int(),
                new Vector2Int(5, 15),
                MapAreaCategory.MONSTER_MOUNT,
                new Vector2Int(0, 0)
            );
        }
        //*/
    }
}