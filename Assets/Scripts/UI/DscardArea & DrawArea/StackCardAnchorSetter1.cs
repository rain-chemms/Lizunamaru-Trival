using UnityEngine;
using System.Collections.Generic;

//检测并设置处于丢牌堆的卡牌的锚点
[RequireComponent(typeof(RectTransform))]
public class StackCardAnchorSetter : MonoBehaviour
{
    [SerializeField] private bool drawOrDiscard = true;//抽牌堆还是弃牌堆,true为抽牌堆,false为弃牌堆
    public void SetDrawOrDiscard(bool isDraw)
    {
        this.drawOrDiscard = isDraw;
    }

    [SerializeField] private Vector2 anchorAppendOffset = new Vector2(10f, 10f);//每张卡牌的锚点额外偏移量
    [SerializeField] private float lerpSpeed = 5;//锚点移动速度
    [SerializeField] private float rotateSpeed = 5;//锚点旋转速度
    [SerializeField] private RectTransform discardAreaRTF;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (discardAreaRTF == null) discardAreaRTF = GetComponent<RectTransform>();
        FreshCardListByDrawOrDiscard();
    }

    [SerializeField] private List<Card> cardList = null;
    //刷新卡牌列表
    public void FreshCardListByDrawOrDiscard()
    {
        if (drawOrDiscard is false)
        {
            cardList = BattleMessage.instance.GetDiscardCardList();//获取弃牌堆的卡牌列表
        }
        else
        {
            cardList = BattleMessage.instance.GetDrawCardList();//获取抽牌堆的卡牌列表
        }
    }

    //按索引设置弃牌堆中卡牌的层级
    public void SetCardInListSortOrder()
    {
        if (cardList == null) return;
        foreach (Card card in cardList)
        {
            if (card == null) continue;
            Canvas cardCanvas = card.GetComponent<Canvas>();
            if (cardCanvas != null)
            {
                cardCanvas.sortingOrder = cardList.IndexOf(card) - 100;
            }
        }
    }

    //设置所有卡牌列表中的卡牌的父物体为目标区域的父物体
    public void SetCardInListParent()
    {
        if (discardAreaRTF == null) return;
        foreach (Card card in cardList)
        {
            if (card == null) continue;
            card.transform.SetParent(discardAreaRTF.transform.parent);
        }
    }
    // 设置所有弃牌堆的卡牌锚点移动
    private void LerpAnchorAndRotate()
    {
        //目标区域不为空时且牌堆的卡牌列表不为空时
        if (discardAreaRTF != null && cardList != null)
        {
            int index = 0;
            foreach (Card card in cardList)
            {
                if(card.GetComponent<CardHandler>().IsDragging()) continue;
                //获取卡牌的锚点
                RectTransform cardRTF = card.GetComponent<RectTransform>();
                //获取卡牌的锚点在目标区域的位置
                if (cardRTF != null)//卡牌RectTransform不为空时
                {
                    //设置锚点
                    if (Vector2.Distance(cardRTF.anchorMax, discardAreaRTF.anchorMax) > 5.0f)
                    {
                        cardRTF.anchorMax = Vector2.Lerp(
                            cardRTF.anchorMax,
                            discardAreaRTF.anchorMax,
                            lerpSpeed * Time.deltaTime
                        );
                        cardRTF.anchorMin = Vector2.Lerp(
                            cardRTF.anchorMin,
                            discardAreaRTF.anchorMin,
                            lerpSpeed * Time.deltaTime
                        );
                    }
                    else
                    {
                        cardRTF.anchorMax = discardAreaRTF.anchorMax;
                        cardRTF.anchorMin = discardAreaRTF.anchorMin;
                    }
                    //设置偏移量
                    if (Vector2.Distance(cardRTF.anchoredPosition, discardAreaRTF.anchoredPosition + anchorAppendOffset * index) > 5.0f)
                    {
                        cardRTF.anchoredPosition = Vector2.Lerp(
                            cardRTF.anchoredPosition,
                            discardAreaRTF.anchoredPosition + anchorAppendOffset * index,
                            lerpSpeed * Time.deltaTime
                        );
                    }
                    else
                    {
                        cardRTF.anchoredPosition = discardAreaRTF.anchoredPosition + anchorAppendOffset * index;
                    }
                    //同步尺寸
                    if (Vector2.Distance(cardRTF.sizeDelta, discardAreaRTF.sizeDelta) > 5.0f)
                    {
                        cardRTF.sizeDelta = Vector2.Lerp(
                            cardRTF.sizeDelta,
                            discardAreaRTF.sizeDelta,
                            lerpSpeed * Time.deltaTime
                        );
                    }
                    else
                    {
                        cardRTF.sizeDelta = discardAreaRTF.sizeDelta;
                    }
                    //同步旋转
                    cardRTF.rotation = Quaternion.Lerp(
                        cardRTF.rotation,
                        discardAreaRTF.rotation,
                        rotateSpeed * Time.deltaTime
                    );
                }
                index++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        FreshCardListByDrawOrDiscard();
        SetCardInListParent();
        LerpAnchorAndRotate();
    }
}
