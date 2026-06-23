using UnityEngine;

[RequireComponent(typeof(CardDisplayer))]
[RequireComponent(typeof(CardHandler))]
public class CardSIdeChanger : MonoBehaviour
{
    [SerializeField] private CardDisplayer cardDisplayer;    
    [SerializeField] private CardHandler cardHandler; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(cardHandler == null) cardHandler = GetComponent<CardHandler>();
        if(cardDisplayer == null) cardDisplayer = GetComponent<CardDisplayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cardHandler.IsDragging())
        {
            cardDisplayer.FlipTo(false);
        }
        else
        {
            cardDisplayer.FlipTo(true);
        }
    }
}
