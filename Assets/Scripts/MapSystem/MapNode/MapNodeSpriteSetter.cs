using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.MapNodeSystem
{
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
        [SerializeField] private Vector3 normalScale = new Vector3(1.0f,1.0f,1.0f); 
        [SerializeField] private float specialNodeScale = 3.5f;
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
            //同步大小
            node.transform.localScale = normalScale;
            MapNodeCategory ctg = node.GetCategory();
            //若节点为初始节点或者Boss节点.将其放大
            if(ctg == MapNodeCategory.CHAPTER_START || ctg == MapNodeCategory.BATTLE_BOSS)
            {
                node.transform.localScale *= specialNodeScale;   
            }
        }
    }
}