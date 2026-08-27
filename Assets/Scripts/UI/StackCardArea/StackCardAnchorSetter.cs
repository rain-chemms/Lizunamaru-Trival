using UnityEngine;
using System.Collections.Generic;
using CardSystem;


//检测并设置处于丢牌堆的卡牌的锚点
[RequireComponent(typeof(RectTransform))]
public class StackCardAnchorSetter : MonoBehaviour
{
    //[SerializeField] private bool drawOrDiscard = true;//抽牌堆还是弃牌堆,true为抽牌堆,false为弃牌堆
    /*
    public void SetDrawOrDiscard(bool isDraw)
    {
        this.drawOrDiscard = isDraw;
    }
    */
    [SerializeField] private string cardListName;//要关联的卡牌列表
    public void SetCardListName(string name) => cardListName = name;
    public string GetCardListName(string name) => name;
    [SerializeField] private Vector2 anchorAppendOffset = new Vector2(10f, 10f);//每张卡牌的锚点额外偏移量
    [SerializeField] private float lerpSpeed = 5;//锚点移动速度
    [SerializeField] private float rotateSpeed = 5;//锚点旋转速度
    [SerializeField] private RectTransform areaRTF;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (areaRTF == null) areaRTF = GetComponent<RectTransform>();
        FreshCardList();
    }

    void OnEnable()
    {
        FreshCardList();
    }

    [SerializeField] private List<Card> cardList;
    //刷新卡牌列表
    private void FreshCardList()
    {
        cardList = BattleMessage.instance?.GetCardListByName(cardListName);
    }
    /*
    public void FreshCardListByDrawOrDiscard()
    {
        if (drawOrDiscard is false)
        {
            cardList = BattleMessage.instance?.GetDiscardCardList();//获取弃牌堆的卡牌列表
        }
        else
        {
            cardList = BattleMessage.instance?.GetDrawCardList();//获取抽牌堆的卡牌列表
        }
    }
    */

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
        if (areaRTF == null) return;
        if(cardList == null) return;
        foreach (Card card in cardList)
        {
            if (card == null) continue;
            card.transform.SetParent(areaRTF.transform.parent);
        }
    }
    // 设置所有弃牌堆的卡牌锚点移动
    private void LerpAnchorAndRotate()
    {
        //目标区域不为空时且牌堆的卡牌列表不为空时
        if (areaRTF != null && cardList != null)
        {
            int index = 0;
            foreach (Card card in cardList)
            {
                if(card == null || (bool)card?.GetComponent<CardHandler>()?.IsDragging()) continue;
                //获取卡牌的锚点
                RectTransform cardRTF = card.GetComponent<RectTransform>();
                //获取卡牌的锚点在目标区域的位置
                if (cardRTF != null)//卡牌RectTransform不为空时
                {
                    //设置锚点
                    //查看卡牌的距离
                    //if (Vector2.Distance(cardRTF.anchorMax, discardAreaRTF.anchorMax) > 5.0f)
                    if (Vector2.Distance(cardRTF.position, areaRTF.position) > 5.0f)
                    {
                        cardRTF.anchorMax = Vector2.Lerp(
                            cardRTF.anchorMax,
                            areaRTF.anchorMax,
                            lerpSpeed * Time.deltaTime
                        );
                        cardRTF.anchorMin = Vector2.Lerp(
                            cardRTF.anchorMin,
                            areaRTF.anchorMin,
                            lerpSpeed * Time.deltaTime
                        );
                    }
                    else
                    {
                        cardRTF.anchorMax = areaRTF.anchorMax;
                        cardRTF.anchorMin = areaRTF.anchorMin;
                    }
                    //设置偏移量
                    //是否已经处于目标区域中
                    bool inTheArea = Vector2.Distance(cardRTF.anchoredPosition, areaRTF.anchoredPosition + anchorAppendOffset * index) > 5.0f;
                    if (inTheArea)
                    {
                        cardRTF.anchoredPosition = Vector2.Lerp(
                            cardRTF.anchoredPosition,
                            areaRTF.anchoredPosition + anchorAppendOffset * index,
                            lerpSpeed * Time.deltaTime
                        );
                    }
                    else
                    {
                        cardRTF.anchoredPosition = areaRTF.anchoredPosition + anchorAppendOffset * index;
                    }
                    //同步尺寸
                    if (Vector2.Distance(cardRTF.sizeDelta, areaRTF.sizeDelta) > 5.0f)
                    {
                        cardRTF.sizeDelta = Vector2.Lerp(
                            cardRTF.sizeDelta,
                            areaRTF.sizeDelta,
                            lerpSpeed * Time.deltaTime
                        );
                    }
                    else
                    {
                        cardRTF.sizeDelta = areaRTF.sizeDelta;
                    }
                    //计算卡牌与areaRTF之间的夹角
                    float angle = Vector2.Angle(areaRTF.position,cardRTF.position);
                    Quaternion tarQut = Quaternion.Euler(0f, 0f ,angle + 180.0f);
                    //同步旋转,若卡牌已经处于目标区域中,则与目标区域同步
                    /*
                    if(inTheArea)
                    {
                        tarQut = areaRTF.rotation;
                    }
                    */
                    cardRTF.rotation = Quaternion.Lerp(
                        cardRTF.rotation,
                        tarQut,
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
        //FreshCardList();
        //FreshCardListByDrawOrDiscard();
        SetCardInListParent();
        LerpAnchorAndRotate();
    }
}
