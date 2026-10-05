using UnityEngine;
using UnityEngine.UI;

namespace RoleChoiceUISystem
{
    [RequireComponent(typeof(Button))]
    public class RoleUICloseButton : MonoBehaviour
    {
        [SerializeField] private RoleChoiceUI choiceUI;
        [SerializeField] private Button button;
        void OnEnable()
        {
            if(choiceUI == null) choiceUI = GetComponentInParent<RoleChoiceUI>();
            if(button == null) button = GetComponent<Button>();
            button.onClick.AddListener(CloseRoleChoiceUI);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(CloseRoleChoiceUI);
        }

        private void CloseRoleChoiceUI()
        {
            choiceUI.IsDisplay = false;
        }

    }
}
