using Unity.VisualScripting;
using UnityEngine;
using CardSystem;


//用于设置卡槽中的卡的锚点
[RequireComponent(typeof(CardSlot))]
[RequireComponent(typeof(RectTransform))]
public class CardSlotCardAnchorSetter : MonoBehaviour
{
    //速度控制
    [SerializeField] private float lerpSpeed = 100.0f;
    [SerializeField] private float rotateSpeed = 5.0f;
    //用于获取卡槽的位置
    [SerializeField] private RectTransform rectTransform;
    //用于获取卡槽中的卡
    [SerializeField] private CardSlot cardSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if (cardSlot == null) cardSlot = GetComponent<CardSlot>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame

    void Update()
    {
        SyncCardDisplay();
    }

    private void SetCardAnchorPosition(RectTransform crtf)
    {
        if (crtf != null)
        {
            //设置偏移位置
            crtf.anchoredPosition = Vector2.Lerp(
                crtf.anchoredPosition,
                rectTransform.anchoredPosition,
                lerpSpeed * Time.deltaTime
            );
        }

    }

    private void SetCardAnchor(RectTransform crtf)
    {
        if (crtf != null)
        {
            //设置锚点
            crtf.anchorMin = Vector2.Lerp(
                crtf.anchorMin,
                Vector2.zero,
                lerpSpeed * Time.deltaTime
            );

            crtf.anchorMax = Vector2.Lerp(
                crtf.anchorMax,
                Vector2.one,
                lerpSpeed * Time.deltaTime
            );
            //设置偏移位置
            crtf.anchoredPosition = Vector2.Lerp(
                crtf.anchoredPosition,
                rectTransform.anchoredPosition,
                lerpSpeed * Time.deltaTime
            );
        }

    }

    private void SetCardRotate(RectTransform crtf)
    {
        if (crtf != null)
        {
            //同步旋转
            crtf.rotation = Quaternion.Lerp(
                crtf.rotation,
                rectTransform.rotation
                , rotateSpeed * Time.deltaTime
            );
        }            
    }

    private void SetCardSizeDelta(RectTransform crtf)
    {
        if (crtf != null)
        {
            //同步尺寸
            crtf.sizeDelta = Vector2.Lerp(
                crtf.sizeDelta,
                rectTransform.sizeDelta,
                lerpSpeed * Time.deltaTime
            );
        }
    }

    public void SyncCardDisplay()
    {
        Card theCard = cardSlot?.GetInnerCard();
        if (theCard != null)
        {
            if ((bool)theCard.GetComponent<CardHandler>()?.IsDragging()) return;
            RectTransform crtf = theCard.GetComponent<RectTransform>();
            if (crtf != null)
            {
                SetCardAnchor(crtf);
                SetCardAnchorPosition(crtf);
                SetCardRotate(crtf);
                SetCardSizeDelta(crtf);
            }
        }
    }
}
