using UnityEngine;
using UnityEngine.EventSystems;
using CardVfxSystem;
using CardSystem;

//用于记录处于CardSelectOperator中的卡牌是否被选中
[RequireComponent(typeof(Card))]
public class CardCardSelectOperatorController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Card card;
    void OnEnable()
    {
        if (card == null) card = GetComponent<Card>();
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
        FreshCardVfx();
    }

    public void FreshCardVfx()
    {
        //依据当前的状态和card的类别设置Vfx显示
        string vfxXColor = "_Black";
        switch (card?.GetCardCategory())
        {
            case CardCategory.POWER:
                vfxXColor = "_Green";
                break;
            case CardCategory.GADGET:
                vfxXColor = "_Blue";
                break;
            case CardCategory.ATTACK:
                vfxXColor = "_Red";
                break;
            case CardCategory.SPELL_ATTACK:
                vfxXColor = "_White";
                break;
            case CardCategory.CURSE:
                vfxXColor = "_Purple";
                break;
            case CardCategory.STATUS:
                vfxXColor = "_DarkGreen";
                break;
            case CardCategory.EFFECTIVE:
            default:
                vfxXColor = "_Black";
                break;
        }
        //尝试打开Vfx
        CardVfxDisplayer dsp = card?.GetComponentInChildren<CardVfxDisplayer>();
        if(isSelected) dsp?.OpenVfx("SpreadGlow" + vfxXColor);
        else dsp?.CloseVfx("SpreadGlow" + vfxXColor);    
    }
}
