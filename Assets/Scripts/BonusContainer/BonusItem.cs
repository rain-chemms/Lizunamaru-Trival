using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BonusSystem.BonusViewSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BonusSystem
{
    //奖励选项,用来触发奖励生效器
    [RequireComponent(typeof(Button))]
    public class BonusItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        void OnEnable()
        {
            if (button == null) button = GetComponent<Button>();
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

        void Start()
        {
            StartCoroutine(FreshItemDisplay());
        }

        private IEnumerator FreshItemDisplay()
        {
            Bonus bonus = GetComponent<Bonus>();
            if (bonus == null) yield break;

            BonusItemViewer bonusViewSystem = GetComponent<BonusItemViewer>();
            if (bonusViewSystem == null) yield break;

            string bonusName = "Just a BonusItem";
            // 等待本地化系统初始化完成
            yield return LocalizationSettings.InitializationOperation;
            // 异步获取
            var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(bonus.DiscribeTable,bonus.DiscribeKey);
            yield return operation;
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                string localizedText = operation.Result;
                bonusName = localizedText;//显示文本
            }
            else
            {
                Debug.LogWarning("[BonusItem]: 本地化字符串获取失败：" + operation.OperationException);
            }
            bonusViewSystem.FreshView(bonus.PreviewSprite, bonusName, (int)bonus.BonusCount);
        }

        private IEnumerator BonusEffect()
        {
            Bonus bonus = GetComponent<Bonus>();
            if (bonus != null)
            {
                BonusContainer.instance.IsDisplay = false;
                yield return bonus.OnEffective();
                if (bonus.EffectiveSuccess)//若奖励生效,则移除奖励项物体
                {
                    Destroy(bonus.gameObject);
                }
                BonusContainer.instance.IsDisplay = true;
            }
            else Destroy(gameObject);
        }
    }
}