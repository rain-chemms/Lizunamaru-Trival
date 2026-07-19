using UnityEngine;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class StackCardNumberDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(text == null) text = GetComponent<TMP_Text>();
    }
    [SerializeField] private string linkedCardListName;
    void OnEnable()
    {
        linkedCardList = BattleMessage.instance?.GetCardListByName(linkedCardListName);
    }
    private List<Card> linkedCardList;
    public void SetLinkedCardList(List<Card> list) => linkedCardList = list;
    void Update()
    {
        if(text != null)
            text.text = linkedCardList?.Count.ToString();
    }
}
