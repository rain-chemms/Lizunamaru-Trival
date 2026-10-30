using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BonusSystem.BonusViewSystem;
namespace BonusSystem
{
    //奖励选项,用来触发奖励生效器
    [RequireComponent(typeof(Button))]
    public class BonusItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        void OnEnable()
        {
            if(button == null) button = GetComponent<Button>();
            button.onClick.AddListener(GetBonus);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(GetBonus);
        }
        
        private void GetBonus()
        {
            StartCoroutine(BonusEffect());
        }

        private IEnumerator BonusEffect()
        {
            Bonus bonus = GetComponent<Bonus>();
            if(bonus != null)
            {
                yield return bonus.OnEffective();
                if(bonus.EffectiveSuccess)//若奖励生效,则移除奖励项物体
                {
                    Destroy(gameObject);
                }
            }
            else Destroy(gameObject);
        }
    }
}