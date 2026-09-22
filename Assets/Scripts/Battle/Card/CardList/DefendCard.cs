using System.Collections;
using UnityEngine;
using GridObjectSystem.RoleSystem;

namespace CardSystem.AllCardHub
{
    public class DefendCard : Card
    {
        [SerializeField] private int gainDefendPoint = 1;
        //卡牌接口的空实现
        public override IEnumerator AfterPlay()
        {
            //让当前玩家获取格挡值
            yield return BattleMessage.instance?.GetRole(
                (uint)BattleMessage.instance?.GetControlPlayerID(),
                true
            )?.GetComponent<RoleDefendGetter>()?.GetOrLoseDefend(gainDefendPoint);
            yield return null;
            yield return base.AfterPlay();
        }
    }
}
