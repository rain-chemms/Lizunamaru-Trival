using System.Collections;
using UnityEngine;

namespace BonusSystem
{
    //奖励生效器
    public interface IBonusEffectiver
    {
        public abstract IEnumerator OnEffective();
    }
}
