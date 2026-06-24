using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//挂载在战斗UI显示器上
[RequireComponent(typeof(BattleMessageDisplayer))]
public class BattleCardSlotListController : MonoBehaviour
{   
    [SerializeField] private SerializableDictionary<CardCategory,CardSlot> prefabDict;//卡槽预制体
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeleteAllCardSlotCategoryNotMatch();
        FreshCardSlotListCount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //删除所有卡槽列表中类型不匹配的卡槽
    public void DeleteAllCardSlotCategoryNotMatch()
    {
        List<CardSlotList> cardSlotListList = BattleMessage.instance.GetCardSlotListList();
        foreach(CardSlotList cardSlotList in cardSlotListList)
        {
            List<CardSlot> cardSlotList1 = cardSlotList.GetCardSlotList();
            if(cardSlotList1 == null || cardSlotList1.Count <= 0) continue;
            CardCategory listCategory = cardSlotList.GetSlotListCardCategory();
            foreach(CardSlot cardSlot in cardSlotList1)
            {
                if(cardSlot.GetSlotCardCategory() != listCategory)
                {
                    Debug.Log("[BattleCardSlotListController]: Checked the CardSlot:<"+cardSlot.name+"> is not in the same category as the CardSlotList:<"+cardSlotList.name+">!Have Delete it!");
                    Destroy(cardSlot.gameObject);//删除不匹配的卡槽
                }
            }
        }
    }

    //依据特定的卡槽类型删除所有卡槽列表中类型不匹配的卡槽
    public void DeleteAllCardSlotCategoryNotMatch(CardCategory checkListCategory)
    {
        List<CardSlotList> cardSlotListList = BattleMessage.instance.GetCardSlotListList();
        foreach(CardSlotList cardSlotList in cardSlotListList)
        {
            List<CardSlot> cardSlotList1 = cardSlotList.GetCardSlotList();
            if(cardSlotList1 == null || cardSlotList1.Count <= 0) continue;
            CardCategory nowListCategory = cardSlotList.GetSlotListCardCategory();
            if(nowListCategory != checkListCategory) continue;//列表类别与输入类别不一致时跳过当前的卡槽列表检测
            foreach(CardSlot cardSlot in cardSlotList1)
            {
                if(cardSlot.GetSlotCardCategory() != nowListCategory)
                {
                    Debug.Log("[BattleCardSlotListController]: Checked the CardSlot:<"+cardSlot.name+"> is not in the same category as the CardSlotList:<"+cardSlotList.name+">!Have Delete it!");
                    Destroy(cardSlot.gameObject);//删除不匹配的卡槽
                }
            }
        }
    }

