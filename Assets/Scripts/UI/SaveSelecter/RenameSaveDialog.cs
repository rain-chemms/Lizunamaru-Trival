using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class RenameSaveDialog : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private Canvas dialogPanel;

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    void OnEnable()
    {
        confirmButton?.onClick.AddListener(OnConfirm);
        cancelButton?.onClick.AddListener(OnCancel);
    }

    void OnDisable()
    {
        confirmButton?.onClick.RemoveListener(OnConfirm);
        cancelButton?.onClick.RemoveListener(OnCancel);    
    }

    private Action<string> onConfirmed;
    private Action<string> onCanceled;

    public void Show(Action<string> confirmed, Action<string> canceled = null)
    {
        onConfirmed = confirmed;
        onCanceled = canceled;
        //控制其开启
        dialogPanel.gameObject.SetActive(true);
    }

    public void OnConfirm()
    {
        onConfirmed?.Invoke(inputField?.text);
        ClearCallbacks();
        dialogPanel.gameObject.SetActive(false);
    }

    public void OnCancel()
    {
        onCanceled?.Invoke(inputField?.text);
        ClearCallbacks();
        dialogPanel.gameObject.SetActive(false);
    }

    private void ClearCallbacks()
    {
        onConfirmed = null;
        onCanceled = null;
    }
}