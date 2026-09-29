using UnityEngine;
using System.Collections.Generic;
using TMPro;
using CardSystem;
using System;

[RequireComponent(typeof(TMP_Text))]
public class StackCardNumberDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        linkedCardList = BattleMessage.instance?.GetCardListByName(linkedCardListName);
        lastCount = (int)linkedCardList?.Count;
        if(text != null) text.text = lastCount.ToString();
    }

    [SerializeField] private string linkedCardListName;
    void OnEnable()
    {
        if(text == null) text = GetComponent<TMP_Text>();
    }
    private List<Card> linkedCardList;
    public void SetLinkedCardList(List<Card> list) => linkedCardList = list;
    
    void Update()
    {
        SyncCardNumber();
    }

    [NonSerialized] private float lastCount;
    private void SyncCardNumber()
    {
        if(linkedCardList == null) return;
        if(text == null) return;
        int nowCount = (int)linkedCardList?.Count;
        if(nowCount == lastCount) return;
        text.text = nowCount.ToString();
        lastCount = nowCount;
    }
}
