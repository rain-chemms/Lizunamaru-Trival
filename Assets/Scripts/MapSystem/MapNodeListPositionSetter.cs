using System;
using UnityEngine;
using UnityEngine.UI;

namespace MapSystem
{
    [RequireComponent(typeof(Map))]
    public class MapNodeListPositionSetter : MonoBehaviour
    {
        [SerializeField] private Map map;
        void OnEnable()
        {
            if(map == null) map = GetComponent<Map>();
            FreshTheMapNodePosition();
        }
        [Serializable]
        internal struct MapDisplayControlData//地图显示的控制数据
        {
            public bool verticalOrHorizontal;//地图是纵向还是横向显示的,false横向,true纵向
            public bool dlOrIc_x;//X坐标是随着索引的增加而增大还是随之减小
            public bool dlOrIc_y;//X坐标是随着索引的增加而增大还是随之减小
        }
        
        [SerializeField] private ScrollRect scrollRect;//用于存放地图节点的滑动区域
        public ScrollRect GetScrollRect() => scrollRect;
        //00坐标索引的位置
        [SerializeField] private Vector2 _00Position = new Vector2(0.0f,0.0f);//00网格位置处的2维度坐标
        public Vector2 Get00Position() => _00Position;
        public void Set00Position(Vector2 pos) => _00Position = pos;
        [SerializeField] private Vector2 _gaps = new Vector2(0.0f,0.0f);//gaps代表网格之间的间隔
        public Vector2 GetGaps() => _gaps;
        public void SetGaps(Vector2 gaps) => _gaps = gaps;
        [SerializeField] private MapDisplayControlData controlSettings;
        public bool VertialOrHorizontal() => controlSettings.verticalOrHorizontal;
        public bool IsIncreaseX() => controlSettings.dlOrIc_x ? false : true;
        public bool IsIncreaseY() => controlSettings.dlOrIc_y ? false : true;
        public void FreshTheMapNodePosition()
        {
            if(map == null) return;
            if(scrollRect == null) return;
            foreach(MapNode node in map.GetNodeList())
            {
                if(node == null) return;
                //将节点的父物体设置为scrollRect的content
                node.transform.SetParent(scrollRect.content);
                Vector2Int index = node.GetIndex();
                //计算偏移量
                Vector2 offset = new Vector2(
                    (controlSettings.dlOrIc_x ? -1.0f : 1.0f) * (controlSettings.verticalOrHorizontal ? index.x : index.y) * _gaps.x,
                    (controlSettings.dlOrIc_y ? -1.0f : 1.0f) * (controlSettings.verticalOrHorizontal ? index.y: index.x) * _gaps.y
                );
                //尝试设置RectTransform
                node.GetComponent<RectTransform>().localPosition = new Vector3(
                    _00Position.x + offset.x,
                    _00Position.y + offset.y,
                    0
                );//z轴不设置
            }
        }
    }
}