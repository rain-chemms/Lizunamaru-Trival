using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(TMP_Text))]
public class BgmExplainTextDisplayer : MonoBehaviour
{
    [SerializeField] private string tableName = "MusicExplainText";//指定查找的Table列表
    public void SetTableName(string name)
    {
        tableName = name;
    }
    public string GetTableName()
    {
        return tableName;
    }
    [SerializeField] private TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }


    public IEnumerator CheckTheBgmExplainText(string bgmName)
    {
        if(text == null) yield break;//如果没有文本则返回
        string keyName = bgmName;
        if(string.IsNullOrEmpty(keyName)) yield break;//如果没有正在播放的Bgm则返回
        //检索本地化字典
        // 等待本地化系统初始化完成
        yield return LocalizationSettings.InitializationOperation;
        // 异步获取
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, keyName);
        yield return operation;

        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            string localizedText = operation.Result;
            text.text = keyName + "\n\t" + localizedText;//显示文本
        }
        else
        {
            Debug.LogError($"[CardKeyWordDisplayer]: Display the Keywords Failed: {operation.OperationException}");
        }
    }
}
