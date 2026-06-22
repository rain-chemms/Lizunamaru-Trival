using UnityEngine;
using TMPro;
using UnityEngine.UI;

//战斗时的游戏卡槽
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RawImage))]
public class CardSlot : MonoBehaviour
{
    [EnumRestrict(typeof(CardCategory),CardCategory.ATTACK, CardCategory.GADGET, CardCategory.POWER, CardCategory.SPELL_ATTACK)]
    [SerializeField] private CardCategory cardCategory = CardCategory.EFFECTIVE;//卡槽类别,默认这种是错误的
    public CardCategory GetSlotCardCategory()
    {
        return cardCategory;
    }
    //卡槽中目前存储的卡
    [SerializeField] private Card innerCard = null;
    public Card GetInnerCard()
    {
        return innerCard;
    }
}
