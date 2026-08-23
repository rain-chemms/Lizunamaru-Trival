using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardSystem;

[RequireComponent(typeof(Button))]
public class CardSelectOperatorExcuteButton : MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
    }
    //关联的卡牌选择执行器
    [SerializeField] private CardSelectOperator cardSelectOperator;
    public CardSelectOperator GetCardSelectOperator() => cardSelectOperator;
    
    void Update()
    {
        CheckButtonEnable();
    }

    //检测按钮是否可用
    private void CheckButtonEnable()
    {
        if(cardSelectOperator == null || button == null) return;
        CardOperateCategory operateCategory = cardSelectOperator.GetOperateCategory();//获取操作种类
        if(operateCategory == CardOperateCategory.AT_LEAST)
        {
            //获取有效选择的卡牌数量
            List<(Card,Transform)> cardData = cardSelectOperator.GetWillSelectCardList_Copy();
            int num = 0;
            foreach((Card card,Transform parent) cp in cardData)
            {
                Card card = cp.card;
                if(card == null) continue;
                CardCardSelectOperatorController ctr = card.GetComponent<CardCardSelectOperatorController>();
                if(ctr!=null)
                {
                    if(ctr.IsSelected()) num++;//当前卡牌处于选择状态,计数器+1
                }
            }
            //检测
            if(num >= (int)cardSelectOperator.GetOperateCount())//手牌数量满足要求,激活按钮
            {
                button.interactable = true;
            }
            else //手牌数量不满足要求,禁用按钮
            {
                button.interactable = false;
            }
        }
        else if(operateCategory == CardOperateCategory.AT_MOST)
        {
            //至多选择时时刻保持按钮开启
            button.interactable = true;
        }
    }
    
}
