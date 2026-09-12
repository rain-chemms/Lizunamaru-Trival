using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using CardSystem;
using NUnit.Framework;

[RequireComponent(typeof(Card))]
[RequireComponent(typeof(Canvas))]
public class CardHandler : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler
{

    [SerializeField] private bool isDragging = false;// 记录是否正在拖拽
    [SerializeField] private bool inHand = false;
    [SerializeField] private bool inDiscard = false;
    [SerializeField] private bool inDraw = false;
    [SerializeField] private bool inSlot = false;
    public bool IsDragging() => isDragging;

    [SerializeField] private float cardLerpSpeed = 10.0f;
    [SerializeField] private float rotateSpeed = 10f;// 旋转速度
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private Vector2 dragOffset = new Vector2(-65, 100);// 拖拽时图片的额外偏移量
    public void SetDragOffset(Vector2 dOffset) => dragOffset = dOffset;
    public Vector2 GetDragOffset() => dragOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Card card;
    void OnEnable()
    {
        //尝试自动获取
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (card == null) card = GetComponent<Card>();
    }
    // 开始拖拽时触发
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        //尝试播放拖拽音效
        card?.GetComponent<CardVoiceController>()?.PlayCardVoice("Drag");
        Debug.Log("[CardHandler]: Mouse Begin Drag]");
    }

    // 结束拖拽时触发
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        card?.GetComponent<CardVoiceController>()?.PlayCardVoice("DisDrag");
        Debug.Log("[CardHandler]: Mouse End Drag]");
    }

    // 鼠标按住并拖拽时触发
    [SerializeField] private Vector2 localMousePosition;//鼠标在图片上的位置 
    public void OnDrag(PointerEventData eventData)
    {
        // 将鼠标的屏幕坐标转换为当前图片的本地坐标,拖拽时更新目标位置坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera, out localMousePosition)) { }
        //{

        // 更新图片的位置，减去初始偏移量防止图片瞬移到鼠标中心
        // 更新图片的位置，减去初始偏移量防止图片瞬移到鼠标中心
        //rectTransform.anchoredPosition += localMousePosition - offset;
        //    rectTransform.anchoredPosition = Vector2.Lerp(
        //        rectTransform.anchoredPosition,
        //        rectTransform.anchoredPosition + localMousePosition + dragOffset,
        //        cardLerpSpeed * Time.deltaTime
        //    );
        ///*
        //    rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,
        //        localMousePosition - offSet,
        //        cardLerpSpeed * Time.deltaTime
        //    );
        //    //*/
        //}
        //更新旋转
        //rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation,
        //    Quaternion.identity,
        //    rotateSpeed * Time.deltaTime
        //);
        //*/
    }


    private void ChangeCardPosAndRot()
    {
        if (!isDragging) return;
        // 更新图片的位置，减去初始偏移量防止图片瞬移到鼠标中心
        // 更新图片的位置，减去初始偏移量防止图片瞬移到鼠标中心
        //rectTransform.anchoredPosition += localMousePosition - offset;
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            localMousePosition + dragOffset,
            cardLerpSpeed * Time.deltaTime
        );
        /*
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,
            localMousePosition - offSet,
            cardLerpSpeed * Time.deltaTime
        );
        */

        //更新旋转
        rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation,
            Quaternion.identity,
            rotateSpeed * Time.deltaTime
        );
    }

    void Update()
    {
        //非拖拽状态且不在手牌中时,图片位置 lerp 回到初始位置
        inHand = BattleMessage.instance.GetHandCardList().Contains(card);
        inDiscard = BattleMessage.instance.GetDiscardCardList().Contains(card);
        inDraw = BattleMessage.instance.GetDrawCardList().Contains(card);
        inSlot = false;
        foreach (CardSlot csl in BattleMessage.instance.GetAllCardSlot())
        {
            if (csl.GetInnerCard() == card)
            {
                inSlot = true;
                break;
            }
        }

        ChangeCardPosAndRot();//检测并改变图片位置和旋转
        //非拖拽状态,非按下状态且不在手牌中时,图片位置 lerp 回到初始位置
        if (!isDragging && !inHand && !inSlot && !inDiscard && !inDraw)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,
                Vector2.zero,
                cardLerpSpeed * Time.deltaTime
            );
        }
    }
}
