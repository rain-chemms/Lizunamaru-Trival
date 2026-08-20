using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CardSystem;
using System.Collections.Generic;
using System.Linq;

//该脚本用于控制HandCardOperator内的卡牌位置
[RequireComponent(typeof(HandCardOperator))]
public class HandCardOperatorInnerCardSetter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private HandCardOperator handCardOperator;
    void OnEnable()
    {
        if(handCardOperator == null) handCardOperator = GetComponent<HandCardOperator>();
    }
    [SerializeField] private ScrollRect cardContainer = null;
    public ScrollRect GetCardContainer() => cardContainer;

    void Update()
    {
        CheckAndSetCardInHandCardOperator();
    }
    
    [Range(0.0f,0.5f)] [SerializeField] private float squeezePercent = 0.0f;//一行开始的挤压量,挤压量在两侧均生效
    public float GetSqueezePrecent() => squeezePercent;
    public void SetSqueezePrecent(float percent) => squeezePercent = Mathf.Clamp(percent, 0.0f, 0.5f);
    [SerializeField] private float rotateLerpSpeed = 0.0f;
    public float GetRotateLerpSpeed() => rotateLerpSpeed;
    public void SetRotateLerpSpeed(float speed) => rotateLerpSpeed = speed;
    private void CheckAndSetCardInHandCardOperator()
    {
        //获取所有载ScrollRect.content中显示的卡牌实体
        List<Card> innerCard = cardContainer?.content?.GetComponentsInChildren<Card>()?.ToList();
        foreach(Card card in handCardOperator?.GetSelectedCards())
        {
            if(card == null) continue;    
            if(innerCard.Contains(card)) continue;

            //不作为子物体的卡牌,将其作为子物体
            card.transform.SetParent(cardContainer.content);
            innerCard.Add(card);
        }
        //依据当前的卡牌的位置设置容器的锚点
        //横向均分,纵向按均设置为 y.max|min = 0.5
        
        int count = innerCard.Count;
        int index = 1;
        float seperate = (1f - 2 * squeezePercent) / count;//每个占据的百分比
        foreach(Card card in innerCard)
        {
            if(card == null) continue;
            RectTransform rtf = card.GetComponent<RectTransform>();
            Vector2 anchor = new Vector2(squeezePercent + (index - 1) * seperate, 0.5f); 
            rtf.anchorMin = new Vector2(anchor.x, anchor.y);
            rtf.anchorMax = new Vector2(anchor.x, anchor.y);
            //卡片处于非拖拽状态时尝试将其旋转到正常位置
            CardHandler handler = card.GetComponent<CardHandler>();
            if(handler!=null && !(bool)handler?.IsDragging())
            {
                rtf.rotation = Quaternion.Lerp(rtf.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotateLerpSpeed);
            }
            index++;
        }    
        
    }
}
