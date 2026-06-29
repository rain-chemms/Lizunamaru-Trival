using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card))]
public class CardPlayAreaChecker : MonoBehaviour,
    IEndDragHandler, IDragHandler
{
    [SerializeField] private Card card;
    [SerializeField] private CardPlayArea cardPlayAreaUnderCard;//卡牌打出区域
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if (card == null) card = GetComponent<Card>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        CheckCardPlayAreaUnderCard();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cardPlayAreaUnderCard != null)
        {
            //如果当前卡牌不在手牌列表中,则不处理
            if (!BattleMessage.instance.GetHandCardList().Contains(card)) return;
            //将当前卡牌加入卡牌打出区域的卡牌列表中,并从手牌中移除
            cardPlayAreaUnderCard.AddCard(card);
            BattleMessage.instance?.GetHandCardList()?.Remove(card);
        }
    }

    public void CheckCardPlayAreaUnderCard()
    {
        //获取卡牌图片下方的物体
        RaycastHit2D[] hit2Ds = Physics2D.RaycastAll(card.transform.position, Vector2.down, 0.1f);
        cardPlayAreaUnderCard = null;
        foreach (var hit2D in hit2Ds)
        {    
            //判断是否是卡槽对象
            if (hit2D.collider.GetComponent<CardPlayArea>() != null)
            {        
                //检测第一个卡槽
                cardPlayAreaUnderCard = hit2D.collider.GetComponent<CardPlayArea>();
                break;
            }
        }
    }
}
