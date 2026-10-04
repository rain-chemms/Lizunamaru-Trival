using System.Collections.Generic;
using System.Linq;
using CardSystem;
using UnityEngine;

public interface IAttackCardInsertSlotSetter
{
    public int RecoverPreRound{ set; get;}
    public void SetInsertSlotAttackTime(Card source)
    {
        //寻找这张卡对应的卡槽的卡槽触发器
        List<CardSlot> slots = BattleMessage.instance?.GetAllCardSlot();
        CardSlot target = null;
        foreach(CardSlot slot in slots.ToList())
        {
            if(slot == null) continue;
            Card innerCard = slot.GetInnerCard();
            if(innerCard == source)
            {
                target = slot;
                break;
            }
        }
        if(target == null) return ;//不在卡槽中
        //尝试获取卡槽的
        CardSlotEffectTriggerController ectr = target.GetComponent<CardSlotEffectTriggerController>(); 
        if(ectr == null) return;//没有卡牌触发器
        ectr.SetRecoverPreRound(RecoverPreRound);
    }
}