    //依据卡槽列表数量刷新卡槽数量
    public void FreshCardSlotListCount()
    {
        //读取BattleMessage单例中的信息
        SerializableDictionary<CardCategory,int> cardSlotListCardSlotCount = BattleMessage.instance.GetCardSlotListCardSlotCount();
        //获取所有的卡槽列表
        List<CardSlotList> cardSlotListList = BattleMessage.instance.GetCardSlotListList();
        //遍历所有的卡槽列表
        foreach(CardSlotList cardSlotList in cardSlotListList)
        {
            //清空null值卡槽
            RemoveAllNullValueCardSlotInList(cardSlotList);
            //获取当前卡槽的类别
            CardCategory nowCardCategory = cardSlotList.GetSlotListCardCategory();
            //获取字典中当前卡槽类别的卡槽数量
            int cardSlotCountInDict = cardSlotListCardSlotCount[nowCardCategory];
            if(cardSlotListCardSlotCount.ContainsKey(nowCardCategory))
            {
                cardSlotCountInDict = cardSlotListCardSlotCount[nowCardCategory];//获取字典中当前卡槽类别的卡槽数量
            }
            else//字典中无当前卡槽类别
            {
                continue;//不检测,跳过
            }
            //获取当前卡槽列表中与卡槽类别相同的卡槽数量 
            int cardSlotCount = GetCardSlotListCategoryMatchedCardSlotCount(cardSlotList);
            if(cardSlotCount < cardSlotCountInDict)//当前卡槽数量小于字典中当前卡槽类别的卡槽数量
            {
                //对当前进行卡槽补充
                //若当前预制体字典为空,或者当前字典中不存在当前类别的预制体,或者预制体为空,或者u一直提不是CardSlot:进行报错
                if(prefabDict == null)
                {
                    Debug.LogError("[BattleCardSlotListController]: The Prefab Dictionary is null !");
                    continue;//跳过当前循环    
                }
                if(!prefabDict.ContainsKey(nowCardCategory))
                {
                    Debug.LogError("[BattleCardSlotListController]: The Prefab Dictionary NOT Contain the current CardCategory:<"+nowCardCategory.ToString()+"> !");
                    continue;//跳过当前循环                   
                }
                if(prefabDict[nowCardCategory] == null)
                {
                    Debug.LogError("[BattleCardSlotListController]: The Prefab Dictionary contain the current CardCategory:<"+nowCardCategory.ToString()+"> But the Prefab GameObject is null !");
                    continue;//跳过当前循环     
                }
                if(prefabDict[nowCardCategory].GetComponent<CardSlot>() == null)
                {
                    Debug.LogError("[BattleCardSlotListController]: The Prefab Dictionary current CardCategory:<"+nowCardCategory.ToString()+"> Prefab GameObject is not a [CardSlot]!");
                    continue;//跳过当前循环         
                }

                //在卡槽的队尾插入新的卡槽
                int insertCount = cardSlotCountInDict - cardSlotCount;
                for(int i = 0; i < insertCount; i++)//插入的次数
                {
                    //创建卡槽,并将其作为子物体添加到卡槽列表中
                    CardSlot newCardSlot = Instantiate(prefabDict[nowCardCategory],cardSlotList.transform);
                    newCardSlot.SetSlotCardCategory(nowCardCategory);
                    //插入新的卡槽到卡槽列表中
                    cardSlotList.GetCardSlotList().Add(newCardSlot);
                }
            }
            else if(cardSlotCount > cardSlotCountInDict)//当前卡槽数量大于字典中当前卡槽类别的卡槽数量
            {
                //对当前进行卡槽删除
                int deleteCount = cardSlotCount - cardSlotCountInDict;
                int haveDeleteCount = 0;
                foreach(CardSlot cardSlot in cardSlotList.GetCardSlotList())
                {
                    //判断是否还需要删除卡槽
                    if(haveDeleteCount >= deleteCount)
                    {
                        break;
                    }
                    /*
                        一下代码可以抽象为一个具体的方法:DeleteCardSlot(CardSlotList cardSlotList,CardSlot deleteCardSlot)
                    */
                    if(cardSlot == null) continue;
                    if(cardSlot.GetSlotCardCategory() == nowCardCategory)
                    {
                        /*
                            若当前卡槽中存在卡牌
                            则将卡槽中的卡牌移动到手牌中
                        */
                        if(cardSlot.GetInnerCard() != null) BattleMessage.instance.GetHandCardList()?.Add(cardSlot.GetInnerCard());
                        //将当前卡槽从卡槽列表中删除
                        cardSlotList.GetCardSlotList().Remove(cardSlot);
                        //删除游戏物体
                        Destroy(cardSlot.gameObject);
                        haveDeleteCount++;
                    }
                }
            }
            else //当前卡槽数量等于字典中当前卡槽类别的卡槽数量
            {
                //对当前进行卡槽数量不变
                continue;
            } 
        }
    }

    //获取与卡槽列表类型匹配的卡槽数量
    private int GetCardSlotListCategoryMatchedCardSlotCount(CardSlotList cardSlotList)
    {
        int cardSlotCount = 0;
        CardCategory cardCategory = cardSlotList.GetSlotListCardCategory();
        foreach(CardSlot cardSlot in cardSlotList.GetCardSlotList())
        {
            if(cardSlot.GetSlotCardCategory() == cardCategory) cardSlotCount++;
        }
        return cardSlotCount;
    }

    //移除卡槽列表中所有的值为哈希值为null的卡槽,防止占用实际的卡槽数量
    public void RemoveAllNullValueCardSlotInList(CardSlotList cardSlotList)
    {
        if(cardSlotList == null) return;
        foreach(CardSlot cardSlot in cardSlotList.GetCardSlotList())
        {
            //移除值为null的卡槽
            if(cardSlot == null) cardSlotList.GetCardSlotList().Remove(cardSlot);
        }
    }
}
