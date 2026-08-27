using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using CardSystem;
using System.Linq;

//已有卡牌选择执行器,类似HandCardOperator,但是选择的是已经存在的卡牌
[RequireComponent(typeof(Canvas))]
public class CardSelectOperator : MonoBehaviour
{
    public static CardSelectOperator instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //卡牌存储相关
    //卡牌列表
    //要临时修改当前卡牌的位置,其中记录者原来的父物体的
    [SerializeField] private List<(Card,Transform)> willSelectCardList = new List<(Card,Transform)>();//在原始的列表中存储的卡牌不动
    /*
        比如所: 在BattleMessage的drawCardList中选择时,将相应的Card的引用加入willSelectCardList中,原来的引用位置不变
        结束选择时,通过operateFunc对原来的drawCardList进行修改并对卡牌加入其他牌堆中进行操作
    */
    public List<(Card,Transform)> GetWillSelectCardList() => willSelectCardList;
    public List<(Card,Transform)> GetWillSelectCardList_Copy() => willSelectCardList.ToList();
    /// <summary>
    /// 添加一个(卡牌,父物体元组)
    /// </summary>
    /// <param name="card">"(Card,Transform)"代表卡牌物体与其当前父物体的元组,必须一对一对的</param>
    public void AddCard((Card,Transform) card)
    {
        if(willSelectCardList != null)
        {
            if(!willSelectCardList.Contains(card)) willSelectCardList.Add(card);
        }
    }
    /// <summary>
    /// 移除一个(卡牌,父物体元组)
    /// </summary>
    /// <param name="card">"(Card,Transform)"代表卡牌物体与其当前父物体的元组,必须一对一对的</param>
    public void RemoveCard((Card,Transform) card)
    {
        if(willSelectCardList != null)
        {
            if(willSelectCardList.Contains(card)) willSelectCardList.Remove(card);
        }
    }

    //操作函数设置
    [SerializeField] public Func<Card,IEnumerator> operateFunc;//操作函数,每次调用时必须传入当前需要对卡牌进新的操作,返回协程对象并传入Card
    public void SetOperateFunc(Func<Card,IEnumerator> operateFunc) => this.operateFunc = operateFunc;
    
    //执行数量
    [SerializeField] private uint operateCount;
    public uint GetOperateCount() => operateCount;
    public void SetOperateCount(uint count) => operateCount = count;
    //执行类别控制
    [SerializeField] private CardOperateCategory operateCategory;
    public CardOperateCategory GetOperateCategory() => operateCategory;
    public void SetOperateCategory(CardOperateCategory operateCategory) => this.operateCategory = operateCategory;
    //状态标识符
    [SerializeField] private bool selectOver = false;//是否已结束选择
    public void SetSelectOver(bool isOver) => selectOver = isOver;

