using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleMessage : MonoBehaviour
{
    public static BattleMessage instance;
    private void Awake()
    {
        if (instance == null)
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
    /*
        扩展方法1:查找某一特定阵营特定ID的角色
            一般来说ID都是唯一的,但是分阵营
            当前情况下:True阵营的ID = 1的角色一般 不与 False阵营ID = 1的角色冲突,因为可以使用阵营区别两者
    */
    
    public Role GetRole(uint id,bool side)
    {
        if(roleList == null) return null;
        foreach(Role role in roleList)
        {
            if(role.GetID() == id && role.GetSide() == side)
            {
                return role;
            }
        }
        return null;
    }
    /*
        扩展方法2:依据当前存储的玩家信息,获取目前正在控制的玩家
            依赖扩展方法1
    */
    public Role GetControlPlayer()//默认获取True阵营的玩家
    {
        return GetRole(controlPlayerID,isPlayerTurn);
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
    //没有本场战斗消耗的牌堆,个人感觉对于这个项目来说用处不大,加了之后就太像杀戮尖塔了
    //卡牌相关的方法
    //可以将卡牌相关的指令包装为一个Cmd类
    /*
        1.弃掉一张牌
        弃牌时会检测是否有奇巧SLY关键字,若有则将卡牌打出
    */
    public IEnumerator DiscardCard(Card card)
    {
        if (instance.discardCardList.Contains(card) || instance.drawCardList.Contains(card))
        {
            Debug.LogError("[BattleMessage]: The Card<" + card.name + "> is In Draw OR Discard List, Can't Discard!");
            yield return null;
        }
        if (handCardList.Contains(card)) handCardList.Remove(card);
        foreach (CardSlot cl in GetAllCardSlot())
        {
            if (cl == null) continue;
            if (cl.GetInnerCard() == card) cl.SetInnerCard(null);//移除卡槽中的卡牌
        }
        yield return ((CardFunctioner)card).AfterDsicard();//触发卡牌丢弃时的效果
        discardCardList.Add(card);
    }
    /*
        2.将弃牌堆的卡牌转移到抽牌堆
    */
    public IEnumerator ReturnDiscardCardToDrawList()
    {
        //检查弃牌堆
        if (discardCardList == null)
        {
            Debug.LogError("[BattleMessage]: Discard Card List is Null, Please Check!");
            yield return null;
        }
        //检查抽牌堆
        if (drawCardList == null)
        {
            Debug.LogError("[BattleMessage]: Draw Card List is Null, Please Check!");
            yield return null;
        }
        //先将弃牌堆洗牌
        //这里随机化弃牌堆牌的顺序
        ShuffleCardList(discardCardList);
        //加入抽牌堆
        foreach (Card card in discardCardList.ToList())
        {
            drawCardList.Add(card);
            discardCardList.Remove(card);
            yield return 0.2f;//此处可以加入时间停顿产生逐渐回到手牌的现象
        }
    }
    /*
        3.抽一定数量的卡牌
    */
    public IEnumerator DrawCard(int count)
    {
        if (drawCardList == null)
        {
            Debug.LogError("[BattleMessage]: Draw Card List is Null, Please Check!");
            yield return null;
        }
        if (handCardList == null)
        {
            Debug.LogError("[BattleMessage]: Hand Card List is Null, Please Check!");
            yield return null;
        }
        if (handCardList == null)
        {
            Debug.LogError("[BattleMessage]: Hand Card List is Full, Please Check!");
            yield return null;
        }
        //一张一张的抽牌
        for (int i = 0; i < count; i++)
        {
            //先判断:
            //抽牌堆没有卡牌时,将弃牌堆的卡牌转移到抽牌堆中,再进行抽牌
            if (drawCardList.Count <= 0)
            {
                yield return ReturnDiscardCardToDrawList();
                //如果还是无卡牌,则停止抽牌
                if (drawCardList.Count <= 0) break;
            }
            //后判断:
            if (handCardList.Count >= maxHandCardCount) break;//手牌已满则停止抽牌
            Card nowCard = drawCardList[0];
            drawCardList.Remove(nowCard);
            handCardList.Add(nowCard);
            yield return ((CardFunctioner)nowCard).AfterDraw();//触发抽到卡牌后的效果
        }
    }
    /*
        4.使用种子对卡牌列表进行洗牌:
            使用Fisher-Yates洗牌算法
    */
    private void ShuffleCardList(List<Card> cardList)
    {
        if (cardList == null) return;
        if (SeedSetter.instance == null) return;
        //获取随机种子
        int seed = SeedSetter.instance.GetSeed_Int();
        //生成随机数生成器
        System.Random rng = new System.Random(seed);
        int n = cardList.Count;
        // Fisher-Yates 洗牌算法
        while (n > 1)
        {
            n--;
            // 注意：rng.Next(n + 1) 的范围是 [0, n]，必须包含 n
            int k = rng.Next(n + 1);
            // 交换元素
            (cardList[k], cardList[n]) = (cardList[n], cardList[k]);
        }
    }
    /*
        5.生成一张牌并加入手中
    */
    public IEnumerator GenerateCardAndAddToHand(Card cardTemplate)
    {
        if (handCardList == null)
        {
            Debug.LogError("[BattleMessage]: Hand Card List is Full, Please Check!");
            yield return null;
        }
        if (handCardList.Count < maxHandCardCount)
        {
            //按照卡牌模板产生一张新卡
            Card newCard = Instantiate(cardTemplate);
            //将新的卡牌加入手牌中
            handCardList.Add(newCard);
        }
        else yield return GenerateCardAndAddToDiscardList(cardTemplate);//爆牌时加入弃牌堆
    }
    /*
        6.生成一张牌并随机加入抽牌堆中
    */
    public IEnumerator GenerateCardAndAddToDrawList(Card cardTemplate)
    {
        if (drawCardList == null)
        {
            Debug.LogError("[BattleMessage]: Draw Card List is Null, Please Check!");
            yield return null;
        }
        Card newCard = Instantiate(cardTemplate);//按照卡牌模板产生一张新卡
        //将新的卡牌加入抽牌堆
        drawCardList.Add(newCard);//添加到抽牌
        ShuffleCardList(drawCardList);//随机化抽牌堆的顺序
    }
    /*
        7.生成一张牌并随机加入弃牌堆中
    */
    public IEnumerator GenerateCardAndAddToDiscardList(Card cardTemplate)
    {
        if (discardCardList == null)
        {
            Debug.LogError("[BattleMessage]: Discard Card List is Null, Please Check!");
            yield return null;
        }
        Card newCard = Instantiate(cardTemplate);//按照卡牌模板产生一张新卡
        //将新的卡牌加入弃牌堆后喜爱
        discardCardList.Add(newCard);//添加到弃牌堆中
        ShuffleCardList(discardCardList);//随机化弃牌堆的顺序   
    }
    /*
        8.消耗一张卡
    */
    public IEnumerator ExhaustCard(Card card)//消耗一张卡
    {
        //当有消耗词条是将触发卡牌的
        Animator animator = card?.GetComponent<Animator>();
        //在指定当前卡牌的排序位置
        Canvas canvas = card?.GetComponentInParent<Canvas>();
        if (canvas != null) canvas.sortingOrder = 100 + canvas.sortingOrder;
        BattleMessage.instance?.GetAllCardSlot()?.ForEach(cardSlot =>
        {
            if (cardSlot?.GetInnerCard() == this)
            {
                cardSlot?.SetInnerCard(null);
            }
        });
        //卡牌列表检测
        if (BattleMessage.instance?.GetHandCardList()?.Contains(card) == true)
        {
            BattleMessage.instance?.GetHandCardList()?.Remove(card);
        }
        if (BattleMessage.instance?.GetDrawCardList()?.Contains(card) == true)
        {
            BattleMessage.instance?.GetDrawCardList()?.Remove(card);
        }
        if (BattleMessage.instance?.GetDiscardCardList()?.Contains(card) == true)
        {
            BattleMessage.instance?.GetDiscardCardList()?.Remove(card);
        }
        animator?.SetTrigger("Exhaust");//触发消耗动画,由消耗动画触发消耗后的效果
        yield return card.AfterExhaust();//触发消耗效果
                                         //获取消耗动画的时长
        AnimationClip[] clips = animator?.runtimeAnimatorController.animationClips;
        float haltTime = 0;
        foreach (AnimationClip clip in clips)
        {
            if (clip?.name == "Exhaust")
            {
                haltTime = (float)clip?.length;
                break;
            }
        }
        //等待消耗动画结束
        yield return haltTime;
    }

    //能量数值相关
    [SerializeField] private uint ricePoint = 0;//行动点数
    public uint GetRicePoint()
    {
        return ricePoint;
    }
    public void SetRicePoint(uint newRicePoint)
    {
        ricePoint = newRicePoint;
    }
    [SerializeField] private uint icePoint = 0;//敌方回合可用行动点数
    public uint GetIcePoint()
    {
        return icePoint;
    }
    public void SetIcePoint(uint newIcePoint)
    {
        icePoint = newIcePoint;
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
        foreach (CardSlotList cardSlotList in cardSlotListList)
        {
            if (cardSlotList.GetSlotListCardCategory() == cardCategory)
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
        foreach (CardSlotList cardSlotList1 in GetCardSlotListList())
        {
            foreach (CardSlot cardSlot in cardSlotList1.GetCardSlotList())
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
    [SerializeField] private SerializableDictionary<CardCategory, int> cardSlotListCardSlotCount = new SerializableDictionary<CardCategory, int>();
    public SerializableDictionary<CardCategory, int> GetCardSlotListCardSlotCount()
    {
        return cardSlotListCardSlotCount;
    }

}
