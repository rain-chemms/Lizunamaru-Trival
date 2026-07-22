using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(StackCardAnchorSetter))]
public class StackCardSortOrderSetter : MonoBehaviour
{
    [SerializeField] private string cardStackName;
    public string GetCardStackName() => cardStackName;
    public void SetCardStackName(string name) => cardStackName = name;
    [SerializeField] private List<Card> cardList;
    
    void Start()
    {
        cardList = BattleMessage.instance?.GetCardListByName(cardStackName);
    }
    
    void OnEnable()
    {
        cardList = BattleMessage.instance?.GetCardListByName(cardStackName);
    }
    
    [SerializeField] private int sortOffset = 10;
    public void SetSortOffset(int offset)
    {
        sortOffset = offset;
    }
    public int GetSortOffset()
    {
        return sortOffset;
    }
    void Update()
    {
        OrderTheCardSort();
    }
    private void OrderTheCardSort()
    {
        //if(drawOrDiscard) cardList = BattleMessage.instance?.GetDiscardCardList(); 
        if(cardList == null) return;
        int index = 0;
        foreach(Card card in cardList)
        {
            if(card == null) continue;
            if(card.GetComponent<Canvas>()!=null) card.GetComponent<Canvas>().sortingOrder = index + sortOffset;
            index++;
        }
    }
}
