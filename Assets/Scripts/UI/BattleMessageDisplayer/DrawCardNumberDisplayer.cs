using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class DrawCardNumberDisplayer : MonoBehaviour
{
    private TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(text == null) text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(text != null)
            text.text = BattleMessage.instance.GetDrawCardList()?.Count.ToString();
    }
}
