using UnityEngine;
using UnityEngine.UI;
using CardSystem;

[RequireComponent(typeof(CardCheckMask))]
public class CardCheckMaskImageSpriteSetter : MonoBehaviour
{
    [SerializeField] private CardCheckMask cardCheckMask;
    [SerializeField] private Image image;
    
    void OnEnable()
    {
        if(cardCheckMask == null) cardCheckMask = GetComponent<CardCheckMask>();
        if(image == null) image = GetComponent<Image>();
    }

    void Start()
    {
        SetTheSpriteToImage();
    }

    //依据卡牌的类型自动设置Image材质
    [SerializeField] private Sprite defaultSprite;
    public Sprite GetDefaultSprite() => defaultSprite;
    
    [SerializeField] private SerializableDictionary<CardCategory, Sprite> categorySpriteDict; 
    public SerializableDictionary<CardCategory, Sprite> GetCategoryMatDict() => categorySpriteDict;
    
    private void SetTheSpriteToImage()
    {
        if(image == null) return;
        Card card = cardCheckMask?.GetCard();
        if(card != null)
        {
            CardCategory category = card.GetCardCategory();
            if(categorySpriteDict.ContainsKey(category))
            {
                image.sprite = categorySpriteDict[category];
            }
            else image.sprite = defaultSprite;
        }
    }
}
