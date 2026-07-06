using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class BgmDisplayCanvas : MonoBehaviour
{
    [SerializeField] private string nowPlayingBgmName;
    public string GetNotPlayingBgmName()
    {
        return nowPlayingBgmName;
    }
    //文本显示相关
    [SerializeField] private BgmExplainTextDisplayer bgmExplainTextDisplayer;
    //搜索相关
    [SerializeField] private TMP_InputField searchField;//搜索框的文字
    [SerializeField] private ScrollRect bgmList;//Bgm列表
    //Bgm按钮预制体
    [SerializeField] private BgmListButton buttonPrefab;
    void Start()
    {
        InitTheBgmList();
    }
    //搜索框输入时检查并更改UI显示
    public void CheckSearchField()
    {
        if(searchField == null) return;
        if(bgmList == null) return;
        //获取BgmListButton列表
        List<BgmListButton> buttons = bgmList.content?.GetComponentsInChildren<BgmListButton>(true).ToList();
        if(buttons == null || buttons.Count == 0) return;
        //进行查询:当从头开始的字符均匹配时则代表有效(不区分大小写)
        foreach(BgmListButton button in buttons)
        {
            if(button == null) continue;
            //如果没有输入,则开启所有的按钮
            if(string.IsNullOrEmpty(searchField.text)) 
            {
                button.gameObject.SetActive(true);
                continue;
            }
            //获取Bgm名称
            string bgmName = button.GetBgmName();
            bool isMatch = bgmName.StartsWith(searchField.text, System.StringComparison.OrdinalIgnoreCase);
            Debug.Log("[BgmDisplayCanvas]: <" + bgmName +"> "+ (isMatch ? "Matched":"Not Matched"));
            if(isMatch)
            {
                button.gameObject.SetActive(true);//显示按钮
            }
            else
            {
                button.gameObject.SetActive(false);//隐藏按钮
            }
        }
    }

    private void InitTheBgmList()
    {
        //确保条件合适
        if(bgmList == null) return;
        if(bgmList.content == null) return;    
        if(buttonPrefab == null) return;
        if(bgmExplainTextDisplayer == null) return;
        //获取全部的Bgm
        List<AudioSource> audios = BgmController.instance?.GetBgmList();
        if(audios == null) return;
        foreach(AudioSource audio in audios)
        {
            if(audio == null) continue;//忽略空音频源
            //创建Bgm按钮
            BgmListButton button = Instantiate(buttonPrefab, bgmList.content);
            button.SetBgmName(audio.name);
            //添加点击事件
            UnityAction action = () => StartCoroutine(bgmExplainTextDisplayer.CheckTheBgmExplainText(audio.name));
            button.GetComponent<Button>()?.onClick.AddListener(action);
            //将按钮添加到列表中
            button.transform.SetParent(bgmList.content);
        }
    }
}
