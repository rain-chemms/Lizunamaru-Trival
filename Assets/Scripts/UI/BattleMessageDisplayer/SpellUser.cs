using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//用于释放符卡的按钮
[RequireComponent(typeof(Button))]
public class SpellUser : MonoBehaviour
{
    [SerializeField] private Button button;

    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        button?.onClick.AddListener(TriggerSpellUse);
    }

    void OnDisable()
    {
        button?.onClick.RemoveListener(TriggerSpellUse);
    }

    private void TriggerSpellUse()
    {
        //当前未使用符卡是尝试触发符卡使用,双重保险
        if(!(bool)BattleMessage.instance?.IsUseingSpell()) 
            StartCoroutine(BattleMessage.instance.UseSpell(true));
    }
}
