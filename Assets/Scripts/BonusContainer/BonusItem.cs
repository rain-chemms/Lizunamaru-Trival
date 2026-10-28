using UnityEngine;
using UnityEngine.UI;

namespace BonusSystem
{
    //奖励选项
    [RequireComponent(typeof(Button))]
    public class BonusItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        void OnEnable()
        {
            if(button == null) button = GetComponent<Button>();
        }

        
    }
}