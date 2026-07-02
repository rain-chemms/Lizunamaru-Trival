using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
//卡牌的属性和功能全在这个类及其继承中实现
public class Card : MonoBehaviour, CardFunctioner
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
    public void AddCardKeyWord(CardKeyWord kw)
    {
        if (cardKeyWords == null) return;
        if (!cardKeyWords.Contains(kw)) cardKeyWords.Add(kw);
    }

    //卡牌接口的空实现
    public virtual IEnumerator AfterInsertToSolt()
    {
        yield return null;
    }
    public virtual IEnumerator AfterPlay()
    {
        //当有消耗词条是将触发卡牌的
        if ((bool)cardKeyWords?.Contains(CardKeyWord.EXHAUST))
        {
            yield return BattleMessage.instance?.ExhaustCard(this);//消耗这张卡
        }
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
    public virtual IEnumerator AfterRoundEnd()
    {
        yield return null;
    }
    //回合开始时触发
    public virtual IEnumerator AfterRoundStart()
    {
        yield return null;
    }

    //在你的回合丢弃时触发
    public virtual IEnumerator AfterDsicard()
    {
        yield return null;
    }

    //在抽到卡牌时触发
    public virtual IEnumerator AfterDraw()
    {
        yield return null;
    }

    public virtual IEnumerator AfterExhaust()
    {
        yield return null;
    }
}