using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class CardPlayArea : MonoBehaviour
{
    [SerializeField] private Queue<Card> willPlayCardQueue = new Queue<Card>();
    [SerializeField] private bool isExecuting = false;//当前是否正在执行
    //添加要打出的卡牌
    public void AddCard(Card card)
    {
        willPlayCardQueue.Enqueue(card);//添加卡牌
    }

    // Update is called once per frame
    void Update()
    {
        if(!isExecuting && willPlayCardQueue.Count > 0)
        {
            StartCoroutine(PlayTheCardInQueue());
        }
    }

    //检查并打出列表顶部的牌
    private IEnumerator PlayTheCardInQueue()
    {
        isExecuting = true;
        while(willPlayCardQueue.Count > 0)
        {
            Card card = willPlayCardQueue.Dequeue();
            yield return BattleMessage.instance?.PlayCard(card,true);
            /*
            //打出卡牌
            if(card?.GetRiceCost() >= BattleMessage.instance?.GetRicePoint())//能量足够可以打出
            {
                BattleMessage.instance?.SetRicePoint((uint)(BattleMessage.instance?.GetRicePoint() - card?.GetRiceCost()));
                yield return ((CardFunctioner)card).AfterPlay();//AfterPlay函数会对消耗字段进行检测,若存在消耗字段则触发消耗的连锁函数
                //将卡牌返回弃牌堆
                //若当前卡牌由消耗关键字,则不将其加入弃牌堆
                if(card!=null && !(bool)card.GetCardKeyWords()?.Contains(CardKeyWord.EXHAUST)) BattleMessage.instance?.GetDiscardCardList()?.Add(card);
            }
            else//将这张牌返回手中
            {
                if(card!=null)
                {
                    BattleMessage.instance?.GetHandCardList().Add(card);
                }
            }
            */
        }
        isExecuting = false;
    }
}
