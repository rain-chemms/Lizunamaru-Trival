using UnityEngine;
using UnityEngine.UI;

namespace RoleChoiceUISystem
{
    [RequireComponent(typeof(Button))]
    public class NewGameStartButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        void OnEnable()
        {
            if(button == null) button = GetComponent<Button>();            
            button?.onClick?.AddListener(CreateNewGame);
        }

        void OnDisable()
        {
            button?.onClick?.RemoveListener(CreateNewGame);    
        }

        private void CreateNewGame()
        {
            
        }
    }
}
