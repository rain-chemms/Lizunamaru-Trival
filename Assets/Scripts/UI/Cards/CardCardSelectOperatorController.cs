using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CardSystem;

//用于记录处于CardSelectOperator中的卡牌是否被选中
[RequireComponent(typeof(Card))]
public class CardCardSelectOperatorController : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private Card card;
    void OnEnable()
    {
        if(card == null) card = GetComponent<Card>();
    }
    [SerializeField] private bool isSelected = false;
    public bool IsSelected() => isSelected;
    public void SetSelected(bool isSelected) => this.isSelected = isSelected;
    //点击时触发的事件
    //点击时,切换选中状态
    //里面可以加一些UI显示的逻辑
    //比如说依据isSelected切换一些材质
    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;
    }
}
