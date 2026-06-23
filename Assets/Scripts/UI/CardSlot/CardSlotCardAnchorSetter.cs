using Unity.VisualScripting;
using UnityEngine;


//用于设置卡槽中的卡的锚点
[RequireComponent(typeof(CardSlot))]
[RequireComponent(typeof(RectTransform))]
public class CardSlotCardAnchorSetter : MonoBehaviour
{
    //速度控制
    [SerializeField] private float lerpSpeed = 10.0f;
    [SerializeField] private float rotateSpeed = 5.0f;
    //用于获取卡槽的位置
    [SerializeField] private RectTransform rectTransform;
    //用于获取卡槽中的卡
    [SerializeField] private CardSlot cardSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(cardSlot == null) cardSlot = GetComponent<CardSlot>();
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Card theCard = cardSlot.GetInnerCard();
        if(theCard != null)
        {
            RectTransform theCardRectTransform = theCard.GetComponent<RectTransform>();
            if(theCardRectTransform != null)
            {
                //设置锚点
                theCardRectTransform.anchorMin = rectTransform.anchorMin;
                theCardRectTransform.anchorMax = rectTransform.anchorMax;
                theCardRectTransform.anchoredPosition = Vector2.Lerp(
                    theCardRectTransform.anchoredPosition,
                    rectTransform.anchoredPosition,
                    lerpSpeed * Time.deltaTime
                );
                theCardRectTransform.rotation = Quaternion.Lerp(
                    theCardRectTransform.rotation,
                    rectTransform.rotation
                    ,rotateSpeed * Time.deltaTime
                );
            }
        }    
    }
}
