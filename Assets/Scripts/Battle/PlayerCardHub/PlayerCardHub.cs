using CardSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System;

//代表玩家在游戏中的牌库,战斗开始时会使用牌库对战斗信息进行初始化,要存储对应卡牌的等级
//单例对象
public class PlayerCardHub : MonoBehaviour
{
    [Serializable]
    public struct PlayerCardHubItem
    {
        public Card cardPrefab;
        public uint cardLevel;
        public bool canRUG;
    }

    //卡牌名字与卡牌等级结构体
    public struct CardNameWithLevel
    {
        public string cardName;
        public uint cardLevel;
        public bool canRUG;
    }

    public static PlayerCardHub instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //这个里面全是Prefab的引用,它本身也是从CardPool中获取的
    [SerializeField] private List<PlayerCardHubItem> cardHub = new List<PlayerCardHubItem>();
    public List<PlayerCardHubItem> GetCardHub() => cardHub;
    public List<PlayerCardHubItem> GetCardHub_Copy() => cardHub.ToList();

    //供给外界调用,用于加载游戏的卡牌
    public void InitCardToBattle()
    {
        //清除旧的卡牌信息和卡牌物体
        List<Card> oldCards = new List<Card>();

        List<Card> hand = BattleMessage.instance?.GetHandCardList();
        List<Card> draw = BattleMessage.instance?.GetDrawCardList();
        List<Card> discard = BattleMessage.instance?.GetDiscardCardList();
        List<Card> exhaust = BattleMessage.instance?.GetExhaustCardList();

        if (hand != null) oldCards.AddRange(hand);
        if (draw != null) oldCards.AddRange(draw);
        if (discard != null) oldCards.AddRange(discard);
        if (exhaust != null) oldCards.AddRange(exhaust);
        Debug.Log("[PlayerCardHub]: OldCards Number:" + oldCards.Count);
        //清空对应的列表
        hand?.Clear();
        draw?.Clear();
        discard?.Clear();
        exhaust?.Clear();
        //清除游戏物体
        foreach(Card c in oldCards?.ToList())
        {
            if(c == null) continue;
            Destroy(c.gameObject);
        }
        oldCards.Clear();

        //依据玩家牌库产生新的游戏物体到抽牌堆
        foreach(PlayerCardHubItem pchi in cardHub?.ToList())
        {
            Card c = pchi.cardPrefab;
            uint level = pchi.cardLevel;
            bool rUG = pchi.canRUG;
            if(c == null) continue;
            Card newCard = Instantiate(c,BattleMessageDisplayer.instance?.transform);//设置父物体为战斗信息显示器
            newCard.SetCardUpgradeLevel(level);//设置卡牌等级
            newCard.SetRepeatUpgrade(rUG);
            draw?.Add(newCard);
        }
        //对抽牌堆进行洗牌
        BattleMessage.instance?.ShuffleCardList(draw);
    }

    //获取所有玩家牌库中卡牌的完整命名,便于SL复原当前游戏进度
    public List<CardNameWithLevel> GetFullNameListOfCardHub()
    {
        List<CardNameWithLevel> result = new List<CardNameWithLevel>();
        foreach(PlayerCardHubItem pchi in cardHub?.ToList())
        {
            Card c = pchi.cardPrefab;
            uint level = pchi.cardLevel;
            bool rUG = pchi.canRUG;
            if(c == null) continue;
            Type cType = c.GetType();
            string cKey = cType.FullName;
            CardNameWithLevel item = new CardNameWithLevel();
            item.cardName = cKey;
            item.cardLevel = level;
            item.canRUG = rUG;
            result.Add(item);//牌库里可以有重复的牌
        }
        return result;
    }

    ///*测试代码
    void Start()
    {
        InitCardToBattle();
    }
    //*/
}
