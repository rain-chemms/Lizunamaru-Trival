using UnityEngine;
using UnityEngine.UI;

namespace MapSystem.MapNodeSystem
{
    [RequireComponent(typeof(MapNode))]
    [RequireComponent(typeof(Button))]
    public class MapNodeScaleSetter : MonoBehaviour
    {
        [SerializeField] private Button button;        
        [SerializeField] private MapNode node;
        void OnEnable()
        {
            if(node == null) node = GetComponent<MapNode>();
            if(button == null) button = GetComponent<Button>();
        }
        [SerializeField] private Vector3 normalScale = new Vector3(1.0f,1.0f,1.0f); 
        [SerializeField] private float specialNodeScale = 3.5f;

        void Update()
        {
            SyncNodeScale();
        }

        private void SyncNodeScale()
        {
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