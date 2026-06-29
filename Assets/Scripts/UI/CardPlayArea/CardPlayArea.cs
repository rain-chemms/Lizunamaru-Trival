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
            //打出卡牌
            yield return ((CardFunctioner)card).AfterPlay();
            //将卡牌返回弃牌堆
            BattleMessage.instance?.GetDiscardCardList()?.Add(card);
        }
        isExecuting = false;
    }
}
