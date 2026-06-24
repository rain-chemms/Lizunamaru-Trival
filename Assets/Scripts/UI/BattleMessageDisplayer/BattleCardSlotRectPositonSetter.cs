using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


//用于读取所有的卡槽列表,并依据卡槽的索引设置卡槽的UI位置
//必须挂载在BattleMessageDisplayer上
[RequireComponent(typeof(BattleMessageDisplayer))]
public class BattleCardSlotRectPositonSetter : MonoBehaviour
{
    [Header("当前超出的卡槽列表时错开的偏移量")]
    [SerializeField] private float offsetY = 0.1f;
    [SerializeField] private float offsetX = 0.025f;
    [SerializeField] private float darknessV = 0.25f;//暗淡值:降低的亮度
    [SerializeField] private float lerpSpeed = 5.0f;

    void Update()
    {
        SetCardSlotRectPosition();   
    } 
    
    //设置卡槽的UI位置
    public void SetCardSlotRectPosition()
    {
        //获取全部的卡槽列表
        List<CardSlotList> cardSlotListList = BattleMessage.instance.GetCardSlotListList();
        //遍历所有卡槽列表,并将它们依次排序
        foreach (CardSlotList cardSlotList in cardSlotListList)
        {
            CheckAllSlotAndSetPosition(cardSlotList);
        }   
    }

    //count为参考个数
    private void CheckAllSlotAndSetPosition(CardSlotList cardSlotList)
    {
        float allLength = 0.0f;//所有卡槽的长度总和 ,>1时代表长度超长了,需要缩位处理

        foreach(CardSlot cardSlot in cardSlotList.GetCardSlotList())
        {
            Vector2 Min = cardSlot.GetComponent<RectTransform>().anchorMin;
            Vector2 Max = cardSlot.GetComponent<RectTransform>().anchorMax;
            //获取卡槽X方向的min-max Anchor的位置,获取其长度
            float length = Max.x - Min.x;//当前卡槽占有的长度
            //Y方向固定设置为min = 0,max = 1
            //设置min
            cardSlot.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(
                cardSlot.GetComponent<RectTransform>().anchorMin,
                new Vector2(
                    allLength,
                    0.0f
                ),
                lerpSpeed * Time.deltaTime
            )
            ;
            //设置max
            cardSlot.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(
                cardSlot.GetComponent<RectTransform>().anchorMax,
                new Vector2(
                    allLength + length,
                    1.0f
                ),
                lerpSpeed * Time.deltaTime
            );
            allLength += length;
        }

        //长度超长了,需要缩位处理 =>更换每张卡片的起始Min.x的位置
        if(allLength > 1.0f)
        {
            //缩位置的比例
            float scale = 1.0f / allLength;
            //对起始点应用缩放处理
            int index = 0;
            foreach (CardSlot cardSlot in cardSlotList.GetCardSlotList())
            {
                Vector2 Min = cardSlot.GetComponent<RectTransform>().anchorMin;
                Vector2 Max = cardSlot.GetComponent<RectTransform>().anchorMax;
                
                cardSlot.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(
                    cardSlot.GetComponent<RectTransform>().anchorMin,
                    new Vector2(
                        Min.x * scale + (index / 2) * offsetX,
                        Min.y - index % 2 * offsetY
                    ),
                    lerpSpeed * Time.deltaTime
                    );
                cardSlot.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(
                    cardSlot.GetComponent<RectTransform>().anchorMax,
                    new Vector2(
                        Max.x - (Min.x - Min.x * scale) + (index / 2) * offsetX,
                        Max.y - index % 2 * offsetY
                    ),
                    lerpSpeed * Time.deltaTime
                );
                //微调卡牌的亮度,使其亮度降低
                if(index % 2 == 1)
                {
                    Image image = cardSlot.GetComponent<Image>();
                    if(image!=null) 
                    {
                        Color currentColor = image.color;
                        float a = currentColor.a;
                        float h, s, v;
                        Color.RGBToHSV(currentColor, out h, out s, out v);//获取当前颜色
                        v -= darknessV; //亮度降低
                        Color newColor = Color.HSVToRGB(h, s, v);
                        newColor.a = a;
                        image.color = newColor;
                        //强制刷新UI
                        image.SetVerticesDirty();
                    }
                }
                //调整图层顺序
                Canvas cvs = cardSlot.GetComponent<Canvas>();
                if (cvs != null)
                {
                    cvs.sortingOrder = -100 - index;
                }
                index++;//索引递增
            }
        }
    }
}
