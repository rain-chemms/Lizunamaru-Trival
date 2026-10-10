using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LevelLoadSystem;

namespace MapSystem.MapNodeSystem
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(MapNode))]
    public class MapNodeButtonFunctioner : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private MapNode node;
        void OnEnable()
        {
            if(node == null) node = GetComponent<MapNode>();
            if(button == null) button = GetComponent<Button>();
            button.onClick.AddListener(OnClickLoadLevel);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(OnClickLoadLevel);
        }

        private void OnClickLoadLevel()
        {
            /*
                优先检测当前是否可以加载地图
                index和当前的node之间不可达的时候直接返回
            */
            if(Map.instance.MapLocked) return;
            Vector2Int index = (Vector2Int)Map.instance?.GetPlayerPos();
            SerializableDictionary<Vector2Int,List<Vector2Int>> linkDict = Map.instance?.GetLinkData();
            if(linkDict.Contains(index))
            {
                List<Vector2Int> tg = linkDict[index];
                Vector2Int end = (Vector2Int)node?.GetIndex();
                if((bool)!tg?.Contains(end))
                {
                    Debug.LogWarning("[]: Link Start<"+ index.x.ToString() +","+ index.y.ToString() +"> => End<"+ end.x.ToString() +","+ end.y.ToString() + "> In Map ,Please Check your Player Index");
                    return ;
                }
            }
            else 
            {
                Debug.LogWarning("Don't have the StartNode<"+ index.x.ToString() +","+ index.y.ToString()+"> MapNode In Map ,Please Check your Player Index");
                return ;
            } 
            //等待当前地图关闭
            Map.instance.SetDisplay(false);
            StartCoroutine(LevelManager.instance?.LoadRandomLevel(index,(MapNodeCategory)node?.GetCategory()));
        }
    }
}
