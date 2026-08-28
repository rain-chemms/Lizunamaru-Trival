using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections;
using System.Threading;

//设置面板:使用单例模式
public class SettingPanel : MonoBehaviour
{
    public static SettingPanel instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    [SerializeField] private ScrollRect partSelectView;//设置选择面板
    public ScrollRect GetPartSelectView() => partSelectView;

    [SerializeField] private RectTransform displayArea;//设置内容显示面板
    public RectTransform GetSettingPanelView() => displayArea;
    //保存的设置信息
    [NonSerialized] private SettingConfigue settingConfigue;
    public SettingConfigue GetSettingConfigue() => settingConfigue;
    public void SetSettingConfigue(SettingConfigue settingConfigue) => this.settingConfigue = settingConfigue;
    [NonSerialized] private string savePath = Path.Combine(Application.persistentDataPath, "game_settings.json");

    //混音器
    [SerializeField] private AudioMixer audioMixer;
    //应用音量设置
    public void ApplyVoiceSettingsToGame()
    {
        audioMixer.SetFloat("MasterVolume", LinearToDb((float)settingConfigue?.masterVolume));
        audioMixer.SetFloat("AmibienceVolume", LinearToDb((float)settingConfigue?.ambienceVolume));
        audioMixer.SetFloat("BgmVolume", LinearToDb((float)settingConfigue?.bgmVolume));
        audioMixer.SetFloat("HumanVoiceVolume", LinearToDb((float)settingConfigue?.humanVoiceVloume));
        audioMixer.SetFloat("SFXVolume", LinearToDb((float)settingConfigue?.sfxVolume));
        audioMixer.SetFloat("UIVolume", LinearToDb((float)settingConfigue?.uiVolume));
        Debug.Log("[ApplyTime]: Value of Sfx:" + settingConfigue.sfxVolume);
        Debug.Log("[ApplyTime]: Value of Ui:" + settingConfigue.uiVolume);
        Debug.Log("[ApplyTime]: Value of Ambience:" + settingConfigue.ambienceVolume);
        Debug.Log("[ApplyTime]: Value of Bgm:" + settingConfigue.bgmVolume);
        Debug.Log("[ApplyTime]: Value of HumanVoice:" + settingConfigue.humanVoiceVloume);
        Debug.Log("[ApplyTime]: Value of Master:" + settingConfigue.masterVolume);
        Debug.Log($"[SettingPanel]: Applied the Settings Configue to the AudioMixer.");
    }

    //用于将音量的1-0映射到-80dB-0dB
    public static float LinearToDb(float linear)
    {
        // 避免 log(0) 导致 -Infinity
        return linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    }

    public static float DbToLinear(float dB)
    {
        // -80dB 及以下视为静音，避免 Pow 计算极小值
        return dB <= -80f ? 0f : Mathf.Pow(10f, dB / 20f);
    }

    /// <summary>从文件加载设置，文件不存在则使用默认值</summary>
    public void LoadInSettingsFromFile()
     {
        //从文件中读取设置信息
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                SettingConfigue cfg = JsonUtility.FromJson<SettingConfigue>(json);
                settingConfigue = cfg;
                Debug.Log($"[SettingPanel]: Loaded the Settings Configue from: {savePath}");
            }
            catch(Exception e)
            {
                Debug.LogError($"[Settings] Failed to load, using defaults: {e.Message}");
                settingConfigue = new SettingConfigue();//使用默认值
            } 
        }
        else
        {
            Debug.Log("[Settings] No save file found, using defaults.");
            settingConfigue = new SettingConfigue();//使用默认值
        }
    }

    /// <summary>保存当前设置到文件</summary>
    public void SaveSettingsToFile()
    {
        //保存设置信息到文件中
        string json = JsonUtility.ToJson(settingConfigue, true);//格式化输出
        File.WriteAllText(savePath, json);
        Debug.Log($"[SettingPanel]: Saved the Settings Configue to: {savePath}");
    }

    //尝试自动获取ScrollRect
    void OnEnable()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game_settings.json");
        foreach (RectTransform child in transform)
        {
            if (child.name.Equals("DisplayArea")) { displayArea = child; continue; }
            ScrollRect scrollRect = child.GetComponent<ScrollRect>();
            if (scrollRect != null && child.name.Equals("PartSelectView"))
            {
                partSelectView = scrollRect;
                continue;
            }
        }
    }

    void Start()
    {
        //加载设置,等待其加载结束
        LoadInSettingsFromFile();
        //应用音量设置
        ApplyVoiceSettingsToGame();    
    }

    ///清除所有displayContent中的子物体
    public void ClearTheDisplayArea()
    {
        foreach (Transform child in displayArea?.transform)
        {
            Destroy(child.gameObject);
        }
    }

    [SerializeField] private SettingPanelSettingsContent displayConent;
    public void SetDisplayContent(SettingPanelSettingsContent settingPanelButton) => displayConent = settingPanelButton;
    public SettingPanelSettingsContent GetDisplayContent() => displayConent;

    //设置新的Content内容
    public void SetContentToDisplayArea()
    {
        SettingPanelSettingsContent cot = Instantiate(displayConent);//创建新的UI控件
        cot.transform.SetParent(displayArea.transform);//设置父节点为Map的ScrollRect 
        //设置左上角anchor.min为0max为1
        //设置四个方向的边距为0
        RectTransform rtf = cot.GetComponent<RectTransform>();
        if (rtf != null)
        {
            rtf.anchorMin = new Vector2(0.0f, 0.0f);
            rtf.anchorMax = new Vector2(1.0f, 1.0f);
            //rtf.localPosition = Vector3.zero;
            rtf.offsetMax = Vector2.zero;
            rtf.offsetMin = Vector2.zero;
        }
    }
}
