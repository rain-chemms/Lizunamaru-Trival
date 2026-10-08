using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveConfirmDialog : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private Canvas dialogPanel;
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
    
    private Action onConfirmed;
    private Action onCanceled;

    public void Show(Action confirmed, Action canceled = null)
    {
        onConfirmed = confirmed;
        onCanceled = canceled;
        //控制其开启
        dialogPanel.gameObject.SetActive(true);
    }

    public void OnConfirm()
    {
        onConfirmed?.Invoke();
        ClearCallbacks();
        dialogPanel.gameObject.SetActive(false);
    }

    public void OnCancel()
    {
        onCanceled?.Invoke();
        ClearCallbacks();
        dialogPanel.gameObject.SetActive(false);
    }

    private void ClearCallbacks()
    {
        onConfirmed = null;
        onCanceled = null;
    }
}