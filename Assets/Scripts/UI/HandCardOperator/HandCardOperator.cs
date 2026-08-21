using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using CardSystem;

//手牌操作器,使用一些卡牌进行触发对手牌的操作
//是单例对象
[RequireComponent(typeof(Canvas))]
public class HandCardOperator : MonoBehaviour
{
    public static HandCardOperator instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] public Func<Card,IEnumerator> operateFunc;//操作函数,每次调用时必须传入当前需要对卡牌进新的操作,返回协程对象并传入Card
    public void SetOperateFunc(Func<Card,IEnumerator> operateFunc) => this.operateFunc = operateFunc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<Card> selectedCards;//当前选中的卡牌
    public List<Card> GetSelectedCards() => selectedCards;
    public List<Card> GetSelectedCards_Copy() => selectedCards.ToList();
    
    [SerializeField] private uint operateCount;
    public uint GetOperateCount() => operateCount;
    public void SetOperateCount(uint count) => operateCount = count;
    [SerializeField] private CardOperateCategory operateCategory;
    public CardOperateCategory GetOperateCategory() => operateCategory;
    public void SetOperateCategory(CardOperateCategory operateCategory) => this.operateCategory = operateCategory;

    //这个功能器直接操作相应的卡牌实体
    //会对BattleMessage中的卡牌列表产生影响
    public bool IsCardSelected(Card card)
    {
        return selectedCards.Contains(card);
    }

    public void AddCard(Card card)
    {
        if(selectedCards == null) return;
        if(!selectedCards.Contains(card))//不包含当前卡牌
        {
            selectedCards.Add(card);
        }    
        //将卡牌从手牌中移除
        BattleMessage.instance?.GetHandCardList()?.Remove(card);
    }

    public void RemoveCard(Card card)
    {
        if(selectedCards == null) return;
        //移除卡牌
        if(selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
        }
        //将卡牌重新返回手牌中
        StartCoroutine(BattleMessage.instance?.AddExistCardToHand(card));
    }

    [SerializeField] private bool selectOver = false;//是否已结束选择
    public void SetSelectOver(bool isOver) => selectOver = isOver;

    public IEnumerator CallTheHandCardOperator(uint operateCount,CardOperateCategory operateCategory)
    {
        SetOperateCount(operateCount);
        SetOperateCategory(operateCategory);
        HandCardOperatorOpenController hoCtr = GetComponent<HandCardOperatorOpenController>();
        hoCtr?.SetOpen(true);
        //激活所有手牌的执行器
        List<Card> handCards = BattleMessage.instance.GetHandCardList_Copy();
        int cardCount = handCards.Where(x => x!= null).ToList().Count;//获取非空的手牌数量
        foreach(Card card in handCards)
        {
            if(card == null) continue;
            CardHandCardOperatorController ctr = card.GetComponent<CardHandCardOperatorController>();
            if(ctr!=null) ctr.enabled = true;
        }
        //牌量足够时等待选择结束
        if(cardCount > operateCount) yield return new WaitUntil(() => selectOver);
        else
        {
            //牌量不够时将剩余的卡牌直接加入选择器中
            foreach (Card card in handCards)
            {
                if(card == null) continue;
                if (!IsCardSelected(card)) AddCard(card);
            }
        }
        selectOver = false;//重置选择结束状态
        //关闭卡牌选择器
        foreach(Card card in handCards)
        {
            if(card == null) continue;
            CardHandCardOperatorController ctr = card.GetComponent<CardHandCardOperatorController>();
            if(ctr!=null) ctr.enabled = false;
        }

        yield return OperateFuncToSelectedCards();
        //执行后清空已选择的卡牌列表
        selectedCards.Clear();
        //关闭选择器
        hoCtr?.SetOpen(false);
    }

    //功能函数
    private IEnumerator OperateFuncToSelectedCards()
    {
        foreach (Card card in selectedCards)
        {
            if(card == null) continue;
            yield return operateFunc(card);
        }
    }

}
