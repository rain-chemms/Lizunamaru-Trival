using UnityEngine;
using CardSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

//这个脚本用于控制卡片在鼠标点击点旋转的角度
[RequireComponent(typeof(Card))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CardHoverChecker))]
public class CardMousePressPointRotater : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IDragHandler,
    IPointerMoveHandler
{
    [SerializeField] private Card card;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CardHoverChecker hoverChecker;

    //本脚本叠加到卡牌上的 X/Y 倾斜,以"右乘"的方式作用在卡牌基底旋转的本地坐标系里。
    //其它负责 Z(手牌扇形 / 拖拽归零)的脚本合成旋转时必须右乘这个值,
    //不要再用 rotation.eulerAngles.x / .y 反解,否则 XYZ 三个轴会互相污染并逐帧漂移。
    public Quaternion LocalTilt => writtenTilt;
    private Quaternion writtenTilt = Quaternion.identity;

    void OnEnable()
    {
        if (card == null) card = GetComponent<Card>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null) diagonalLength = GetDiagonalLength(rectTransform);
        else diagonalLength = 1.0f;
        if (hoverChecker == null) hoverChecker = GetComponent<CardHoverChecker>();
        //初始化改变颜色的目标Image
        Image cardFace = GetComponentsInChildren<Image>(true).Where(img => img.name.Equals("CardFace")).FirstOrDefault();
        Image back = GetComponentsInChildren<Image>(true).Where(img => img.name.Equals("Back")).FirstOrDefault();
        Image front = GetComponentsInChildren<Image>(true).Where(img => img.name.Equals("Front")).FirstOrDefault();
        Image inStack = GetComponentsInChildren<Image>(true).Where(img => img.name.Equals("InStack")).FirstOrDefault();
        if (cardFace != null) controlImages.Add(cardFace);
        if (back != null) controlImages.Add(back);
        if (front != null) controlImages.Add(front);
        if (inStack != null) controlImages.Add(inStack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Handle(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Handle(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Handle(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Handle(eventData);
    }

    [SerializeField] private Vector2 lastLocalPos = Vector2.zero;
    private void Handle(PointerEventData eventData)
    {
        RectTransform rect = GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPos))
        {
            // localPos 就是鼠标相对于这个 RectTransform 本地空间的位置
            lastLocalPos = localPos;
        }
    }
    [SerializeField] private const float rotateUnit = 0.125f;
    [SerializeField] private const float rotateSpeed = 10.0f;

    [SerializeField] private const float zRotateAppend = 2.5f;//z轴旋转角度附加值
    [SerializeField] private float diagonalLength = 0f;

    private float GetDiagonalLength(RectTransform rt)
    {
        if (rt == null) return 0f;

        float w = rt.rect.width;
        float h = rt.rect.height;
        return Mathf.Sqrt(w * w + h * h);
    }

    void Update()
    {
        CheckAndLerpXYRot();
        CheckAndSetCardImageColor();
    }
    [SerializeField] private const float centerBrightness = 0.85f;
    [SerializeField] private const float lowestBrightness = 0.65f;
    [SerializeField] private const float colorLerpSpeed = 1.0f;
    [SerializeField] private static Color baseColor = Color.HSVToRGB(0.0f, 0.0f, centerBrightness);//卡牌旋转为0时的基础颜色
    [SerializeField] private List<Image> controlImages = new List<Image>();
    //设置卡牌的Image基础颜色以模拟亮度控制
    private void CheckAndSetCardImageColor()
    {
        if (rectTransform == null) return;
        if (hoverChecker == null) return;
        float height = rectTransform.rect.height / 2;
        float dist = lastLocalPos.y / height;//鼠标距离中心的比例,自带正负号

        Color targetColor = baseColor;
        if (hoverChecker.IsHovering())
        {
            targetColor =
                Color.HSVToRGB(0.0f, 0.0f,
                    dist > 0
                    ?
                        centerBrightness + dist * (1.0f - centerBrightness)
                    :
                        centerBrightness + dist * (centerBrightness - lowestBrightness)
                );
        }

        foreach (Image img in controlImages)
        {
            if (img == null) continue;
            img.color = Color.Lerp(img.color, targetColor, colorLerpSpeed * Time.deltaTime);
        }
    }

    //只更新x和y轴的旋转角度
    private void CheckAndLerpXYRot()
    {
        if (rectTransform == null) return;
        if (hoverChecker == null) return;

        //目标倾斜:只含 X/Y,Z 恒为 附加值
        Quaternion targetTilt = Quaternion.identity;
        if (hoverChecker.IsHovering())
        {
            targetTilt = Quaternion.Euler(
                lastLocalPos.y * rotateUnit,
                -lastLocalPos.x * rotateUnit,
                //0.0f
                ///*
                lastLocalPos.x > 0
                ?
                    lastLocalPos.magnitude / diagonalLength * -zRotateAppend
                :
                    lastLocalPos.magnitude / diagonalLength * zRotateAppend
            //*/
            );
        }

        //先用上一帧的倾斜量剥离,再插值出本帧倾斜量回写,顺序不能颠倒
        Quaternion prevTilt = writtenTilt;

        //只插值倾斜量本身,不去插值整个 rotation
        writtenTilt = Quaternion.Lerp(
            prevTilt,
            targetTilt,
            rotateSpeed * Time.deltaTime
        );

        //从当前世界旋转里剥掉上一帧自己写进去的倾斜,剩下的就是其它脚本控制的基底(含 Z)
        Quaternion baseRot = rectTransform.rotation * Quaternion.Inverse(prevTilt);
        rectTransform.rotation = baseRot * writtenTilt;
    }
}
