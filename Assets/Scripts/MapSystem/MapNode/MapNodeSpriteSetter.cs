using System.Runtime.InteropServices;
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
        [SerializeField] private MapNode node;
        void OnEnable()
        {
            if(node == null) node = GetComponent<MapNode>();
            if(button == null) button = GetComponent<Button>();
            FreshNodeOutSight();
        }
        public void FreshNodeOutSight()
        {
            //默认设置
            SerializableDictionary<MapNodeCategory,Sprite> spriteDict = MapNodeSpriteManager.instance.SpriteDict;
            if(node == null) return ;
            if(button == null) return ;
            MapNodeCategory ctg = node.GetCategory();
            if(spriteDict.TryGetValue(ctg,out Sprite sprite))
            {
                if(sprite != null)
                {
                    button.image.sprite = sprite;
                }
            }
            else button.image.sprite = MapNodeSpriteManager.instance.DefaultSprite;
            //对特殊的节点值进行处理
            if(ctg == MapNodeCategory.CHAPTER_START)
            {
                var bossDict = MapNodeSpriteManager.instance.BossBattleDict;
                
            }
            else if(ctg == MapNodeCategory.BATTLE_BOSS)
            {
                var startDict = MapNodeSpriteManager.instance.ChapterStartDict;

            }

        }
    }
}