using UnityEngine;
using System.Collections;
using BulletSystem;
using System;

namespace CardSystem.AllCardHub
{
    //爆裂赤蛙:相关角色诹坊子
    //己方回合开始时,每次射击时子弹命中是会产生一个爆炸
    public class Power_BurstingRedFrog : Card
    {
        [SerializeField] private Bullet explosionPrefab;
        public void SetExplosionPrefab(Bullet bt) => explosionPrefab = bt;
        public Bullet GetExplosionPrefab() => explosionPrefab;

        //检查器
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
            
                Debug.Log("[Power_BurstingRedFrog]: Insert Over and Set");
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
                Debug.Log("[Power_BurstingRedFrog]: Remove Over and disSet");
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
            Debug.Log("[Power_BurstingRedFrog]: 己方回合开始,添加BulletPostProcess");
        }

        private void OnSelfTurnEnd()
        {
            //移除后处理函数
            if (BattleMessage.instance != null)
            {
                BattleMessage.instance.BulletPostProcess -= BulletPostProcess;
                bPPAdded = false;
            }
            Debug.Log("[Power_BurstingRedFrog]: 己方回合结束,移除BulletPostProcess");
        }

        //子弹后处理脚本
        private void BulletPostProcess(Bullet bt)
        {
            if (bt == null) return;
            //为其添加脚本
            BulletExplosionGenerator beg = bt.gameObject.AddComponent<BulletExplosionGenerator>();//添加爆炸生成脚本
            //设置爆炸的子弹预制体
            beg.SetExplosionPrefab(explosionPrefab);//添加爆炸的子弹预制体
            Debug.Log("[Power_BurstingRedFrog]: 已为当前子弹添加脚本:" + beg.GetType().Name);
        }
        
    }
}
