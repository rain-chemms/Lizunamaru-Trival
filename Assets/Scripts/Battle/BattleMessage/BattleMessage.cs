using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    [SerializeField] private string roundChangeLocalizeTable = "RoundChangeText";
    [SerializeField] private string selfTurnTextKey = "RoundChange_SelfTurn";
    [SerializeField] private string enemyTurnTextKey = "RoundChange_EnermyTurn";
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
    public void SetIsPlayerTurn(bool isPlayerTurn)
    {
        this.isPlayerTurn = isPlayerTurn;
    }
    /*
        扩展方法1:依据当前的回合进行回合切换
    */
    public IEnumerator ChangeTurn()
    {
        //丢弃所有手牌到弃牌堆
        foreach (Card card in handCardList.ToList())
        {
            handCardList.Remove(card);
            //卡槽中的卡不动
            discardCardList.Add(card);
        }
        isPlayerTurn = !isPlayerTurn;//切换回合
        //并将所有当前回合角色的回合操作设置为非结束
        foreach (Role role in roleList)
        {
            if (role?.GetSide() == isPlayerTurn)
            {
                role.SetRoundOperateEnd(false);
            }
        }
        //设置播放本地化文字
        //寻找本地化键值对
        
        //在回合切换之前获取控制的玩家角色的所属阵营
        //bool showTurn = true;
        Role controlPlayer = GetControlPlayer();        
        string key = "";
        //设置键值
        if (controlPlayer?.GetSide() == isPlayerTurn)
        {
            key = selfTurnTextKey;
        }
        else
        {
            key = enemyTurnTextKey;
        }
        string resultText = "Bugs Turn";
        // 等待本地化系统初始化完成
        yield return LocalizationSettings.InitializationOperation;
        // 异步获取
        var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(roundChangeLocalizeTable, key);
        yield return operation;
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            string localizedText = operation.Result;
            resultText = localizedText;//显示文本
        }
        resultText += round.ToString();
        RoundChangeDisplayer.instance.SetDisplayText(resultText);
        //播放回合切换效果
        Animator animator = RoundChangeDisplayer.instance?.GetAnimator();
        animator?.SetTrigger("ChangeTurn");
        AnimatorStateInfo info = (AnimatorStateInfo)animator?.GetCurrentAnimatorStateInfo(0);
        //等待动画器结束
        yield return info.length * info.speed;//显示回合切换效果
        //检测控制的角色的回合状态
        controlPlayer = GetControlPlayer();
        if (controlPlayer?.GetSide() == isPlayerTurn)
        {
            int drawCardCount = drawCardPreRound/*要加一些其他的东西影响抽牌数*/;
            yield return DrawCard(drawCardCount);//抽5张牌
            ricePoint += riceChargePreRound;
            icePoint = 0;
        }
        else
        {
            icePoint += iceChargePreRound + ricePoint;//将剩余的ricePoint变为icePoint
            ricePoint = 0;
        }
        //增加回合数
        round++;
    }

    //敌人玩家角色控制相关
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
    public List<Role> GetRoleList_Copy()
    {
        return roleList.ToList();
    }
    /*
        扩展方法1:查找某一特定阵营特定ID的角色
            一般来说ID都是唯一的,但是分阵营
            当前情况下:True阵营的ID = 1的角色一般 不与 False阵营ID = 1的角色冲突,因为可以使用阵营区别两者
    */

    public Role GetRole(uint id, bool side)
    {
        if (roleList == null) return null;
        foreach (Role role in roleList)
        {
            if (role.GetID() == id && role.GetSide() == side)
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
        return GetRole(controlPlayerID, isPlayerTurn);
    }
    /*
        角色扩展功能
        1.获取防御
    */
    // 卡牌相关
    //抽牌堆
    [SerializeField] private List<Card> drawCardList = new List<Card>();
    public List<Card> GetDrawCardList()
    {
        return drawCardList;
    }
    public List<Card> GetDrawCardList_Copy()
    {
        return drawCardList.ToList();
    }
    //弃牌堆
    [SerializeField] private List<Card> discardCardList = new List<Card>();
    public List<Card> GetDiscardCardList()
    {
        return discardCardList;
    }
    public List<Card> GetDiscardCardList_Copy()
    {
        return discardCardList.ToList();
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
    public List<Card> GetHandCardList_Copy()
    {
        return handCardList.ToList();
    }
    [SerializeField] private int drawCardPreRound = 5;//每回合抽牌数
    public int GetDrawCardPreRound()
    {
        return drawCardPreRound;
    }
    public void SetDrawCardPreRound(int count)
    {
        drawCardPreRound = count;
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
            yield break;
        }
        if (handCardList.Contains(card)) handCardList.Remove(card);
        foreach (CardSlot cl in GetAllCardSlot())
        {
            if (cl == null) continue;
            if (cl.GetInnerCard() == card) cl.SetInnerCard(null);//移除卡槽中的卡牌
        }
        yield return ((CardFunctioner)card).AfterDiscard();//触发卡牌丢弃时的效果
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
        //将这张卡从牌堆中移除
        if (drawCardList.Contains(card)) drawCardList.Remove(card);
        if (handCardList.Contains(card)) handCardList.Remove(card);
        if (discardCardList.Contains(card)) discardCardList.Remove(card);
        //尝试播放卡片的消耗音效
        card.GetComponent<CardVoiceController>()?.PlayCardVoice("Exhaust");
        //等待消耗动画结束
        yield return haltTime;
    }

    /*
        9.由玩家位置处角色产生一颗子弹(ConcentratePoint版)
    */
    /*
        10.打出一张卡牌
    */
    public IEnumerator PlayCard(Card card, bool costRice = true)
    {
        //打出卡牌
        if (card == null) yield break;
        if (card?.GetRiceCost() <= instance?.GetRicePoint() && costRice)//能量足够且耗能的情况下可以打出
        {
            instance?.SetRicePoint((uint)(instance?.GetRicePoint() - card?.GetRiceCost()));
            yield return ((CardFunctioner)card).AfterPlay();//AfterPlay函数会对消耗字段进行检测,若存在消耗字段则触发消耗的连锁函数
            //将卡牌返回弃牌堆
            //若当前卡牌有消耗关键字,则不将其加入弃牌堆
            if (card != null && !(bool)card.GetCardKeyWords()?.Contains(CardKeyWord.EXHAUST)) instance?.GetDiscardCardList()?.Add(card);
        }
        else if (!costRice)
        {
            //不对费用进行消耗
            yield return ((CardFunctioner)card).AfterPlay();//AfterPlay函数会对消耗字段进行检测,若存在消耗字段则触发消耗的连锁函数
            //将卡牌返回弃牌堆
            //若当前卡牌有消耗关键字,则不将其加入弃牌堆
            if (card != null && !(bool)card.GetCardKeyWords()?.Contains(CardKeyWord.EXHAUST)) instance?.GetDiscardCardList()?.Add(card);
        }
        else//将这张牌返回手中
        {
            if (card != null)
            {
                instance?.GetHandCardList().Add(card);
            }
            yield return null;
        }
    }
    //用于直线子弹,会依据玩家的位置和目标坐标的位置产生子弹
    public IEnumerator GenerateBullet(Role role, Bullet bulletPrefab, Vector2Int targetIndex, Vector3 posOffset = default, bool triggerAnim = true, string roleAnimName = "Skill")//角色产生一颗子弹,posOffset为这颗子弹的微小位置偏移
    {
        if (role == null || bulletPrefab == null)
        {
            Debug.LogError("[BattleMessage]: Role or Bullet is Null, Generate Bullet Error ,Please Check!");
            yield break;
        }
        Role player = role;
        Vector2Int index = targetIndex;
        //依据选择的地块和玩家当前的高度执行射击
        //获取对应索引的棋盘格的XZ坐标
        List<BattleGrid> grids = BattleBoard.instance?.GetBattleGridList();
        BattleGrid grid = null;
        foreach (BattleGrid g in grids)
        {
            if (g == null) continue;
            if (g.GetIndex().x == index.x && g.GetIndex().y == index.y)
            {
                grid = g;
                break;
            }
        }
        Vector3 target = new Vector3(
            grid == null ? 0.0f : (float)grid?.transform.position.x,
            player == null ? 0.0f : (float)player?.transform.position.y,
            grid == null ? 0.0f : (float)grid?.transform.position.z
        );
        Vector3 direction = (target - (Vector3)player?.transform.position).normalized;
        //产生子弹实体并设置其方向和初始位置
        Bullet bt = Instantiate(bulletPrefab, target, Quaternion.identity);
        if (bt != null)
        {
            bt.SetSide(player.GetSide());//设置子弹的阵营
            bt.transform.position = (Vector3)player?.transform.position + posOffset;//设置子弹的初始位置
            bt.SetDirection(direction);//设置子弹的方向
        }
        //控制玩家动画播放
        if (triggerAnim)
        {
            RoleAnimTrigger animTrigger = player?.GetComponent<RoleAnimTrigger>();
            animTrigger?.TriggerAnim(roleAnimName);
            AnimatorStateInfo stateInfo = (AnimatorStateInfo)((Animator)animTrigger?.GetComponent<Animator>())?.GetCurrentAnimatorStateInfo(0);
            yield return (float)stateInfo.normalizedTime * (float)stateInfo.length;
        }
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
    public List<CardSlotList> GetCardSlotListList_Copy()
    {
        return cardSlotListList.ToList();
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

    public List<CardSlot> GetAllCardSlot_Copy()
    {
        return GetAllCardSlot().ToList();
    }

    //每个种类的卡槽列表中卡槽的数量
    //卡槽数量更新时以整个字典为准
    [Header("卡槽列表中卡槽的数量:仅限Power,Attack,Gadget这3种可设置")]
    [SerializeField] private SerializableDictionary<CardCategory, int> cardSlotListCardSlotCount = new SerializableDictionary<CardCategory, int>();
    public SerializableDictionary<CardCategory, int> GetCardSlotListCardSlotCount()
    {
        return cardSlotListCardSlotCount;
    }

    public SerializableDictionary<CardCategory, int> GetCardSlotListCardSlotCount_Copy()
    {
        return new SerializableDictionary<CardCategory, int>(cardSlotListCardSlotCount);
    }
    //主要扩展功能
    /*
        1.根据LodeInMessage信息重置场景,包括棋盘格的信息
    */
    public void ResetBattleSceneByLodeInMessage(BattleLodeInMessage lodeInMessage)
    {
        //确保参数有效
        if(lodeInMessage == null) 
        {
            Debug.LogError("[BattleMessage]: BattleLodeInMessage is null, Please Check!");
            return;
        }
        BattleBoard board = BattleBoard.instance;
        if(board == null) 
        {
            Debug.LogError("[BattleMessage]: BattleBoard is null, Please Check the Instance is really exist!");
            return;
        }
        /*
            一下部分为清除已有的战斗场景信息
        */
        //清除战斗信息中的所有角色(包括控制的玩家)
        foreach(Role role in roleList)
        {
            if(role != null)
            {
                roleList.Remove(role);
                Destroy(role.gameObject);
            }
        }
        //清空卡牌列表
        foreach(Card card in handCardList)
        {
            if(card != null)
            {
                handCardList.Remove(card);
                Destroy(card.gameObject);
            }
        }
        foreach(Card card in discardCardList)
        {
            if(card != null)
            {
                discardCardList.Remove(card);
                Destroy(card.gameObject);
            }
        }
        foreach(Card card in drawCardList)
        {
            if(card != null)
            {
                drawCardList.Remove(card);
                Destroy(card.gameObject);
            }
        }
        //卡槽中的卡牌
        foreach(CardSlot cardSlot in GetAllCardSlot().ToList())
        {
            if(cardSlot == null) continue;
            if(cardSlot.GetInnerCard() != null) 
            {
                Destroy(cardSlot.GetInnerCard().gameObject);
                cardSlot.SetInnerCard(null);
            }
        }
        //清空BattleBoard中的棋盘格
        List<BattleGrid> grids = board.GetBattleGridList();
        if(grids!=null)
        {
            foreach(BattleGrid grid in grids)
            {
                grids.Remove(grid);    
                Destroy(grid.gameObject);
            }   
        }
        /*
            一下部分为依据LodeInMessage信息初始化战斗场景
        */
        int id_append = 0;

        /*
            这部分需要从玩家信息获取器中实时读取玩家信息
            信息包括:玩家的默认阵营,玩家血量,玩家最大生命值,玩家金币量,玩家卡牌列表+未加入的其他控制信息
        */
        //初始化玩家角色,依据lodeInMessage设置玩家位置,并设置控制的玩家ID为当前玩家
        Role player = null;
        //Instantiate(player,board.transform);
        player?.SetSide(true);
        player?.SetID((uint)id_append);
        controlPlayerID = (uint)player?.GetID();
        //初始化玩家的手牌

        //初始化敌人及其位置
        foreach(KeyValuePair<Vector2Int, Role> role_pair in lodeInMessage.GetEnermyDict().ToList())
        {
            if(role_pair.Value == null) continue;//敌人角色为空时跳过
            Role role = Instantiate(role_pair.Value,board.transform);//向棋盘中加入角色
            //初始化敌人角色位置
            role?.SetGridIndex(role_pair.Key);
            //初始化敌人角色ID
            role?.SetID((uint)id_append);
            //初始化敌人角色的阵营,不同于玩家角色
            role?.SetSide(!(bool)player?.GetSide());
            //将敌人加入列表
            roleList.Add(role);
            id_append++;
        }
    }
}
