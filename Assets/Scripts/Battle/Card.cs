using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class Card : MonoBehaviour
{
    //卡牌类别
    [SerializeField] private CardCategory cardCategory;
    public CardCategory GetCardCategory()
    {
        return cardCategory;
    } 
    //卡牌关键字列表
    [SerializeField] private List<CardKeyWord> cardKeyWords = new List<CardKeyWord>();
    public List<CardKeyWord> GetCardKeyWords()
    {
        return cardKeyWords;
    }

    //在卡牌镶嵌入槽位后调用
    public virtual void AfterInsertToSolt()
    {}

    //在卡牌从槽位中取出后调用
    public virtual void AfterRemoveFromSolt()
    {}

    //public virtual void 
}