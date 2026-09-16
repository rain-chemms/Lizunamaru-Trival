using UnityEngine;


namespace GridObjectSystem.AbilitySystem
{
    //用于获取能力图标的单例
    public class AbilitySpriteGetter : MonoBehaviour
    {
        public static AbilitySpriteGetter instance;
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

        [SerializeField] private Sprite debugSprite;//测试用的默认图标
        public Sprite GetDebugSprite() => debugSprite;

        [SerializeField] private SerializableDictionary<string,Sprite> abilitySpriteDict = new SerializableDictionary<string, Sprite>();
        public SerializableDictionary<string,Sprite> GetAbilitySpriteDict() => abilitySpriteDict;
        
        public void AddSprite(string abilityName,Sprite sprite)
        {
            if(!(bool)abilitySpriteDict?.ContainsKey(abilityName))
            {
                abilitySpriteDict.Add(abilityName,sprite);
            }
            else if(abilitySpriteDict[abilityName] != sprite)
            {
                abilitySpriteDict[abilityName] = sprite;
            }
        }
        
        public Sprite GetSprite(string abilityName) 
        {
            Sprite target = null;
            if((bool)abilitySpriteDict?.ContainsKey(abilityName))
            {
                target = abilitySpriteDict[abilityName];
            }
            return target;
        }
    }
}
