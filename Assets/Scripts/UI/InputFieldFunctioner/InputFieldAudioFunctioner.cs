using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldAudioFunctioner : MonoBehaviour
{
    [SerializeField] private bool autoLinkFunction = false;
    [SerializeField] private TMP_InputField inputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(inputField == null) inputField = GetComponent<TMP_InputField>();
        //添加事件响应
        if(autoLinkFunction)
        {
            inputField?.onEndEdit.AddListener(OnEndEdit);
            inputField?.onValueChanged.AddListener(OnValueChanged);
            inputField?.onSelect.AddListener(OnSelect);
            inputField?.onDeselect.AddListener(OnDeselect);
        }
    }

    [SerializeField] private AudioSource endEditVoice;
    public void SetEndEditVoice(AudioSource endEditVoice)
    {
        this.endEditVoice = endEditVoice;
    }
    public AudioSource GetEndEditVoice()
    {
        return endEditVoice;
    }
    public void OnEndEdit(string value)
    {
        if(endEditVoice == null) return;
        endEditVoice.loop = false;
        endEditVoice.Play();
    }
    [SerializeField] private AudioSource valueChangeVoice;
    public void SetValueChangeVoice(AudioSource valueChangeVoice)
    {
        this.valueChangeVoice = valueChangeVoice;
    }
    public AudioSource GetValueChangeVoice()
    {
        return valueChangeVoice;
    }
    public void OnValueChanged(string value)
    {
        if(valueChangeVoice == null) return;
        valueChangeVoice.loop = false;
        valueChangeVoice.Play();
    }
    [SerializeField] private AudioSource onSelectVoice;
    public void SetOnSelectVoice(AudioSource onSelectVoice)
    {
        this.onSelectVoice = onSelectVoice;
    }
    public AudioSource GetOnSelectVoice()
    {
        return onSelectVoice;
    }
    public void OnSelect(string text)
    {
        if(onSelectVoice == null) return;
        onSelectVoice.loop = false;
        onSelectVoice.Play();
    }
    [SerializeField] private AudioSource onDeselectVoice;
    public void SetOnDeselectVoice(AudioSource onDeselectVoice)
    {
        this.onDeselectVoice = onDeselectVoice;
    }
    public AudioSource GetOnDeselectVoice()
    {
        return onDeselectVoice;
    }
    public void OnDeselect(string text)
    {
        if(onDeselectVoice == null) return;
        onDeselectVoice.loop = false;
        onDeselectVoice.Play();
    }
}
