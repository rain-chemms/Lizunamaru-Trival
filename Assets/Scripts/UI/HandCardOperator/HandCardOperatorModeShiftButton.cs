using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HandCardOperatorModeShiftButton : MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        button.onClick.AddListener(ShiftDisplayMode);
        squeezeModel = false;
        CheckAndSetModel();
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(ShiftDisplayMode);
    }

    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    public GridLayoutGroup GetGridLayoutGroup() => gridLayoutGroup;

    [SerializeField] private ContentSizeFitter contentSizeFitter;
    public ContentSizeFitter GetContentSizeFitter() => contentSizeFitter;

    [SerializeField] private bool squeezeModel = false; 
    private void ShiftDisplayMode()
    {
        squeezeModel = !squeezeModel;
        CheckAndSetModel();
    }

    private void CheckAndSetModel()
    {
        if(squeezeModel)
        {
            if(gridLayoutGroup != null) gridLayoutGroup.enabled = false;
            if(contentSizeFitter != null) contentSizeFitter.enabled = false;
        }
        else
        {
            if(gridLayoutGroup != null) gridLayoutGroup.enabled = true;
            if(contentSizeFitter != null) contentSizeFitter.enabled = true;
        }
    }
}
