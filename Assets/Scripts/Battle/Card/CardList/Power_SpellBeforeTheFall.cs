using UnityEngine;
using System.Collections;
using BulletSystem;
using System;
using GridObjectSystem.RoleSystem;

namespace CardSystem.AllCardHub
{
    //未雨绸缪之符:相关角色 帕秋莉.诺雷姬
    //在卡槽中时,每个回合结束时获得 {0:mul(100)}% 的符卡充能
    //打出时,获得 {1:mul(100)} 的符卡充能
    public class Power_SpellBeforeTheFall : Card
    {
        [SerializeField] private float increaseSpellPreRound = 0.2f;
        public float GetIncreaseSpellPreRound() => increaseSpellPreRound;
        public void SetIncreaseSpellPreRound(float precent) => increaseSpellPreRound = precent;
        [SerializeField] private float increaseSpellAfterPlay = 0.5f;
        public float GetIncreaseSpellAfterPlay() => increaseSpellAfterPlay;
        public void SetIncreaseSpellAfterPlay(float precent) => increaseSpellAfterPlay = precent;
        
        public override IEnumerator AfterPlay()
        {
            Role player = BattleMessage.instance?.GetControlPlayer();
            //小于0时不生效
            if(increaseSpellAfterPlay >= 0)
            {
                player?.SetSpellPrecent((float)player?.GetSpellPrecent() + increaseSpellAfterPlay);
            }
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterRoundEnd()
        {
            //检测卡牌是否在卡槽中
            if(!(bool)BattleMessage.instance?.IsCardInSlot(this))
            {
                yield break;//不在卡槽中时,不生效
            }
            Role player = BattleMessage.instance?.GetControlPlayer();
            //小于0时不生效
            if(increaseSpellPreRound >= 0)
            {
                if(player?.GetSide() == BattleMessage.instance?.IsPlayerTurn())
                {    
                    player?.SetSpellPrecent((float)player?.GetSpellPrecent() + increaseSpellPreRound);
                }
            }
            yield return base.AfterRoundEnd();
        }
    }
}