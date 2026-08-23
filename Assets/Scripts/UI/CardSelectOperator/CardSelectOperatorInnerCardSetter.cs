using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using CardSystem;

//内部卡牌位置设置器
[RequireComponent(typeof(CardSelectOperator))]
public class CardSelectOperatorInnerCardSetter : MonoBehaviour
{
    [SerializeField] private CardSelectOperator cardSelectOperator;
    void OnEnable()
    {
        if(cardSelectOperator == null) cardSelectOperator = GetComponent<CardSelectOperator>();
    }

    void Update()
    {
        CheckAndSetInnerCard();
    }
    
    [SerializeField] public ScrollRect cardContainer;
    public ScrollRect GetCardContainer() => cardContainer;
    
    private void CheckAndSetInnerCard()
    {
        if(cardSelectOperator == null) return;
        if(cardContainer == null) return;
        //将内部的卡牌的父物体设置为ScrollRect的content
        foreach((Card card,Transform parent) in cardSelectOperator?.GetWillSelectCardList_Copy())
        {
            if(card == null) continue;
            card.transform.SetParent(cardContainer.content.transform);//设置父物体
        }
    }

}