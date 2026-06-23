using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Card))]
public class CardReturnHandChecker : MonoBehaviour,
    IEndDragHandler,IDragHandler
{
    [SerializeField] private Card card;
    [SerializeField] private ReturnHandArea returnHandAreaUnderCard;

    void Start()
    {
        //尝试自动获取
        if(card==null) card = GetComponent<Card>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        CheckReturnHandAreaUnderCard();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(returnHandAreaUnderCard != null)
        {
            //如果当前卡牌不在手牌列表中,则添加奥手牌列表,并从其他卡槽中移除
            if(!BattleMessage.instance.GetHandCardList().Contains(card))
            {
                //检测卡槽中是否有当前卡牌
                foreach(CardSlot cardSlot in BattleMessage.instance.GetAllCardSlot())
                {
                    //如果检测到当前卡牌,则从该卡槽中移除
                    if(cardSlot.GetInnerCard() == card) 
                    {
                        cardSlot.SetInnerCard(null);
                        break;
                    }
                }
                BattleMessage.instance.GetHandCardList().Add(card);
            }
            
        }
    }

    public void CheckReturnHandAreaUnderCard()
    {
        //获取卡牌图片下方的物体
        RaycastHit2D[] hit2Ds = Physics2D.RaycastAll(card.transform.position, Vector2.down, 0.1f);
        returnHandAreaUnderCard = null;
        foreach (var hit2D in hit2Ds)
        {
            //判断是否是卡槽对象
            if (hit2D.collider.GetComponent<ReturnHandArea>() != null)
            {
                //检测第一个卡槽
                returnHandAreaUnderCard = hit2D.collider.GetComponent<ReturnHandArea>();
                break;
            }
        }
    }
}