using UnityEngine;
using System.Collections;

namespace BonusSystem.BonusFuncs
{
    //金币奖励生效器
    public class CoinBonus : Bonus
    {
        public override IEnumerator OnEffective()
        {
            BattleMessage.instance?.SetCoins((uint)BattleMessage.instance?.GetCoins() + bonusCount);
            EffectiveSuccess = true;
            yield return null;
        }
    }
}
