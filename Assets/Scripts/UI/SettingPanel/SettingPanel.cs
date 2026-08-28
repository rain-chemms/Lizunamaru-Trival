using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

//设置面板:使用单例模式
public class SettingPanel : MonoBehaviour
{
    public static SettingPanel instance;
    private void Awake()
    {
        if(instance == null)
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
    [NonSerialized] private string savePath = Path.Combine(Application.persistentDataPath, "game_settings.json");
    
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
        string json = JsonUtility.ToJson(settingConfigue,true);//格式化输出
        File.WriteAllText(savePath, json);
        Debug.Log($"[SettingPanel]: Saved the Settings Configue to: {savePath}");
    }

    //尝试自动获取ScrollRect
    void OnEnable()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game_settings.json");
        foreach(RectTransform child in transform)
        {
            if(child.name.Equals("DisplayArea")) {displayArea = child;continue;}
            ScrollRect scrollRect = child.GetComponent<ScrollRect>();
            if(scrollRect!=null && child.name.Equals("PartSelectView")) 
            {
                partSelectView = scrollRect;
                continue;
            }
        }
        
        LoadInSettingsFromFile();
    }

    ///清除所有displayContent中的子物体
    public void ClearTheDisplayArea()
    {
        foreach(Transform child in displayArea?.transform)
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
        if(rtf != null)
        {
            rtf.anchorMin = new Vector2(0.0f, 0.0f);
            rtf.anchorMax = new Vector2(1.0f, 1.0f);
            //rtf.localPosition = Vector3.zero;
            rtf.offsetMax = Vector2.zero;
            rtf.offsetMin = Vector2.zero;
        }
    }
}
