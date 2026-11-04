using UnityEngine;
using CardSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        if (hoverChecker == null) hoverChecker = GetComponent<CardHoverChecker>();
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

    void Update()
    {
        CheckAndLerpXYRot();
    }

    //只更新x和y轴的旋转角度
    private void CheckAndLerpXYRot()
    {
        if (rectTransform == null) return;
        if (hoverChecker == null) return;

        //目标倾斜:只含 X/Y,Z 恒为 0,完全不受卡牌当前 Z 旋转影响
        Quaternion targetTilt = Quaternion.identity;
        if (hoverChecker.IsHovering())
        {
            targetTilt = Quaternion.Euler(
                lastLocalPos.y * rotateUnit,
                -lastLocalPos.x * rotateUnit,
                0.0f
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
