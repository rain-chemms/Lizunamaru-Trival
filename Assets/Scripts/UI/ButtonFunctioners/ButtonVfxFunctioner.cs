using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonVfxFunctioner : MonoBehaviour,
    IPointerEnterHandler
{
    [SerializeField] private Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(button == null) button = GetComponent<Button>();
        if(autolinkClickFunction) button.onClick.AddListener(PlayButtonClickVoice);
    }
    
    [SerializeField] private AudioSource clickVoice;
    [SerializeField] private bool autolinkClickFunction = true;
    public void SetClickVoice(AudioSource clickVoice)
    {
        this.clickVoice = clickVoice;
    }
    public AudioSource GetClickVoice()
    {
        return clickVoice;
    }
    private void PlayButtonClickVoice()
    {
        if(clickVoice == null) return;
        if(clickVoice.loop) clickVoice.loop = false;//确保只播放一次
        clickVoice.Play();
    }

    [SerializeField] private AudioSource fouceVoice;
    public void SetFouceVoice(AudioSource fouceVoice)
    {
        this.fouceVoice = fouceVoice;
    }
    public AudioSource GetFouceVoice()
    {
        return fouceVoice;
    }
    private void PlayButtonFouceVoice()
    {
        if(fouceVoice == null) return;
        if(fouceVoice.loop) fouceVoice.loop = false;//确保只播放一次
        fouceVoice.Play();
    }
    //事件处理
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayButtonFouceVoice();
    }
}
