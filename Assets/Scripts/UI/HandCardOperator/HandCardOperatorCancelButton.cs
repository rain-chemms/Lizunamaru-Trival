using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HandCardOperatorCancelButton : MonoBehaviour
{
    //关联的手牌执行器
    [SerializeField] private HandCardOperator handCardOperator;
    public HandCardOperator GetHandCardOperator() => handCardOperator;

    [SerializeField] private Button button;
    void OnEnable()
    {
        if (button == null) button = GetComponent<Button>();
        if (handCardOperator == null) handCardOperator = HandCardOperator.instance;
    }
    
    void Update()
    {
        CheckButtonEnable();
    }

    private void CheckButtonEnable()
    {
        bool canCancel = (bool)handCardOperator?.GetCanCancel();
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
