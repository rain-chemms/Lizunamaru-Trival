using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Card))]
[RequireComponent(typeof(CardDisplayer))]
public class CardInStackChecker : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private CardDisplayer cardDisplayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(card == null) card = GetComponent<Card>();
        if(cardDisplayer == null) cardDisplayer = GetComponent<CardDisplayer>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCardInStack();
    }

    private void CheckCardInStack()
    {
        List<Card> drawStack = BattleMessage.instance?.GetDrawCardList();
        List<Card> discardStack = BattleMessage.instance?.GetDiscardCardList();
        if((bool)drawStack?.Contains(card) || (bool)discardStack?.Contains(card))
        {
            if(!cardDisplayer.IsInStack()) cardDisplayer?.SetInStack(true);
        }
        else
        {
            if(cardDisplayer.IsInStack()) cardDisplayer?.SetInStack(false);
        }
    }
}
