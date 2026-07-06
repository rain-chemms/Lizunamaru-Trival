using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
public class BgmSlider : MonoBehaviour,
    IPointerUpHandler,
    IPointerDownHandler
{
    [SerializeField] private Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(slider == null) slider = GetComponent<Slider>();
    }
    [SerializeField] private bool isDragging = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        ChangeTheBgmBySlide();
    }

    void Update()
    {
        if(!isDragging) WithBgmSetSliderValue();
    }
        
    //依据当前播放进度显示滑块
    public void WithBgmSetSliderValue()
    {
        if(slider == null) return;
        AudioSource audio = BgmController.instance.GetBgm(BgmController.instance.GetNowBgm());
        if(audio == null) return;
        slider.value = slider.maxValue * audio.time / audio.clip.length;//设置进度条
    }

    //更改音乐的进度
    public void ChangeTheBgmBySlide()
    {
        if(slider == null) return;
        AudioSource audio = BgmController.instance.GetBgm(BgmController.instance.GetNowBgm());
        if(audio == null) return;
        audio.time =  audio.clip.length * slider.value / slider.maxValue;//设置播放器进度
        //尝试播放修改音乐进度后的音效
        AudioSource sliderAS = slider.GetComponent<AudioSource>();
        if(sliderAS != null)
        {
            sliderAS.loop = false;
            sliderAS.Play();
        }
    }
}
