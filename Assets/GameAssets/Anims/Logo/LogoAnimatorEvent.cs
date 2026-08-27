using UnityEngine;
using System.Collections;

public class LogoAnimatorEvent : MonoBehaviour
{
    //Logo播放完之后加载主场景
    [SerializeField] private string loadSceneName = "MenuScene";
    public void AfterTheLoadEnd()
    {
        SceneLoader.instance.LoadScene(loadSceneName);
    }

    //初始化单例对象,关闭一些单例
    public void InitialTheInstances()
    {
        //关闭的单例物体
        //战斗系统相关
        BattleMessage.instance?.gameObject?.SetActive(false);//战斗信息
        BattleMessageDisplayer.instance?.gameObject?.SetActive(false);//战斗信息显示器
        BattleBoard.instance?.gameObject?.SetActive(false);//战斗棋盘
        ConcentratePoint.instance?.gameObject?.SetActive(false);//瞄准点
        RoundChangeDisplayer.instance?.gameObject?.SetActive(false);//回合显示器
    }

    [SerializeField] public string afterLoadBgm = "MenuBgm";
    public void SetAfterLoadBgm(string bgm) => afterLoadBgm = bgm;
    public string GetAfterLoadBgm() => afterLoadBgm;
    
    //播放默认的背景音乐
    public void PlayDefaultBgm()
    {
        BgmController.instance?.PlayBgm(afterLoadBgm);
    }
}
