using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using CardSystem;

[RequireComponent(typeof(Canvas))]
public class CardHandIndexDisplayer : MonoBehaviour
{
    public static bool displaySettingOpen;
    [SerializeField] private TMP_Text indexText;
    public TMP_Text IndexText { get => indexText; }
    [SerializeField] private Card card;
    [SerializeField] private Canvas canvas;
    void OnEnable()
    {
        if(card == null) card = GetComponentInParent<Card>();
        if(canvas == null) canvas = GetComponent<Canvas>();
        if(indexText == null) indexText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        SetIndexAndDisplay();
    }
    
    private int lastIndex = -1;
    private void SetIndexAndDisplay()
    {
        if(!displaySettingOpen) 
        {
            if(canvas != null) canvas.enabled = false;
            return;
        }
        
        if(canvas == null) return;
        if(card == null) return;
        List<Card> cardList = BattleMessage.instance?.GetHandCardList_Copy();
        if(cardList == null || !cardList.Contains(card))
        {
            canvas.enabled = false;
            return;
        }
        
        canvas.enabled = true;
        int index = cardList.IndexOf(card);
        if(lastIndex != index)
        {
            indexText.text = $"{index + 1}";
            lastIndex = index;
        }
    }
}