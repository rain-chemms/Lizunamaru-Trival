using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardAnimatorEvents : MonoBehaviour
{
    [SerializeField] private Card card;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(card == null) card = GetComponent<Card>();     
    }

    //再消耗动画结束时调用
    public void AfterExhaust()
    {
        card.GetComponent<Animator>()?.SetBool("IsHidden",true);
    }
}
