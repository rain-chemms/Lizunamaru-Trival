using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using CardSystem;

//该脚本用于触发卡牌加入手牌选择器的效果
//默认情况下它是关闭的

[RequireComponent(typeof(Card))]
public class CardHandCardOperatorController : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private Card card;
    void OnEnable()
    {
        if(card == null) card = GetComponent<Card>();
    }

    //点击时触发的事件
    public void OnPointerClick(PointerEventData eventData)
    {
        //将当前卡牌在HandOperator和手牌列表中进行切换
        HandCardOperator instance = HandCardOperator.instance;
        if(instance == null) return;
        //当前卡牌已经被选中在HandOperator中了
        if(instance.IsCardSelected(card)) HandCardOperator.instance.RemoveCard(card);//将其从HandOperator中移除,并加入手牌
        else
        {
            //若当前超出了选中数量则返回
            int num = (int)instance?.GetSelectedCards()?.Count;
            if(num >= (int)instance.GetOperateCount()) return;
            HandCardOperator.instance.AddCard(card);//否则加入HandOperator中
        }
    }
}