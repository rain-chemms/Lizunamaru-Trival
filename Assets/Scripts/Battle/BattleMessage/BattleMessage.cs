using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMessage : MonoBehaviour
{
    public static BattleMessage instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    //回合控制相关
    [SerializeField] private uint round = 0;//回合数数
    public uint GetRound()
    {
        return round;
    }
    [SerializeField] private bool isPlayerTurn = true;//是否是玩家回合
    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
    [SerializeField] private uint controlPlayerID = 0;//控制的玩家的ID,卡牌触发系统从这个id的玩家中生效
    public uint GetControlPlayerID()
    {
        return controlPlayerID;
    }
    public void SetControlPlayerID(uint id)
    {
        controlPlayerID = id;
    }
    [SerializeField] private List<Role> roleList = new List<Role>();//敌人与玩家对象的列表
    public List<Role> GetRoleList()
    {
        return roleList;
    }

    // 卡牌相关
    //抽牌堆
    [SerializeField] private List<Card> drawCardList = new List<Card>();
    public List<Card> GetDrawCardList()
    {
        return drawCardList;
    }
    //弃牌堆
    [SerializeField] private List<Card> discardCardList = new List<Card>();
    public List<Card> GetDiscardCardList()
    {
        return discardCardList;
    }
    //手牌
    [SerializeField] private uint maxHandCardCount = 10;//最大手牌数
    public uint GetMaxHandCardCount()
    {
        return maxHandCardCount;
    }
    [SerializeField] private List<Card> handCardList = new List<Card>();
    public List<Card> GetHandCardList()
    {
        return handCardList;
    }

    //能量数值相关
    [SerializeField] private uint ricePoint = 0;//行动点数
    public uint GetRicePoint()
    {
        return ricePoint;
    }
    [SerializeField] private uint icePoint = 0;//敌方回合可用行动点数
    public uint GetIcePoint()
    {
        return icePoint;
    }
    [SerializeField] private uint riceChargePreRound = 6;//每回合可以恢复的行动点数
    public uint GetRiceChargePreRound()
    {
        return riceChargePreRound;
    }
    [SerializeField] private uint iceChargePreRound = 3;//每回合可以恢复的敌方回合的行动点数
    public uint GetIceChargePreRound()
    {
        return iceChargePreRound;
    }
    
    [SerializeField] private float spellPrecent = 0.0f;//符卡攻击的充能情况
    public float GetSpellPrecent()
    {
        return spellPrecent;
    }
    //卡槽相关
    [Header("所有的卡槽列表")]
    [SerializeField] private List<CardSlotList> cardSlotListList = new List<CardSlotList>();//所有卡槽列表的管理器
    public List<CardSlotList> GetCardSlotListList()
    {
        return cardSlotListList;
    }
    public CardSlotList GetCardSlotList(CardCategory cardCategory)
    {
        foreach(CardSlotList cardSlotList in cardSlotListList)
        {
            if(cardSlotList.GetSlotListCardCategory() == cardCategory)
            {
                return cardSlotList;
            }
        }
        return null;
    }
    [SerializeField] private CardSlot spellAttackCardSlot;//
    public CardSlot GetSpellAttackCardSlot()
    {
        return spellAttackCardSlot;
    }
    //获取全部卡槽组成的列表
    public List<CardSlot> GetAllCardSlot()
    {
        List<CardSlot> allCardSlotList = new List<CardSlot>();
        foreach(CardSlotList cardSlotList1 in GetCardSlotListList())
        {
            foreach(CardSlot cardSlot in cardSlotList1.GetCardSlotList())
            {
                allCardSlotList.Add(cardSlot);
            }
        }
        allCardSlotList.Add(spellAttackCardSlot);
        return allCardSlotList;
    }

    //每个种类的卡槽列表中卡槽的数量
    //卡槽数量更新时以整个字典为准
    [Header("卡槽列表中卡槽的数量:仅限Power,Attack,Gadget这3种可设置")]
    [SerializeField] private SerializableDictionary<CardCategory,int> cardSlotListCardSlotCount = new SerializableDictionary<CardCategory,int>();
    public SerializableDictionary<CardCategory,int> GetCardSlotListCardSlotCount()
    {
        return cardSlotListCardSlotCount;
    }
    
}
