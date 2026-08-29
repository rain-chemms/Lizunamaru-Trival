using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CardSystem.AllCardHub
{
    //空白卡牌
    public class WhiteEmptyCard : Card
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        //卡牌接口的空实现
        public override IEnumerator AfterInsertToSolt()
        {
            yield return null;
            yield return base.AfterInsertToSolt();
        }
        public override IEnumerator AfterPlay()
        {
            List<Card> cardList = BattleMessage.instance?.GetHandCardList()?.ToList();
            if (cardList == null)
            {
                Debug.LogError("[WhiteEmptyCard]: The HandCardList is null, Please Check!");
                yield return null;
            }
            foreach (Card card in cardList)
            {
                yield return (BattleMessage.instance?.DiscardCard(card));
            }
            //回复相应的麦饭点数
            BattleMessage.instance?.SetRicePoint((uint)BattleMessage.instance?.GetRicePoint() + (uint)cardList?.Count);
            yield return 1.5f;//时间间隔
                              //随后触发卡牌的基础检测
            yield return base.AfterPlay();
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