    //willSelectList可以在外部设置也可以直接传入
    //直接传入时,会覆盖外界已经加入的选择的CardList
    public IEnumerator CallTheCardSelectOperator(uint operateCount,CardOperateCategory operateCategory,List<(Card,Transform)> willSelectList = null)//激活选择器
    {
        SetOperateCount(operateCount);//设置执行数量
        SetOperateCategory(operateCategory);//设置执行类别
        if(willSelectList != null)//传入的列表不为空时,将传入的列表加入willSelectList
        {
            willSelectCardList.Clear();//清空已有的卡牌列表
            foreach ((Card card,Transform parent) cp in willSelectList)
            {
                if(cp.card == null) continue;//忽略空引用
                AddCard(cp);//将卡牌加入列表
            }
        }
        //尝试获取所有的StackCardAnchorSetter和所有StackCardSortOrderSetter组件,临时关闭它们
        List<StackCardAnchorSetter> stackCardAnchorSetters = UnityEngine.Object.FindObjectsByType<StackCardAnchorSetter>(FindObjectsSortMode.None).ToList();
        List<StackCardSortOrderSetter> stackCardSortOrderSetters = UnityEngine.Object.FindObjectsByType<StackCardSortOrderSetter>(FindObjectsSortMode.None).ToList();
        foreach (StackCardAnchorSetter c in stackCardAnchorSetters) c.enabled = false;
        foreach (StackCardSortOrderSetter c in stackCardSortOrderSetters) c.enabled = false;

        //开启选择器UI显示
        CardSelectOperatorOpenController coCtr = GetComponent<CardSelectOperatorOpenController>();
        coCtr.SetOpen(true);
        
        //开启所有willSelectCardList中的卡牌的CardCardSelectOperatorController脚本
        //关闭打出控制脚本
        foreach ((Card card,Transform parent) in willSelectCardList)
        {
            ///开启控制器
            if(card == null) continue;
            CardCardSelectOperatorController ctr = card.GetComponent<CardCardSelectOperatorController>();
            if(ctr != null) 
            {    
                ctr.SetSelected(false);
                ctr.enabled = true;
            }
            //关闭打出区域检测器
            CardPlayAreaChecker cpaChecker = card.GetComponent<CardPlayAreaChecker>();
            if(cpaChecker != null) cpaChecker.enabled = false;
            //关闭卡槽插入检测器
            CardInsertSlotChecker cslChecker = card.GetComponent<CardInsertSlotChecker>();
            if(cslChecker != null) cslChecker.enabled = false;
            //关闭CardInStackChecker
            CardInStackChecker cisChecker = card.GetComponent<CardInStackChecker>();
            if(cisChecker != null) cisChecker.enabled = false;
            //将CardDisplayer的InStack设置为false
            CardDisplayer cd = card.GetComponent<CardDisplayer>();
            if(cd != null) cd.SetInStack(false);
        }

        //获取有效的卡牌数量
        int cardCount = willSelectCardList.ToList().Count;
        //判断卡牌数量是否充足,处于At_Most条件下可以不做选择因此一定要打开
        //EQUAL不能多也不能少,但如果卡牌数量不够,那EQUAL按照AT_LEAST处理
        if(cardCount > operateCount  || operateCategory == CardOperateCategory.AT_MOST || operateCategory ==  CardOperateCategory.NOT_ABOVE) yield return new WaitUntil(() => selectOver);//充足时等待玩家选择结束
        else
        {
            //牌量不够时的逻辑:将所有牌标记为已经选择
            foreach ((Card card,Transform parent) in willSelectCardList)
            {
                if(card == null) continue;
                CardCardSelectOperatorController ctr = card.GetComponent<CardCardSelectOperatorController>();
                if(ctr != null) ctr.SetSelected(true);
            }
        }
        
        selectOver = false;//重置选择结束状态
        //关闭所有卡牌的CardCardSelectOperatorController脚本
        //重新打开控制脚本
        foreach ((Card card,Transform parent) in willSelectCardList)
        {
            if(card == null) continue;
            CardCardSelectOperatorController ctr = card.GetComponent<CardCardSelectOperatorController>();
            if(ctr != null) 
            {
                ctr.enabled = false;
            }
            //重新开启打出区域检测器
            CardPlayAreaChecker cpaChecker = card.GetComponent<CardPlayAreaChecker>();
            if(cpaChecker != null) cpaChecker.enabled = true;
            //重新开启卡槽插入检测器
            CardInsertSlotChecker cslChecker = card.GetComponent<CardInsertSlotChecker>();
            if(cslChecker != null) cslChecker.enabled = true;
            //打开CardInStackChecker
            CardInStackChecker cisChecker = card.GetComponent<CardInStackChecker>();
            if(cisChecker != null) cisChecker.enabled = true;
        }
        //清空已选择的卡牌列表
        //防止UI显示错误,将已选择的卡牌放回原来的位置
        //保留要执行的卡牌
        foreach ((Card card,Transform parent) cp in willSelectCardList.ToList())
        {
            Card card = cp.card;
            Transform parent = cp.parent;
            CardCardSelectOperatorController ctr = card?.GetComponent<CardCardSelectOperatorController>();
            //被选择的卡牌不去放回原来的父物体,放回操作由operateFunc执行
            if((bool)ctr?.IsSelected()) {continue;}
            willSelectCardList.Remove(cp);
            card.transform.SetParent(parent);//放回原UI父物体位置
        }
        //相关的卡牌数据存储移动操作在外部进行
        yield return OperateFuncToSelectedCards();
        //移除剩余的卡牌
        willSelectCardList.Clear();
        //重新开启所有StackCardAnchorSetter和所有StackCardSortOrderSetter组件
        foreach (StackCardAnchorSetter c in stackCardAnchorSetters) c.enabled = true;
        foreach (StackCardSortOrderSetter c in stackCardSortOrderSetters) c.enabled = true;
        //关闭选择器UI显示
        coCtr.SetOpen(false);
    }

    //功能函数
    private IEnumerator OperateFuncToSelectedCards()
    {
        foreach ((Card card,Transform parent) cp in willSelectCardList)
        {
            Card card = cp.card;
            if(card == null) continue;
            //若当前卡牌处于已经选择的状态则执行相关的操作
            CardCardSelectOperatorController ctr = card?.GetComponent<CardCardSelectOperatorController>();
            //当前卡牌处于选择状态
            if((bool)ctr?.IsSelected()) yield return operateFunc(card);
        }
    }
}
