using UnityEngine;
using UnityEngine.UI;
using CardSystem;

[RequireComponent(typeof(Card))]
public class CardInStackImageSetter : MonoBehaviour
{
    [SerializeField] private Image cardInStackImage;
    public Image GetCardInStackImage() => cardInStackImage;

    [SerializeField] private Sprite defaultSprite;
    public Sprite GetDefaultSprite() => defaultSprite;

    [SerializeField] SerializableDictionary<CardCategory,Sprite> cardInStackImageDict;
    public SerializableDictionary<CardCategory,Sprite> GetCardInStackImageDict() => cardInStackImageDict;

    [SerializeField] private Card card;
    public Card GetCard() => card;

    void OnEnable()
    {
        //尝试自动获取
        if(card == null) card = GetComponent<Card>();
        if(cardInStackImage == null)
        {
            Image[] images = this.GetComponentsInChildren<Image>();
            foreach(Image image in images)
            {
                if(image == null) continue;
                if(image.name == "InStack")
                {
                    cardInStackImage = image;
                    break;
                }
            }
        }
        //依据卡片自身类型设置Spite  
        SetImageByCardCategory(card.GetCardCategory());
    }

    private void SetImageByCardCategory(CardCategory category)
    {
        if(cardInStackImage == null) return;
        if(cardInStackImageDict == null) return;
        if(cardInStackImageDict.ContainsKey(category))
        {
            cardInStackImage.sprite = cardInStackImageDict[category];
        }
        else cardInStackImage.sprite = defaultSprite;
    }
}
