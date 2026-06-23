using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class SpellSlideValueDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(text == null) text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = ((int)(BattleMessage.instance.GetSpellPrecent() * 100)).ToString()+ "%";
    }
}
