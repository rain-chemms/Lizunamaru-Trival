using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MapSystem
{
    [RequireComponent(typeof(Map))]
    [RequireComponent(typeof(MapNodeListPositionSetter))]
    public class MapNodePathDisplayer : MonoBehaviour
    {
        [SerializeField] private Map map; 
        [SerializeField] private MapNodeListPositionSetter mapNPS;
        void OnEnable()
        {
            if(map == null) map = GetComponent<Map>();
            if(mapNPS == null) mapNPS = GetComponent<MapNodeListPositionSetter>();
            FreshMapPath();
        }
        [SerializeField] private Image pathPrefab;
        [SerializeField] private List<Image> pathList = new List<Image>();//用于存储路径物体的引用
        public List<Image> GetPathList() => pathList;
        [SerializeField] private ScrollRect scrollRect;//用于存放地图节点的滑动区域
        public ScrollRect GetScrollRect() => scrollRect;
        public void FreshMapPath()
        {
            //确保条件正确
            if(scrollRect == null) return;
            if(map == null) return;          
            if(pathList == null) return;  
            if(map.GetLinkData() == null) return;
            //Debug.Log("[MapNodePathDisplayer]: Map Links Number: <"+map?.GetLinkData()?.Count+">");
            //清除旧的连接信息
            foreach(Image path in pathList)
            {
                Destroy(path?.gameObject);
            }
            pathList.Clear();

            foreach(KeyValuePair<Vector2Int,List<Vector2Int>> kv in map.GetLinkData())
            {
                Vector2Int index = kv.Key;
                List<Vector2Int> paths = kv.Value;
                //获取起点位置
                MapNode startNode = null;
                foreach(MapNode node in map.GetNodeList())
                {
                    if(node.GetIndex().x == index.x && node.GetIndex().y == index.y)
                    {
                        startNode = node;
                        break;
                    }
                }
                if(startNode == null) continue;//节点不存在则跳过
                foreach(Vector2Int way in paths)//一次获取路径终点位置
                {
                    MapNode endNode = null;
                    foreach(MapNode node in map.GetNodeList())
                    {
                        if(node.GetIndex().x == way.x && node.GetIndex().y == way.y)
                        {
                            endNode = node;
                            break;
                        }
                    }   
                    if(endNode == null) continue;//找不到节点则跳过
                    //所有条件都符合
                    //创建一条新的路径
                    Image pa = Instantiate(pathPrefab);
                    pa.transform.SetParent(scrollRect.content.transform);//设置父节点为Map的ScrollRect
                    //设置路径的位置旋转
                    //Vector3 centerPos = ((Vector3)startNode.GetComponent<RectTransform>()?.localPosition + (Vector3)endNode.GetComponent<RectTransform>()?.localPosition) * 0.5f;//中心点位置
                    //获取两个节点之差的方向
                    Vector2 dir = (endNode.transform.position - startNode.transform.position).normalized;
                    float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Quaternion rotate = Quaternion.Euler(0f, 0f, angleZ);
                    /*
                    Quaternion.LookRotation(
                       dir,
                       Vector3.up
                    );//旋转的大小为起始点望向终点
                    */
                    //依据旋转的大小计和地图节点设置器的信息计算算实际width
                    Vector2 _gaps = mapNPS.GetGaps();//获取间隔
                    bool vOrH = mapNPS.VertialOrHorizontal();//获取地图的走向
                    bool xIncrease = mapNPS.IsIncreaseX();//获取X轴的走向
                    bool yIncrease = mapNPS.IsIncreaseY();//获取Y轴的走向
                    //计算间隔
                    float xDlt = (vOrH ? _gaps.x : _gaps.y) * (way.x - index.x) * (xIncrease ? 1.0f : -1.0f);
                    float yDlt = (vOrH ? _gaps.y : _gaps.x) * (way.y - index.y) * (yIncrease ? 1.0f : -1.0f);
                    float width = 
                        Mathf.Sqrt(Mathf.Pow(xDlt, 2) + Mathf.Pow(yDlt, 2))
                    ;//物体的长度
                    //设置Image预制体的信息
                    RectTransform rtf = pa.GetComponent<RectTransform>();
                    if(rtf!=null)
                    {
                        //rtf.pivot = new Vector2(0.0f, 0.0f);
                        rtf.sizeDelta = new Vector2(width,rtf.sizeDelta.y);//设置大小
                        rtf.localRotation = rotate;
                    }
                    rtf.localPosition = startNode.transform.localPosition;//设置位置
                    pathList.Add(pa);//添加到路径列表
                }
            }
        }
    }
}