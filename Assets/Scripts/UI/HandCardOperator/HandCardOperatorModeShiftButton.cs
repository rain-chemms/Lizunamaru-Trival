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
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(ShiftDisplayMode);
    }

    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    public GridLayoutGroup GetGridLayoutGroup() => gridLayoutGroup;

    private void ShiftDisplayMode()
    {
        gridLayoutGroup.enabled = !gridLayoutGroup.enabled;
    }
}
