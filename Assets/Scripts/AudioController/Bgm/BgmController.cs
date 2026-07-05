using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

//BGM控制器,为单例模式
public class BgmController : MonoBehaviour
{
    public static BgmController instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //自动添加子物体AudioSource
        AutoGetBgmList();
    }
    [SerializeField] List<AudioSource> bgmList = new List<AudioSource>();
    //自动添加子物体的AudioSource
    public void AutoGetBgmList()
    {
        if(bgmList == null) return;
        List<AudioSource> childAudioSource = GetComponentsInChildren<AudioSource>().ToList();
        if(childAudioSource != null)
        {
            foreach(AudioSource bgm in childAudioSource)
            {
                if(bgm == null) continue;
                if(!(bool)bgmList?.Contains(bgm))
                {
                    bgmList?.Add(bgm);
                }
            }
        }
    }
    public List<AudioSource> GetBgmList()
    {
        return bgmList.ToList();
    }
    public void AddBgm(AudioSource bgm)
    {
        if(!(bool)bgmList?.Contains(bgm)) bgmList?.Add(bgm);
    }
    public void RemoveBgm(AudioSource bgm)
    {
        if((bool)bgmList?.Contains(bgm)) bgmList?.Remove(bgm);
    }
    public AudioSource GetBgm(string bgmName)
    {
        if(bgmList == null) return null;
        AudioSource targetBgm = null;
        foreach(AudioSource bgm in bgmList)
        {
            if(bgm == null) continue;
            if(bgm.name.Equals(bgmName))
            {
                targetBgm = bgm;
                break;
            }
        }
        return targetBgm;
    }
    [SerializeField] private float fadeDuration = 1.0f;//淡入淡出时间
    public void SetFadeDuration(float duration)
    {
        fadeDuration = duration;
    }
    public float GetFadeDuration()
    {
        return fadeDuration;
    }
    /*
        淡入淡出相关控制逻辑
    */
    // 淡入淡出协程管理器
    // 用于追踪当前正在淡入淡出的协程，以便在切换时停止旧的协程
    private Dictionary<AudioSource, Coroutine> activeCoroutines = new Dictionary<AudioSource, Coroutine>();

    // 提供给外界桉树调用的公共方法,用于播放指定名称的BGM
    public void PlayBgm(string bgmName)
    {
        if(bgmList == null) return ;
        foreach (AudioSource bgm in bgmList.ToList())
        {
            if (bgm == null) continue;//跳过无效的AudioSource
            //找到目标音乐并开始淡入
            if (bgm.name.Equals(bgmName))
            {
                // 如果音乐还没播放，先播放起来（此时音量应为0）
                if (!bgm.isPlaying)
                {
                    bgm.Play();//播放音乐
                    bgm.volume = 0f;//初始音量设置为0
                }
                //启动或重启淡入协程
                StartFade(bgm, 1.0f);//音量设置为1.0f    
            }
            else if (bgm.isPlaying)//若非当前播放的音乐,则开始淡出
            {
                //启动淡出协程
                StartFade(bgm, 0.0f);//淡出为0.0f,自动停止
            }
        }
    }
    
    // 核心方法:启动或替换一个AudioSource的淡入淡出协程
    private void StartFade(AudioSource source, float targetVolume)
    {
        // 如果这个音源已经有协程在跑了,先停掉它,避免冲突
        if (activeCoroutines.ContainsKey(source))
        {
            StopCoroutine(activeCoroutines[source]);//停止旧的协程
            activeCoroutines.Remove(source);//移除旧的协程
        }
        
        // 启动新的淡入淡出协程并记录下来
        Coroutine newCoroutine = StartCoroutine(FadeVolume(source, targetVolume));
        activeCoroutines.Add(source, newCoroutine);//添加新的协程
    }
    
    //淡入淡出协程
    private IEnumerator FadeVolume(AudioSource source, float targetVolume)
    {
        float startVolume = source.volume;//获取当前初始音量
        float timeElapsed = 0f;//计时器
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            // 使用Lerp进行平滑插值
            source.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / fadeDuration);//音量插值变化
            yield return null;//等待一帧
        }
        // 确保最终音量精确
        source.volume = targetVolume;

        // 如果是淡出到0,并且音乐还在播放,就停止它
        if (targetVolume <= 0.0f && source.isPlaying)
        {
            source.Stop();
        }

        // 协程结束后,从字典中移除
        if(activeCoroutines.ContainsKey(source))
        {
            activeCoroutines.Remove(source);
        }
    }
}
