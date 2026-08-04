using UnityEngine;
using BulletSystem;
using System.Collections;

namespace GridObjectSystem.GadgetSystem.Guns
{
    public class SolemnLament : Gadget
    {
        [SerializeField] private Bullet bulletPrefab;//存储的子弹,依据不同种类的阴阳玉分别设置
        public Bullet GetBulletPrefab() => bulletPrefab;
        [SerializeField] private uint bulletNumberPreRound = 6;//每回合产生多少颗对应的子弹
        public uint GetBulletNumberPreRound() => bulletNumberPreRound;
        public void SetBulletNumberPreRound(uint preRound) => bulletNumberPreRound = preRound;
        [SerializeField] private float shootInterval;//两发子弹的射击间隔
        public float GetShootInterval() => shootInterval;
        public override IEnumerator OnGadgetEffect()
        {
            yield return base.OnGadgetEffect();
            if(!enabled) yield break;
            for (uint i = 0; i < bulletNumberPreRound; i++)
            {
                yield return GenerateBullet();
                yield return new WaitForSeconds(shootInterval);//进行射击等待
            }
        }
        //每回合开始和结束时都触发射击
        public override IEnumerator OnEveryRoundEnd()
        {
            yield return base.OnEveryRoundEnd();
            if(!enabled) yield break;
            yield return OnGadgetEffect();
        }

        public override IEnumerator OnEveryRoundStart()
        {
            yield return base.OnEveryRoundStart();
            if(!enabled) yield break;
            yield return OnGadgetEffect();
        }

        private IEnumerator GenerateBullet()
        {
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

            //调起子弹产生协程并传入参数
            yield return BattleMessage.instance?.GenerateBullet(
                this,//传入产生的Role信息,包含位置等
                bulletPrefab,//子弹预设体
                target,//目标位置
                default,
                true,
                "Shoot"
            );
        }
    }
}