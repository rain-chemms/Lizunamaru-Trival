using UnityEngine;
using UnityEngine.UI;

namespace MapSystem{
    //地图节点图标设置器
    [RequireComponent(typeof(MapNode))]
    [RequireComponent(typeof(Button))]
    public class MapNodeSpriteSetter : MonoBehaviour
    {
        
        [SerializeField] private Button button;
        private MapNode node;
        void OnEnable()
        {
            if(node == null) node = GetComponent<MapNode>();
            if(button == null) button = GetComponent<Button>();
            FreshNodeOutSight();
        }
        //节点类型和节点图标映射器
        [SerializeField] private SerializableDictionary<MapNodeCategory,Sprite> spriteDict;
        public void FreshNodeOutSight()
        {
            if(node == null) return ;
            if(button == null) return ;
            if(spriteDict.TryGetValue(node.GetCategory(),out Sprite sprite))
            {
                if(sprite != null)
                {
                    button.image.sprite = sprite;
                }
            }    
        }
    }
}