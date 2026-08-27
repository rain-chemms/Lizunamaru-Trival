using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HandCardOperatorExcuteButton : MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if (button == null) button = GetComponent<Button>();
        if (handCardOperator == null) handCardOperator = HandCardOperator.instance;
    }
    //关联的手牌执行器
    [SerializeField] private HandCardOperator handCardOperator;
    public HandCardOperator GetHandCardOperator() => handCardOperator;

    void Update()
    {
        CheckButtonEnable();
    }

    private void CheckButtonEnable()
    {
        if (handCardOperator == null || button == null) return;
        CardOperateCategory operateCategory = handCardOperator.GetOperateCategory();
        //至少的选择
        int num = (int)handCardOperator?.GetSelectedCards()?.Count;
        bool canOver = false;
        switch (operateCategory)
        {
            case CardOperateCategory.EQUAL:
                canOver = num == (int)handCardOperator.GetOperateCount();
                break;
            case CardOperateCategory.AT_LEAST:
                canOver = num >= (int)handCardOperator.GetOperateCount();
                break;
            case CardOperateCategory.NOT_ABOVE:
                canOver = num <= (int)handCardOperator.GetOperateCount();
                break;
            case CardOperateCategory.AT_MOST:
            default:
                canOver = true;
                break;
        }

        if (canOver)//手牌数量满足要求,激活按钮
        {
            button.interactable = true;
        }
        else //手牌数量不满足要求,禁用按钮
        {
            button.interactable = false;
        }
    }
}
