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
    
    [SerializeField] public int appendLayerCount = 1;
    [SerializeField] public float rotateLerpSpeed = 5;
    private void CheckAndSetInnerCard()
    {
        if(cardSelectOperator == null) return;
        if(cardContainer == null) return;
        //将内部的卡牌的父物体设置为ScrollRect的content
        foreach((Card card,Transform parent) in cardSelectOperator?.GetWillSelectCardList_Copy())
        {
            if(card == null) continue;
            card.transform.SetParent(cardContainer.content.transform);//设置父物体
            //设置卡牌的Canvas.layer
            Canvas cvs = card.GetComponent<Canvas>();
            Canvas selectorCvs = cardSelectOperator.GetComponent<Canvas>();
            if(cvs != null && selectorCvs != null)
            {
                cvs.sortingOrder = selectorCvs.sortingOrder + appendLayerCount;//设置图层
            }
            //尝试设置卡牌的旋转为0
            RectTransform rtf = card?.GetComponent<RectTransform>();
            if(rtf != null)
            {
                rtf.rotation = Quaternion.Lerp(rtf.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotateLerpSpeed);
            }

        }
    }

}