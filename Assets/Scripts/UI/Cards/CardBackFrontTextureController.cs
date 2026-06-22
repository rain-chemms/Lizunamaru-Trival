using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 卡牌显示器
// 必须挂载在卡牌的节点上
[RequireComponent(typeof(Card))]
public class CardBackFrontTextureController : MonoBehaviour
{
    [SerializeField] private Sprite defaultBackSprite;//默认的贴图,用于Bug显示
    [SerializeField] private Image back;
    public Image GetBackImage()
    {
        return back;
    }
    [SerializeField] private SerializableDictionary<CardCategory,Sprite> backSpriteDict;
    [SerializeField] private Image front;
    public Image GetFrontImage()
    {
        return front;
    }
    [SerializeField] private SerializableDictionary<CardCategory, Sprite> frontSpriteDict;
    [SerializeField] private Card card;
    private void Start()
    {
        //尝试自动获取
        //尝试自动获取
        if(card == null)
        {
            card = GetComponent<Card>();
        }
        //检查并改变卡牌的基础外观
        CheckAndChangeCardTexture();
    }

    // 检查并改变卡牌外观
    private void CheckAndChangeCardTexture()
    {
        if(back == null || front == null) return;
        //设置默认
        back.sprite = defaultBackSprite;
        front.sprite = defaultBackSprite;
        //获取backTexture
        CardCategory ctg = (CardCategory)card?.GetCardCategory();
        foreach (KeyValuePair<CardCategory,Sprite> item in backSpriteDict)
        {
            if(item.Value == null) continue;
            if (item.Key == ctg)
            {
                back.sprite = item.Value;
            }
        }
        //获取frontTexture
        foreach (KeyValuePair<CardCategory, Sprite> item in frontSpriteDict)
        {
            if(item.Value == null) continue;
            if (item.Key == ctg)
            {
                front.sprite = item.Value;
            }
        }
    }
}