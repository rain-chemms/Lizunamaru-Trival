using UnityEngine;

//战斗中的方向位置种类
public enum BattleDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}


//卡片的种类
//用于标记卡槽,卡槽列表,卡片的种类
public enum CardCategory
{
    POWER,//能力卡
    GADGET,//道具卡
    ATTACK,//攻击卡
    SPELL_ATTACK,//符卡
    EFFECTIVE//即时生效卡
}

//地图节点的类别
public enum MapNodeCategory
{
    STORE,//商店
    CHAPTER_START,//事件:每层的开始事件:默认为月虹市场
    EVENT,//事件
    BATTLE_ELITE,//精英:道中BOSS
    BATTLE_NORMAL,//小怪
    BATTLE_BOSS,//关底BOSS
    CAMPFIRE,//篝火
    BONUS//奖励箱
}

//卡牌关键字
public enum CardKeyWord
{
    EXHAUST,//消耗,打出后从本场战斗中移除
    RETAIN,//保留,回合结束时不回到弃牌堆
    ETHEREAL,//虚无,回合结束时在手中则消耗
    INNATE,//固有,开局时加入手牌
    UNPLAYABLE,//不可被打出
    UNINSERTABLE,//不可插入卡槽
    SLY,//奇巧,丢弃手牌时将其打出
    ETERNAL,//永恒,无法从牌堆中移除
    SLOT,//槽位,牌嵌入槽位时可以在上面叠加更多卡牌
}