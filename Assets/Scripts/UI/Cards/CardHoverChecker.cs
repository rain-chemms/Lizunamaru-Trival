using UnityEngine;
using UnityEngine.EventSystems;
using CardSystem;

//检测卡牌上是否有鼠标悬停,对卡牌的位置产生变换
[RequireComponent(typeof(Card))]
public class CardHoverChecker : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool isHovering = false;
    public bool IsHovering() => isHovering;
    // 鼠标进入时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        Debug.Log("鼠标悬停");
        // 例如：改变颜色、播放音效等
    }

    // 鼠标离开时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        Debug.Log("鼠标离开");
    }

    [SerializeField] private Card card;
    void OnEnable()
    {
        if(card == null) card = GetComponent<Card>();
    }
    //悬停的时候要显示卡牌的关键字解释
}
