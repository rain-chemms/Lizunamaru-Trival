using CardSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//代表玩家在游戏中的牌库,战斗开始时会使用牌库对战斗信息进行初始化
//单例对象
public class PlayerCardHub : MonoBehaviour
{
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
    [SerializeField] private List<Card> cardHub = new List<Card>();
    public List<Card> GetCardHub() => cardHub;
    public List<Card> GetCardHub_Copy() => cardHub.ToList();

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
        foreach(Card c in cardHub?.ToList())
        {
            if(c == null) continue;
            Card newCard = Instantiate(c,BattleMessageDisplayer.instance?.transform);//设置父物体为战斗信息显示器
            draw?.Add(newCard);
        }
        //对抽牌堆进行洗牌
        BattleMessage.instance?.ShuffleCardList(draw);
    }

    /*测试代码
    void Start()
    {
        InitCardToBattle();
    }
    //*/
}
