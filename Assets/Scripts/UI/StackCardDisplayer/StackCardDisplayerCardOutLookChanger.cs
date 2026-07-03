using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Rendering;
using System.Linq;


[RequireComponent(typeof(StackCardDisplayer))]
public class StackCardDisplayerCardOutLookChanger : MonoBehaviour
{
    [SerializeField] private int index = 2;
    [SerializeField] private int styleNum = 3;
    [SerializeField] private StackCardDisplayer stackCardDisplayer = null;
    public void IndexChange()
    {
        index = (index + 1) % 3;
    }

    void Start()
    {
        //尝试自动获取
        if(stackCardDisplayer == null) stackCardDisplayer = GetComponent<StackCardDisplayer>();
    }

    public void ChangeTheCardOutLookByIndex()
    {
        SerializedDictionary<Card,Card> displayCards = stackCardDisplayer.GetSourceCardDict();
        List<Card> cards = displayCards?.Keys?.ToList();
        if(cards == null) return;
        foreach(Card card in cards)
        {
            CardDisplayer cdp = card.GetComponent<CardDisplayer>();
            if(cdp == null) continue;
            switch(index)
            {
                case 2:
                    cdp.SetInStack(false);
                    cdp.FlipTo(false);        
                    break;
                case 1:
                    cdp.SetInStack(false);
                    cdp.FlipTo(true);        
                    break;
                case 0:
                default:
                    cdp.SetInStack(true);
                    cdp.FlipTo(false);        
                    break;
            }
        }
    }
}
