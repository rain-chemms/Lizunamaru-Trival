using System.Collections;
using UnityEngine;

namespace CardSystem.AllCardHub
{
    //万宝槌:第一张攻击卡,相关角色:少名针妙丸
    public class Attack_MiracleMallet : Card
    {
        public override IEnumerator AfterPlay()
        {
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterTriggerEffective()
        {
            Debug.Log("[Attack_MiracleMallet]: Trigger The Effective!");
            yield return base.AfterTriggerEffective();
        }
    }
}
