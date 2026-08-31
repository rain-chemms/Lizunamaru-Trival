using UnityEngine;
using System.Collections;
using BulletSystem;
using System;
using System.Linq;

namespace CardSystem.AllCardHub
{
    //吸血鬼之牙:相关角色蕾米莉亚.斯卡雷特
    //己方回合开始时,每次射击时命中敌人时恢复2点生命值
    public class Power_VampireFang : Card
    {
        [SerializeField] private float hpRecover = 2.0f;
        public float GetHpRecover() => hpRecover;
        public void SetHpRecover(float hp) => hpRecover = hp;
        [SerializeField] private bool bPPAdded = false;
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
            
                Debug.Log("[Power_VampireFang]: Insert Over and Set");
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
                Debug.Log("[Power_VampireFang]: Remove Over and disSet");
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
            //为其添加脚本
            BulletHpRecover bhr = bt.gameObject.AddComponent<BulletHpRecover>();//添加生命值恢复脚本
            //设置生命恢复数值和要恢复的目标
            bhr.SetEffectiveRole(BattleMessage.instance?.GetRole((uint)BattleMessage.instance?.GetControlPlayerID(), true));
            bhr.SetRecoverPoint(hpRecover);
            Debug.Log("[Power_VampireFang]: 已为当前子弹添加脚本:" + bhr.GetType().Name);
        }
    }
}
