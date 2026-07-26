using UnityEngine;
using System.Collections;

namespace MapSystem
{
    //代表当前的地图节点
    //每个地图节点都对应一个按钮
    //只负责存储地图的节点信息
    public class MapNode : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        [SerializeField] private Vector2Int index;//节点的索引
        public Vector2Int GetIndex() => index;
        public void SetIndex(Vector2Int index) => this.index = index;
        [SerializeField] private MapNodeCategory category;//当前节点的类别
        public MapNodeCategory GetCategory() => category;
        public void SetCategory(MapNodeCategory category) => this.category = category;
    }

}
