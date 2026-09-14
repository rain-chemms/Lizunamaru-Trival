using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GlobalSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class UICursorSetter_WithoutClickCheck : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] private Texture2D hoverCursor = null;
        public Texture2D HoverCursor
        {
            get => hoverCursor;
            set => hoverCursor = value;
        }

        // 进入时切换为悬停光标
        public void OnPointerEnter(PointerEventData eventData)
            => Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);

        // 离开时恢复默认
        public void OnPointerExit(PointerEventData eventData)
            => Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);//设置会Unity的默认选项
    }
}
