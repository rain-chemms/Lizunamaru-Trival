using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SpellPrecentSliderDisplayer : MonoBehaviour
{
    [SerializeField] private Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(slider == null) slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        float spellPrecent = BattleMessage.instance.GetSpellPrecent();
        slider.value = spellPrecent;
    }
}
