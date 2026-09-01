using UnityEngine;
using BulletSystem;
using System.Collections;

namespace GridObjectSystem.GadgetSystem.Guns
{
    //迷你八卦炉: (相关角色:marisa) 每回合开始时向角色所朝方向发射一发激光
    public class MiniHakerro : Gadget
    {
        [SerializeField] private Bullet bulletPrefab;
        public Bullet GetBulletPrefab() => bulletPrefab;

        private void SetBulletParentToSelf(Bullet bt)
        {
            if(bt == null) return;
            bt.transform.SetParent(transform);
            //尝试关闭子弹的旋转同步器
            var dSyncer = bt.GetComponent<BulletRotateDirectionSetter>();
            if(dSyncer != null) 
            {
                dSyncer.ChangeBulletDirection_Instant();//瞬间设置子弹方向
                dSyncer.SetSyncDirectionOpen(false);
            }
            //自己设置子弹的初始方向

        }

        public override IEnumerator OnGadgetEffect()
        {
            yield return base.OnGadgetEffect();
            //获取目标方向
            Vector2Int target = GetGridIndex();
            switch (GetDirection())
            {
                case BattleDirection.RIGHT:
                    target.x += 1;
                    break;
                case BattleDirection.LEFT:
                    target.x -= 1;
                    break;
                case BattleDirection.DOWN:
                    target.y -= 1;
                    break;
                case BattleDirection.UP:
                default:
                    target.y += 1;
                    break;
            }
            //调用生成子弹
            yield return BattleMessage.instance?.GenerateBullet(this,bulletPrefab,target,default,true,"Shoot",SetBulletParentToSelf);
        }

        public override IEnumerator OnEveryRoundStart()
        {
            yield return OnGadgetEffect();
            yield return base.OnEveryRoundStart();
        }
    }
}