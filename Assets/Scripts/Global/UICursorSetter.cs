using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GlobalSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class UICursorSetter : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private Texture2D hoverCursor = null;
        public Texture2D HoverCursor
        {
            get => hoverCursor;
            set => hoverCursor = value;
        }
        [SerializeField] private Texture2D pressCursor = null;
        public Texture2D PressCursor
        {
            get => pressCursor;
            set => pressCursor = value;
        }

        // 进入时切换为悬停光标
        public void OnPointerEnter(PointerEventData eventData)
            => Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);

        // 离开时恢复默认
        public void OnPointerExit(PointerEventData eventData)
            => Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);//设置会Unity的默认选项

        // 按下时切换为点击光标
        public void OnPointerDown(PointerEventData eventData)
            => Cursor.SetCursor(pressCursor, Vector2.zero, CursorMode.Auto);

        // 抬起时恢复悬停光标
        public void OnPointerUp(PointerEventData eventData)
            => Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
    }
}
