using UnityEngine;
using System.Collections;
using GridObjectSystem.RoleSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CardSystem
{
    //该脚本用于激活SpellAttackDisplayer释放相应的特效
    [RequireComponent(typeof(Card))]
    public class CardSpellAttackWaker : MonoBehaviour
    {
        //一下两个参数用于对技能进行本地化
        [SerializeField] private string textKey = "Spell_Debug";
        public string GetTextKey() => textKey;
        public void SetTextKey(string key) => textKey = key;
        
        [SerializeField] private string searchTable = "SpellDisplayTexts";//搜索本地字典
        public string GetSearchTable() => searchTable;
        public void SetSearchTable(string table) => searchTable = table;

        [SerializeField] private Sprite defaultSprite;
        public Sprite GetDefaultSprite() => defaultSprite;

        public IEnumerator WakeSpellAttackDisplayer(Role role,bool leftOrRight = true)
        {
            Sprite sprite = role?.GetSpellSprite();
            if(sprite == null) sprite = defaultSprite;
            yield return SpellAttackDisplayer.instance?.WakeDisplayer(sprite, leftOrRight, textKey, searchTable);
        }
    }
}