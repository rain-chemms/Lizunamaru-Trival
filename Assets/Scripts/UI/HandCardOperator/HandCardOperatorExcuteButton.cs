using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HandCardOperatorExcuteButton : MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        if(handCardOperator == null) handCardOperator = HandCardOperator.instance;
    }

    [SerializeField] private HandCardOperator handCardOperator;
    public HandCardOperator GetHandCardOperator() => handCardOperator;

    void Update()
    {
        CheckButtonEnable();
    }

    private void CheckButtonEnable()
    {
        if(handCardOperator == null || button == null) return;
        CardOperateCategory operateCategory = handCardOperator.GetOperateCategory();
        //至少的选择
        if(operateCategory == CardOperateCategory.AT_LEAST)
        {
            int num = (int)handCardOperator?.GetSelectedCards()?.Count;
            if(num >= (int)handCardOperator.GetOperateCount())//手牌数量满足要求,激活按钮
            {
                button.interactable = true;
            }
            else //手牌数量不满足要求,禁用按钮
            {
                button.interactable = false;
            }
        }
        else if(handCardOperator.GetOperateCategory() == CardOperateCategory.AT_MOST)
        {
            //至多选择时时刻保持按钮开启
            button.interactable = true;
        }
    }
}
