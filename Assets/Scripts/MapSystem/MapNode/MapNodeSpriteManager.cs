using UnityEngine;

namespace MapSystem.MapNodeSystem
{
    //地图节点Sprite管理器
    public class MapNodeSpriteManager : MonoBehaviour
    {
        public static MapNodeSpriteManager instance;
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

        //一般的节点的图片获取
        [Header("默认节点图像")]
        [SerializeField] private Sprite defaultSprite;
        public Sprite DefaultSprite { get => defaultSprite; set => defaultSprite = value; }
        [Header("一般的节点设置")]
        [SerializeField] private SerializableDictionary<MapNodeCategory,Sprite> spriteDict = new SerializableDictionary<MapNodeCategory, Sprite>();
        public SerializableDictionary<MapNodeCategory,Sprite> SpriteDict { get => spriteDict; }
        [Header("开始关卡图片资源字典:StartEventName => Sprite")]
        [SerializeField] private SerializableDictionary<string,Sprite> chapterStartDict = new SerializableDictionary<string, Sprite>();
        public SerializableDictionary<string,Sprite> ChapterStartDict { get => chapterStartDict; }

        [Header("Boss战关卡图片资源字典:BossName => Sprite")]
        [SerializeField] private SerializableDictionary<string,Sprite> bossBattleDict = new SerializableDictionary<string, Sprite>();
        public SerializableDictionary<string,Sprite> BossBattleDict { get => bossBattleDict; }

    }
}
