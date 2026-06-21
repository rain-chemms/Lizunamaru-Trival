using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//卡槽列表,必须挂在画布上
[RequireComponent(typeof(Canvas))]
public class CardSlotList : MonoBehaviour
{
    //管理的卡槽列表
    [SerializeField] private List<CardSlot> cardSlotList = new List<CardSlot>();
    


    //卡槽列表的种类
    [EnumRestrict(typeof(CardCategory),CardCategory.ATTACK, CardCategory.GADGET, CardCategory.POWER, CardCategory.SPELL_ATTACK)]
    [SerializeField] private CardCategory cardCategory = CardCategory.EFFECTIVE;//默认为卡槽类型,正常来说是错误类型
    public CardCategory GetSlotListCardCategory()
    {
        return cardCategory;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
