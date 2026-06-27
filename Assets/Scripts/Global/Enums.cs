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