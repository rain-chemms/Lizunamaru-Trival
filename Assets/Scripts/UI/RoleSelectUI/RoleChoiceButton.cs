using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Events;
using Unity.VisualScripting;

namespace RoleChoiceUISystem
{
    [RequireComponent(typeof(Button))]
    public class RoleChoiceButton : MonoBehaviour
    {
        [SerializeField] private RoleChoiceUI choiceUI;
        [SerializeField] private Button button;
        [SerializeField] private RoleChoiceData roleChoiceData;//相关的角色数据
        void OnEnable()
        {
            if(choiceUI == null) choiceUI = GetComponentInParent<RoleChoiceUI>();
            if(button == null) button = GetComponent<Button>();
            button.onClick.AddListener(SetProfileArtAndDisplay);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(SetProfileArtAndDisplay);
        }

        private void SetProfileArtAndDisplay()
        {
            //设置角色的基本信息
            BattleMessage.instance?.SetRoleChoiceData(roleChoiceData);
            //应用内部的立绘
            Transform cutf = choiceUI?.ProfileArtArea;
            foreach(Transform child in cutf)
            {
                Destroy(child.gameObject);
            }
            ProfileArt artPrefab = roleChoiceData.RoleProfileArt;
            ProfileArt art = null;
            if(artPrefab != null) art = Instantiate(roleChoiceData.RoleProfileArt,cutf?.transform);
            //接下来可以对art进行一些操作//
            //应用描述文字
            TMP_Text dscb = choiceUI.RoleDiscribeArea;
            //
            string key = roleChoiceData.RoleDiscribeKey;
            string searchHub = roleChoiceData.RoleDiscribeTableName;
            LocalizedString targetDsc = new LocalizedString(searchHub,key);
            var dscHandle = targetDsc.GetLocalizedStringAsync();
            // 异步获取完成后赋值文本
            dscHandle.Completed += h =>
            {
                if (h.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    dscb.text = h.Result;
                }
                dscHandle.Release();
            };
            //设置生命值和初始金币
            TMP_Text hpText = choiceUI.HpText;
            TMP_Text coinText = choiceUI.StartCoinText;
            uint coin = roleChoiceData.StartCoin; 
            float hp = roleChoiceData.StartMaxHp;
            if(hpText != null) hpText.text = "<color=red>" + hp.ToString() + "/" + hp.ToString() + "</color>";
            if(coinText != null) coinText.text = "<color=yellow>" + coin.ToString() + "</color>";
            //设置角色名
            LocalizedString targetName = new LocalizedString("RoleName", "RoleName_" + roleChoiceData.RoleName);
            TMP_Text nameText = choiceUI.NameText;
            var nameHandle = targetName.GetLocalizedStringAsync();
            // 异步获取完成后赋值文本
            nameHandle.Completed += h =>
            {
                if (h.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    nameText.text = h.Result;
                }
                nameHandle.Release();
            };
            if(nameText != null) nameText.text = roleChoiceData.RoleName;
        }
    }
}
