using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class HandCardDisplayer : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector2(0, 0);//卡牌的索引偏移
    [SerializeField] private float lerpSpeed = 1.0f;//偏移速度
    [SerializeField] private float maxRotate = 27.0f;//角度制  

    [SerializeField] private float rotateSpeed = 1.0f;//旋转速度
    [SerializeField] private List<Card> handCardList = null;//手牌列表从战斗信息中获取
    //基础锚点Canvas
    [SerializeField] private RectTransform baseCardAnchor = null;
    public RectTransform GetBaseCardAnchor()
    {
        return baseCardAnchor;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        handCardList = BattleMessage.instance.GetHandCardList();
        //尝试自动获取
        if (baseCardAnchor == null)
        {
            baseCardAnchor = GetComponent<RectTransform>();//获取子节点失败是,默认获取自身
            RectTransform[] anchors = this.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform anchor in anchors)
            {
                if (anchor.name == "BaseCardAnchor")
                {
                    baseCardAnchor = anchor;
                    break;
                }
            }
        }
    }

    private void ChangeHandCardRotation()
    {
        int cardCount = handCardList.Count;
        float center = (float)(cardCount / (int)2);//中间索引
        if(cardCount % 2 == 0) center -= 0.5f;
        float rotateUnit = maxRotate / cardCount;//旋转角度的单元
        int index = 0;
        foreach(Card card in handCardList)
        {
            if(card == null) continue;
            //检测是否正在拖拽
            if((bool)card.GetComponent<CardHandler>()?.IsDragging()) continue;
            //计算中心索引1
            //计算旋转角度
            float rotate = rotateUnit * (index - center);
            //获取并设置旋转角度
            RectTransform cardRTF = card.GetComponent<RectTransform>();
            if(cardRTF != null)
            {
                cardRTF.rotation = Quaternion.Lerp(
                    cardRTF.rotation,
                    baseCardAnchor.rotation * Quaternion.Euler(0, 0, rotate),
                    rotateSpeed * Time.deltaTime
                );    
            }
            index++;
        }
    
    }
    private void ChangeHandCardPosition()
    {
        int cardCount = handCardList.Count;
        float center = (float)(cardCount / (int)2);//中间索引
        if(cardCount % 2 == 0) center -= 0.5f;
        int index = 0;
        foreach(Card card in handCardList)
        {
            if(card == null) continue;
            //检测是否正在拖拽
            if((bool)card.GetComponent<CardHandler>()?.IsDragging()) continue;
            Vector2 allLerp = new Vector2(
                offset.x * (index - center),
                -Mathf.Abs(offset.y * (index - center))//y方向全部向下
            );
            //获取并设置位置
            RectTransform cardRTF = card.GetComponent<RectTransform>();
            Vector2 target = baseCardAnchor.anchoredPosition + allLerp;
            if(Vector2.Distance(cardRTF.anchoredPosition,target) > 1f)
            {
                cardRTF.anchoredPosition = Vector2.Lerp(
                    cardRTF.anchoredPosition,
                    target,
                    lerpSpeed * Time.deltaTime
                );
            }
            else
            {
                cardRTF.anchoredPosition = target;
            }
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeHandCardRotation();
        ChangeHandCardPosition();
    }
}