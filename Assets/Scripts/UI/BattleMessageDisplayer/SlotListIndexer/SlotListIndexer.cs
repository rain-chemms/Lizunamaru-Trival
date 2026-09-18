using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class SlotListIndexer : MonoBehaviour
{
    [SerializeField] private CardSlotList cardSlotList;
    void OnEnable()
    {
        // 尝试从父物体中获取CardSlotList
        if(cardSlotList == null) cardSlotList = GetComponentInParent<CardSlotList>();
    }

    [SerializeField] private int slotIndex = 0;
    public void SetSlotIndex(int index) => slotIndex = index;
    public int GetSlotIndex() => slotIndex;

    //依据当前的CardSlotList,滚动当前索引值
    public void RollBackSlotIndex()
    {
        if(cardSlotList == null) {slotIndex = 0;return ;}
        List<CardSlot> slots = cardSlotList.GetCardSlotList();
        if(slots == null) {slotIndex = 0;return ;}
        int count = slots.Count;
        if(slotIndex < 0) slotIndex = 0;
        else if(slotIndex >= count) slotIndex = count - 1;
        else if(slotIndex == 0) slotIndex = count - 1;
        else slotIndex--;
    }

    public void RollForwardSlotIndex()
    {
        if(cardSlotList == null) {slotIndex = 0;return ;}
        List<CardSlot> slots = cardSlotList.GetCardSlotList();
        if(slots == null || slots.Count <= 0) {slotIndex = 0;return ;}
        int count = slots.Count;
        if(slotIndex < 0) slotIndex = 0;
        else if(slotIndex >= count) slotIndex = count - 1;
        else if(slotIndex == count - 1) slotIndex = 0;
        else slotIndex++;
    }

    //获取对应索引的CardSlot
    public CardSlot GetIndexSlot()
    {
        if(cardSlotList == null) return null;
        List<CardSlot> slots = cardSlotList.GetCardSlotList();
        if(slots == null) return null;
        int count = slots.Count;
        if(slotIndex < 0 || slotIndex >= count) return null;
        return slots[slotIndex];
    }

    public void TriggerInnerCard()
    {
        StartCoroutine(GetIndexSlot()?.GetComponent<CardSlotEffectTriggerController>()?.TriggerInnerCard());
    }
}
