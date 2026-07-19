using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card))]
public class CardInsertSlotChecker : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [SerializeField] private Card card;
    [SerializeField] private CardSlot cardSlotUnderCard;//在卡牌图片下方的卡槽对象
    void Start()
    {
        //尝试自动获取
        if (card == null) card = GetComponent<Card>();
    }

    void Update()
    {
        //检测卡牌图片下方是否存在卡槽对象
        //CheckCardSlotUnderCard();
    }

    public void OnDrag(PointerEventData eventData)
    {
        CheckCardSlotUnderCard();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cardSlotUnderCard != null)
        {
            //如果当前卡牌已经在某一个卡槽中,则不处理
            foreach (CardSlot cardSlot in BattleMessage.instance.GetAllCardSlot())
            {
                if (cardSlot.GetInnerCard() == card) return;
            }

            //如果当前卡牌不在手牌中,则不处理
            if (!(bool)BattleMessage.instance?.IsCardInHand(card)) return;
            //如果当前卡牌无法被插入,则不处理
            if((bool)card?.GetCardKeyWords()?.Contains(CardKeyWord.UNINSERTABLE)) return;
            //卡片类别一致时才可设置值
            if (cardSlotUnderCard.GetSlotCardCategory() == card.GetCardCategory())
            {
                //若卡槽中已有卡牌,则将其放入弃牌堆
                Card cardInSlot = cardSlotUnderCard.GetInnerCard();
                if (cardInSlot != null)
                {
                    BattleMessage.instance.GetDiscardCardList().Add(cardInSlot);
                    ((CardFunctioner)cardInSlot).AfterRemoveFromSolt();//触发卡牌的移除效果
                    cardSlotUnderCard.SetInnerCard(null);
                }

                cardSlotUnderCard.SetInnerCard(card);
                //将卡牌的父级设置为卡槽的父级
                card.transform.SetParent(cardSlotUnderCard.transform.parent);
                //若在手牌中,将其从手牌中移除
                BattleMessage.instance.GetHandCardList().Remove(card);
                //触发卡牌的插入效果
                ((CardFunctioner)card).AfterInsertToSolt();
                Debug.Log("[CardInsertSlotChecker]: Set the CardSlot:<" +
                    cardSlotUnderCard.name + "|" + cardSlotUnderCard.GetSlotCardCategory().ToString() + "(" + (int)cardSlotUnderCard.GetSlotCardCategory() + ")" + "> to the Card:<" +
                    card.name + "|" + card.GetCardCategory().ToString() + "(" + (int)card.GetCardCategory() + ")" + ">!");
            }
        }
    }

    //检测卡牌图片下方是否存在卡槽对象
    private void CheckCardSlotUnderCard()
    {
        //获取卡牌图片下方的物体
        RaycastHit2D[] hit2Ds = Physics2D.RaycastAll(card.transform.position, Vector2.down, 0.1f);
        cardSlotUnderCard = null;
        foreach (var hit2D in hit2Ds)
        {
            //判断是否是卡槽对象
            if (hit2D.collider.GetComponent<CardSlot>() != null)
            {
                //检测第一个卡槽
                cardSlotUnderCard = hit2D.collider.GetComponent<CardSlot>();
                break;
            }
            
        }
    }

}
