using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(StackCardAnchorSetter))]
public class StackCardSortOrderSetter : MonoBehaviour
{
    [SerializeField] private bool drawOrDiscard = false;//控制的牌堆,true:抽牌堆,false:弃牌堆
    [SerializeField] private int sortOffset = 10;
    public void SetSortOffset(int offset)
    {
        sortOffset = offset;
    }
    public int GetSortOffset()
    {
        return sortOffset;
    }
    void Update()
    {
        OrderTheCardSort();
    }
    private void OrderTheCardSort()
    {
        List<Card> cardList = null;
        if(drawOrDiscard) cardList = BattleMessage.instance?.GetDiscardCardList(); 
        else cardList = BattleMessage.instance?.GetDrawCardList();
        if(cardList == null) return;
        int index = 0;
        foreach(Card card in cardList)
        {
            if(card == null) continue;
            if(card.GetComponent<Canvas>()!=null) card.GetComponent<Canvas>().sortingOrder = index + sortOffset;
            index++;
        }
    }
}
