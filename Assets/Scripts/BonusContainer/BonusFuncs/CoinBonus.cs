using UnityEngine;
using System.Collections;

namespace BonusSystem.BonusFuncs
{
    //金币奖励生效器
    public class CoinBonus : Bonus
    {
        [SerializeField] private uint coinCount;
        public uint CoinCount => coinCount;

        public override IEnumerator OnEffective()
        {
            BattleMessage.instance?.SetCoins((uint)BattleMessage.instance?.GetCoins() + coinCount);
            EffectiveSuccess = true;
            yield return null;
        }
    }
}
