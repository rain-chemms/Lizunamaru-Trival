using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardSystem;

[RequireComponent(typeof(Button))]
public class CardSelectOperatorCancelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if (button == null) button = GetComponent<Button>();
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
        bool canCancel = (bool)cardSelectOperator?.GetCanCancel();
        if(canCancel)
        {
            if(button != null) button.interactable = true;
        }
        else
        {
            if(button != null) button.interactable = false;
        }
    }

}