using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using CardSystem;
using NUnit.Framework;
using System;

[RequireComponent(typeof(Card))]
[RequireComponent(typeof(Canvas))]
public class CardHandler : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler
{

    [SerializeField] private bool isDragging = false;// 记录是否正在拖拽
    [SerializeField] private bool inHand = false;
    [SerializeField] private bool inDiscard = false;
    [SerializeField] private bool inDraw = false;
    [SerializeField] private bool inExhaust = false;
    [SerializeField] private bool inSlot = false;
    public bool IsDragging() => isDragging;
    public void SetIsDragging(bool isDragging) => this.isDragging = isDragging;

    [SerializeField] private float cardLerpSpeed = 10.0f;
    [SerializeField] private float rotateSpeed = 10f;// 旋转速度
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private Vector2 dragOffset = new Vector2(0.0f, 0.0f);// 拖拽时图片的额外偏移量
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
    
    private RectTransform parentRT;
    [SerializeField] private Vector2 grabOffset;      // 按下瞬间，卡牌 pivot 与鼠标的差
    public Vector2 GetGrabOffset() => grabOffset;
    public void SetGrabOffset(Vector2 grabOffset) => this.grabOffset = grabOffset;

    [SerializeField] private Vector2 dragTarget;
    public Vector2 GetDragTarget() => dragTarget;
    public void SetDragTarget(Vector2 dragTarget) => this.dragTarget = dragTarget;
    //用于恢复concentratePoint显示的临时数据
    internal struct ConcentratePointTempData
    {
        public bool isDisplay;
        public bool isLock;
        public Vector2Int startIndex;
    }
    private ConcentratePointTempData cPData = new ConcentratePointTempData();
    // 开始拖拽时触发
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        parentRT = rectTransform.parent as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRT, eventData.position, GetUICamera(), out Vector2 local))
        grabOffset = rectTransform.anchoredPosition - local;
        dragTarget = rectTransform.anchoredPosition;
        //尝试播放拖拽音效
        card?.GetComponent<CardVoiceController>()?.PlayCardVoice("Drag");
        Debug.Log("[CardHandler]: Mouse Begin Drag]");
        //若卡牌开启了拖拽显示
        bool openArrowline = (bool)card?.IsOpenCardArrowLine();
        CardArrowLine arrowLine = BattleMessageDisplayer.instance?.GetCardArrowLine();
        if(arrowLine != null)
        {
            arrowLine.IsOpen = openArrowline;
            if(openArrowline)
            {
                arrowLine.SetStartMode(CardArrowLine.ArrowPointMode.CardUI);
                arrowLine.SetCard(card);
            }
        }
    }


    private void SaveAndDisplayLockCTP()
    {
        bool openArrowline = (bool)card?.IsOpenCardArrowLine();
        if(openArrowline)
        {
            //记录并开启锁定点样式
            cPData.isDisplay = ConcentratePoint.instance.IsDisplay();
            cPData.isLock = ConcentratePoint.instance.IsLocked();
            cPData.startIndex = ConcentratePoint.instance.GetIndex();
            //显示并锁定
            ConcentratePoint.instance.SetDisplay(true);
            ConcentratePoint.instance.SetIsLocked(true);
        }
    }

    private void RestoreLockCTP()
    {
        //开启锁定点显示的时候恢复锁定点数据
        bool openArrowline = (bool)card?.IsOpenCardArrowLine();
        if(openArrowline)
        {
            ConcentratePoint.instance.SetDisplay(cPData.isDisplay);
            ConcentratePoint.instance.SetIsLocked(cPData.isLock);
            ConcentratePoint.instance.SetIndex(cPData.startIndex);
        }
    }


    private Camera GetUICamera()
    {
        Canvas c = GetComponentInParent<Canvas>();
        return (c != null && c.renderMode != RenderMode.ScreenSpaceOverlay) ? c.worldCamera : null;
    }

    // 结束拖拽时触发
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        card?.GetComponent<CardVoiceController>()?.PlayCardVoice("DisDrag");
        Debug.Log("[CardHandler]: Mouse End Drag]");
        //拖拽结束后关闭ArrowLine并恢复锁定点样式
        CardArrowLine arrowLine = BattleMessageDisplayer.instance?.GetCardArrowLine();
        if(arrowLine != null) 
        {
            arrowLine.IsOpen = false;
        }
    }

    // 鼠标按住并拖拽时触发
    public void OnDrag(PointerEventData eventData)
    {
        // 将鼠标的屏幕坐标转换为当前图片的本地坐标,拖拽时更新目标位置坐标
        /*
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera, out localMousePosition)) { }
        */
        if (parentRT == null) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRT, eventData.position, GetUICamera(), out Vector2 local))
        dragTarget = local + grabOffset + dragOffset;   // 只在成功时才更新 
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
            dragTarget,
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

    [NonSerialized] private bool lastIsDrag_CTP = false;
    void Update()
    {
        //非拖拽状态且不在手牌中时,图片位置 lerp 回到初始位置
        inHand = (bool)BattleMessage.instance?.GetHandCardList()?.Contains(card);
        inDiscard = (bool)BattleMessage.instance?.GetDiscardCardList()?.Contains(card);
        inDraw = (bool)BattleMessage.instance?.GetDrawCardList()?.Contains(card);
        inExhaust = (bool)BattleMessage.instance?.GetExhaustCardList()?.Contains(card);
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
        if (!isDragging && !inHand && !inSlot && !inDiscard && !inDraw && !inExhaust)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,
                Vector2.zero,
                cardLerpSpeed * Time.deltaTime
            );
        }

        //检测并设置锁定点样式
        if(lastIsDrag_CTP != isDragging)
        {
            lastIsDrag_CTP = isDragging;
            if(isDragging) SaveAndDisplayLockCTP();
            else RestoreLockCTP();
        }
    }


}
