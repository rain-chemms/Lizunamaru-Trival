using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace GlobalSystem
{
    [RequireComponent(typeof(Button))]
    public class InputActionButtonBinder : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputAction;
        [SerializeField] private Button targetButton;

        private void OnEnable()
        {
            if(targetButton == null) targetButton = GetComponent<Button>();
            if (inputAction != null && inputAction.action != null)
            {
                inputAction.action.Enable();
                // 使用 started 确保只在按下瞬间触发一次，避免按住连续触发
                inputAction.action.started += OnTriggered;
            }
        }

        private void OnDisable()
        {
            if (inputAction != null && inputAction.action != null)
            {
                inputAction.action.started -= OnTriggered;
                inputAction.action.Disable();
            }
        }

        private void OnTriggered(InputAction.CallbackContext ctx)
        {
            // 模拟按钮点击，会自动触发 Button.onClick 中绑定的所有事件
            // 同时包含视觉反馈（颜色变化、动画等）
            targetButton.OnSubmit(null);
        }
    }
}
