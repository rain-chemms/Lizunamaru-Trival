using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System.Linq;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using UnityEngine.Events;


//该脚本用于激活SpellAttackDisplayer释放相应的特效
[RequireComponent(typeof(Card))]
public class CardSpellAttackWaker : MonoBehaviour
{
    //一下两个参数用于对技能进行本地化
    [SerializeField] private string textKey = "Spell_Debug";
    public string GetTextKey()
    {
        return textKey;
    }
    public void SetTextKey(string key)
    {
        textKey = key;
    }
    [SerializeField] private string searchTable = "SpellDisplayTexts";//搜索本地字典
    public string GetSearchTable()
    {
        return searchTable;
    }
    public void SetSearchTable(string table)
    {
        searchTable = table;
    }
    //用于显示符卡攻击的人物立绘
    [SerializeField] private Sprite sprite;
    public Sprite GetSprite()
    {
        return sprite;
    }
    public void SetSprite(Sprite sprite)
    {
        this.sprite = sprite;
    }
    public IEnumerator WakeSpellAttackDisplayer(bool leftOrRight = true)
    {
        SpellAttackDisplayer instance = SpellAttackDisplayer.instance;
        //设置图像
        Image img = instance?.GetRoleImage();
        if (img != null)
        {
            img.sprite = sprite;
        }
        //设置文本
        yield return LocalizationSettings.InitializationOperation;
        string keyName = textKey;
        // 异步获取
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(searchTable, keyName);
        yield return operation;
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            TMP_Text txt = instance?.GetSpellText();
            if (txt != null)
            {
                txt.text = operation.Result;
            }
        }
        else
        {
            Debug.LogError($"[CardSpellAttackWaker]: Display the Keywords Failed: {operation.OperationException}");
        }
        //播放动画
        instance?.GetAnimator().SetTrigger(leftOrRight ? "Left" : "Right");
    }
}
