using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class InHandCardOverrideSortingController : MonoBehaviour
{
    [SerializeField] private Card card;
    void Start()
    {
        //尝试自动获取
        if(card == null) card = GetComponent<Card>();
    }
    // Update is called once per frame
    void Update()
    {
        ChangeTheCardSorting();
    }

    private void ChangeTheCardSorting()
    {
        List<Card> handCardList = BattleMessage.instance.GetHandCardList();
        if((bool)handCardList?.Contains(card))
        {
            int index = handCardList.IndexOf(card);
            Canvas cardCanvas = card.GetComponent<Canvas>();
            if(cardCanvas != null)
            {
                cardCanvas.sortingOrder = index + 1;
            }
        }   
    }
}
