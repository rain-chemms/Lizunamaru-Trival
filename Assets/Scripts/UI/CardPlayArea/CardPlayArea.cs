using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardSystem;

[RequireComponent(typeof(RectTransform))]
public class CardPlayArea : MonoBehaviour
{
    [SerializeField] private Queue<Card> willPlayCardQueue = new Queue<Card>();
    [SerializeField] private bool isExecuting = false;//当前是否正在执行
    public bool IsExecuting() => isExecuting;

    [SerializeField] private int sortOrder = 0;
    public void SetSortOrder(int layer) => sortOrder = layer;
    public int GetSortOrder() => sortOrder;

    //添加要打出的卡牌
    public void AddCard(Card card)
    {
        //依据当前队列的索引设置卡牌的sortLayer
        Canvas cvs = card?.GetComponent<Canvas>();
        if(cvs != null) 
        {
            cvs.overrideSorting = true;
            cvs.sortingOrder = sortOrder + willPlayCardQueue.Count;
        }
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
        }
        isExecuting = false;
    }
}
