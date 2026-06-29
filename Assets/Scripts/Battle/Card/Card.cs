using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
//卡牌的属性和功能全在这个类及其继承中实现
public class Card : MonoBehaviour,CardFunctioner
{
    //卡牌类别
    [SerializeField] private CardCategory cardCategory;
    public void SetCardCategory(CardCategory ctg)
    {
        cardCategory = ctg;
    }
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
    //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public virtual IEnumerator AfterPlay()
    {
        yield return null;
    }
    public virtual IEnumerator AfterRemoveFromSolt()
    {
        yield return null;
    }
    public virtual IEnumerator AfterTriggerEffective()
    {
        yield return null;
    }
    
}