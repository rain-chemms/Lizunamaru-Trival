using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class KeyMappingItemPanel : MonoBehaviour
{
    [Header("键位绑定相关")]
    [SerializeField] private InputActionReference keyReference;
    [SerializeField] private int bindingIndex = 0; 

    [Header("UI控件相关")]
    [SerializeField] private Button listenButton;
    [SerializeField] private TMP_Text keyText;

    [Header("配置")]
    [SerializeField] private float timeout = 5f;
    [SerializeField] private string waitingText = "...";

    private InputActionRebindingExtensions.RebindingOperation _rebindOp;

    private void Awake()
    {
        listenButton.onClick.AddListener(StartInteractiveRebind);
        UpdateDisplay();
    }

    /// <summary>
    /// 核心: 点击按钮 → 启动交互式重绑定
    /// </summary>
    public void StartInteractiveRebind()
    {
        // 1.防止重复触发
        if (_rebindOp != null && !_rebindOp.completed) return;

        // 2.UI设置: 禁用按钮 + 显示等待文字
        listenButton.interactable = false;
        keyText.text = waitingText;

        // 3.获取目标 Action
        InputAction action = keyReference.action;

        // 4.启动交互式重绑定操作
        _rebindOp = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse/*")           // 排除鼠标（按需调整）
            .WithCancelingThrough("<Keyboard>/escape")   // ESC 取消
            .WithTimeout(timeout)
            .OnMatchWaitForAnother(0.1f)                // 防抖，等待更精确输入
            .OnComplete(operation =>
            {
                Debug.Log($"[KeyMappingItemPanel]: Rebinding KeyMapping Complete: {operation.selectedControl.path}!");
                operation.Dispose();
                FinishRebind(true);
            })
            .OnCancel(operation =>
            {
                Debug.Log("[KeyMappingItemPanel]: Rebinding Canceled or Overtime!");
                operation.Dispose();
                FinishRebind(false);
            });

        _rebindOp.Start();
    }

    /// <summary>
    /// 重绑定结束后的统一清理
    /// </summary>
    [SerializeField] private AudioSource successSound;
    [SerializeField] private AudioSource failSound;

    private void FinishRebind(bool success)
    {
        _rebindOp = null;
        listenButton.interactable = true;
        UpdateDisplay();
        //播放对应的音效
        if(success) successSound?.Play();
        else failSound?.Play();
    }

    /// <summary>
    /// 更新按钮上显示的当前绑定名称
    /// </summary>
    private void UpdateDisplay()
    {
        InputAction action = keyReference.action;
        var binding = action.bindings[bindingIndex];

        // GetBindingDisplayString 自动返回人类可读的名称
        // 如 "<Keyboard>/space" → "Space", "<Gamepad>/buttonSouth" → "A (Xbox)"
        string displayString = action.GetBindingDisplayString(
            bindingIndex, 
            out _, 
            out _,
            InputBinding.DisplayStringOptions.DontUseShortDisplayNames
        );

        keyText.text = !string.IsNullOrEmpty(displayString) 
            ? displayString 
            : action.GetBindingDisplayString();
    }

    /// <summary>
    /// 重置此按钮对应的绑定为默认值
    /// </summary>
    public void ResetBinding()
    {
        keyReference.action.RemoveBindingOverride(bindingIndex);
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        // 必须清理,否则切换场景时会报错
        _rebindOp?.Dispose();
        listenButton.onClick.RemoveListener(StartInteractiveRebind);
    }

}
