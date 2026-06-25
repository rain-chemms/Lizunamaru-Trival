using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Card))]
public class CardInStackImageSetter : MonoBehaviour
{
    [SerializeField] private Image cardInStackImage;
    [SerializeField] SerializableDictionary<CardCategory,Sprite> cardInStackImageDict;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Card card;
    void Start()
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
    }
}
