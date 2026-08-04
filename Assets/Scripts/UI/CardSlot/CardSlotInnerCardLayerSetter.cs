using UnityEngine;
using CardSystem;

[RequireComponent(typeof(CardSlot))]
public class CardSlotInnerCardLayerSetter : MonoBehaviour
{
    [SerializeField] private CardSlot cardSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        //获取卡槽组件
        if(cardSlot == null) cardSlot = GetComponent<CardSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        SetInnerCardLayer();//设置内部卡的UI层级
    }

    private void SetInnerCardLayer()
    {
        if(cardSlot == null) return;
        Card card = cardSlot.GetInnerCard();
        if(card == null) return;
        Canvas cardCanvas = card.GetComponent<Canvas>();
        Canvas cardSlotCanvas = cardSlot.GetComponent<Canvas>();
        if(cardCanvas != null && cardSlotCanvas != null)
        {
            cardCanvas.sortingOrder = cardSlotCanvas.sortingOrder + 1;
        }
        else
        {
            cardSlot.GetComponent<RectTransform>()?.SetAsFirstSibling();
        }
    }
}
