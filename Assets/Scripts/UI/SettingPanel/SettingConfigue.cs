//当前脚本存储具体的设置信息,用于json文件的序列化和反序列化
//纯数据类,不包含使用方法
using System;
using UnityEngine;

[Serializable]
public class SettingConfigue
{
    // 视频设置
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public float refreshRate = 60f;//刷新率
    public FullScreenMode fullScreenMode = FullScreenMode.FullScreenWindow;
    public int vSyncCount = 1;
    //音频设置:和混音器有关
    public float masterVolume = 1.0f;//主音量
    public float uiVolume = 1.0f;//UI音效
    public float bgmVolume = 1.0f;//背景音乐
    public float sfxVolume = 1.0f;//游戏物体音效
    public float ambienceVolume = 1.0f;//环境氛围音效
    public float humanVoiceVloume = 1.0f;//人声音效
}