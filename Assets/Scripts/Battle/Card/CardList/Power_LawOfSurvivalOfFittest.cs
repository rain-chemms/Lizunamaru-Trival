using UnityEngine;
using System.Collections;
using BulletSystem;
using System;
using System.Linq;
using GridObjectSystem.RoleSystem;

namespace CardSystem.AllCardHub
{
    //在卡槽中时,你的所有非激光射击伤害{0}
    //你的回合结束之后若仍在卡槽中,获得{1}点生命值
    public class Power_LawOfSurvivalOfFittest : Card
    {
        [SerializeField] private float increaseBulletDamage = 3.0f;
        public float GetIncreaseBulletDamage() => increaseBulletDamage;
        public void SetIncreaseBulletDamage(float damage) => increaseBulletDamage = damage;
        [SerializeField] private float recoverHpPreRound = 3;
        public float GetRecoverHpPreRound() => recoverHpPreRound;
        public void SetRecoverHpPreRound(float hp) => recoverHpPreRound = hp;
        [SerializeField] private bool bPPAdded = false;
        public override IEnumerator AfterRoundEnd()
        {
            yield return base.AfterRoundEnd();
            bool inSlot = false;    
            foreach(CardSlot slot in BattleMessage.instance?.GetAllCardSlot_Copy())
            {
                if(slot == null) continue;
                if(slot.GetInnerCard() == this)
                {
                    inSlot = true;
                    break;
                }
            }
            if(!inSlot) yield break;//不在卡槽中不触发
            //触发回血
            BattleMessage instance = BattleMessage.instance;
            Role player = instance?.GetControlPlayer();
            if(player != null && instance != null)
            {
                if(player.GetSide() == instance.IsPlayerTurn())
                {
                    RoleHealther roleHealther = player.GetComponent<RoleHealther>();
                    if(roleHealther != null)
                    {
                        roleHealther.GetHealth(recoverHpPreRound);
                    }
                    else
                    {
                        if(recoverHpPreRound >= 0)
                        {
                            float newHp = player.GetHp() + recoverHpPreRound;
                            if(newHp > player.GetMaxHp()) newHp = player.GetMaxHp();
                            player.SetHp(newHp);
                        }
                    }
                    Debug.Log("[Power_LawOfSurvivalOfFittest]: Trigger The health To Player:"+player.name+", Health Point:"+recoverHpPreRound);
                }
            }
        }
        
        public override IEnumerator AfterInsertToSolt()
        {
            //在卡槽中时
            if (BattleMessage.instance != null)
            {
                if(!bPPAdded)
                {
                    BattleMessage.instance.BulletPostProcess += BulletPostProcess;
                    bPPAdded = true;
                }
                BattleMessage.instance.SelfTurnStartAction += OnSelfTurnStart;
                BattleMessage.instance.SelfTurnEndAction += OnSelfTurnEnd;
            
                Debug.Log("[Power_LawOfSurvivalOfFittest]: Insert Over and Set");
            }
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterRemoveFromSolt()
        {


            //移除出卡槽时,释放两个效果
            if (BattleMessage.instance != null)
            {
                if(bPPAdded)
                {
                    BattleMessage.instance.BulletPostProcess -= BulletPostProcess;
                    bPPAdded = false;
                }
                Action actStart = BattleMessage.instance.SelfTurnStartAction -= OnSelfTurnStart;
                Action actEnd = BattleMessage.instance.SelfTurnEndAction -= OnSelfTurnEnd;
                Debug.Log("[Power_LawOfSurvivalOfFittest]: Remove Over and disSet");
            }
            yield return base.AfterPlay();
        }
        //己方回合和目前控制的角色有关
        private void OnSelfTurnStart()
        {
            //添加后处理函数
            if (BattleMessage.instance != null)
            {
                BattleMessage.instance.BulletPostProcess += BulletPostProcess;
                bPPAdded = true;
            }
            Debug.Log("[Power_VampireFang]: 己方回合开始,添加BulletPostProcess");
        }

        private void OnSelfTurnEnd()
        {
            //移除后处理函数
            if (BattleMessage.instance != null)
            {
                BattleMessage.instance.BulletPostProcess -= BulletPostProcess;
                bPPAdded = false;
            }
            Debug.Log("[Power_VampireFang]: 己方回合结束,移除BulletPostProcess");
        }

        private void BulletPostProcess(Bullet bt)
        {
            if (bt == null) return;
            //为非激光子弹添加伤害
            if(increaseBulletDamage > 0 && !(bool)bt?.GetComponent<BulletDamageTrigger>()?.IsLaserMode()) bt?.SetDamage(bt.GetDamage() + increaseBulletDamage);
        }
    }
}