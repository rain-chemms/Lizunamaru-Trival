using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HandCardOperatorSqueezeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    void OnEnable()
    {
        if(slider == null) slider = GetComponent<Slider>();
    }
    
    [SerializeField] private HandCardOperatorInnerCardSetter innerCardSetter;

    public void SetPrecentToSetter()
    {
        innerCardSetter.SetSqueezePrecent((float)slider?.value);
    }
}