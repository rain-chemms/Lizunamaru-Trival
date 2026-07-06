using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BgmPauseButton : MonoBehaviour
{
    [SerializeField] private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(button == null) button = GetComponent<Button>();
    }

    //暂停当前正在播放的音乐
    public void PauseOrResumeBgm()
    {
        string bgmName = BgmController.instance?.GetNowBgm();
        foreach (AudioSource bgm in BgmController.instance.GetBgmList().ToList())
        {
            if(bgm == null) continue;
            if (bgm.name.Equals(bgmName))
            {
                if(bgm.isPlaying)
                {
                    //bgm.Pause();
                    BgmController.instance?.StartFade(bgm, 0.0f, true);
                }
                else
                {
                    //bgm.Play();
                    if (!bgm.isPlaying)
                    {
                        bgm.Play();//播放音乐
                        bgm.volume = 0f;//初始音量设置为0
                    }
                    BgmController.instance?.StartFade(bgm, 1.0f , true);
                }
            }
        }
    }
}
