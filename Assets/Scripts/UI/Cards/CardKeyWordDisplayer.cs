using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using System.Linq;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(Card))]
public class CardKeyWordDisplayer : MonoBehaviour
{
    [SerializeField] private string tableName = "CardKeywords";//指定查找的Table列表
    public void SetTableName(string name)
    {
        tableName = name;
    }
    public string GetTableName()
    {
        return tableName;
    }
    [SerializeField] private TMP_Text keyWordText;
    [SerializeField] public LocalizeStringEvent localizeEvent;
    [SerializeField] private Card card;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if (card == null) card = GetComponent<Card>();
        if (keyWordText == null)
        {
            TMP_Text[] tl = GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text t in tl)
            {
                if (t.name.Equals("Discription"))
                {
                    if (t.GetComponent<LocalizeStringEvent>() != null)
                    {
                        keyWordText = t;
                        localizeEvent = t.GetComponent<LocalizeStringEvent>();
                        break;
                    }
                }
            }
        }
    }

    private UnityAction<string> saveAction = null;
    void OnEnable()
    {
        if (localizeEvent != null)
        {
            // 使用匿名方法包装,启动协程
            saveAction = (text) => StartCoroutine(OnLocalized(text));
            // 添加监听
            localizeEvent.OnUpdateString.AddListener(saveAction);
        }
    }

    void OnDisable()
    {
        if (localizeEvent != null && saveAction != null)
        {
            localizeEvent.OnUpdateString.RemoveListener(saveAction);
        }
    }
    //本地化触发后添加文本
    IEnumerator OnLocalized(string localizedText)
    {
        // 此时localizedText是已经翻译好的文本
        yield return GetKeywordTextList();
        keyWordText.text = localizedText + "\n" + "<color=#fff045>"+ string.Join("\n", keyWordTextList.ToArray()) + "</color=#fff045>";//将列表中的关键字一一列出添加到字符串添加到文本中
    }

    [SerializeField] private List<string> keyWordTextList = new List<string>();
    public List<string> GetKeyWordTextList()
    {
        return keyWordTextList.ToList();
    }
    private IEnumerator GetKeywordTextList()
    {
        if(keyWordTextList == null) keyWordTextList = new List<string>();
        keyWordTextList.Clear();//清空缓存的文本
        List<CardKeyWord> kwList = card?.GetCardKeyWords();
        if (kwList == null)
        {
            Debug.LogError("[CardKeyWordDisplayer]: The cardKeyWords is null, Please Check!");
            yield return null;
        }
        // 等待本地化系统初始化完成
        yield return LocalizationSettings.InitializationOperation;
        // 异步获取指定表和Key的本地化字符串
        //string tableName = "CardKeywords";//指定查找的Table列表
        foreach (CardKeyWord kw in kwList)
        {
            //将UN_RETAIN这类的字符串转化为UnRetain
            string keyName = "CardKeyword_" + string.Concat(
                kw.ToString().Split('_')
                .Where(
                    s => s.Length > 0
                ) // 过滤掉连续下划线产生的空字符串
                .Select(
                    word => char.ToUpper(word[0]) + 
                    word.Substring(1).ToLower()
                )
            );
            // 异步获取
            var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, keyName);
            yield return operation;

            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                string localizedText = operation.Result;
                bool have = false;
                foreach(string k in keyWordTextList.ToList())
                {
                    if(k.Equals(localizedText))
                    {
                        have = true;
                        break;
                    }
                }
                if(!have) keyWordTextList.Add(localizedText);
            }
            else
            {
                Debug.LogError($"[CardKeyWordDisplayer]: Display the Keywords Failed: {operation.OperationException}");
            }
        }
    }
}
