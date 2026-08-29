using System.Collections;
using UnityEngine;
using GridObjectSystem.RoleSystem;

namespace CardSystem.AllCardHub
{
    public class DefendCard : Card
    {
        [SerializeField] private int gainDefendPoint = 1;
        //卡牌接口的空实现
        public override IEnumerator AfterInsertToSolt()
        {
            yield return null;
            yield return base.AfterInsertToSolt();
        }
        public override IEnumerator AfterPlay()
        {
            yield return base.AfterPlay();
            //让当前玩家获取格挡值
            yield return BattleMessage.instance?.GetRole(
                (uint)BattleMessage.instance?.GetControlPlayerID(),
                true
            )?.GetComponent<RoleDefendGetter>()?.GetOrLoseDefend(gainDefendPoint);
            yield return null;

        }
        public override IEnumerator AfterRemoveFromSolt()
        {
            yield return null;
            yield return base.AfterRemoveFromSolt();
        }
        public override IEnumerator AfterTriggerEffective()
        {
            yield return null;
            yield return base.AfterTriggerEffective();
        }
        public override IEnumerator AfterRoundEnd()
        {
            yield return null;
            yield return base.AfterRoundEnd();
        }
        //回合开始时触发
        public override IEnumerator AfterRoundStart()
        {
            yield return null;
            yield return base.AfterRoundStart();
        }

        //在你的回合丢弃时触发
        public override IEnumerator AfterDiscard()
        {
            yield return null;
            yield return base.AfterDiscard();
        }

        //在抽到卡牌时触发
        public override IEnumerator AfterDraw()
        {
            yield return null;
            yield return base.AfterDraw();
        }
    }
}
