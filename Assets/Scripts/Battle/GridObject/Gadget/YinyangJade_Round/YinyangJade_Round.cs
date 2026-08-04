using UnityEngine;
using BulletSystem;
using System.Collections;

namespace GridObjectSystem.GadgetSystem.YinyangJades
{
    /// <summary>
    /// 道具:圆形阴阳玉(红/蓝)
    ///     红色:每回合向自身方向发射三发封魔针
    ///     蓝色:发射两发符纸(自动追踪)
    /// </summary>
    public class YinyangJade_Round : Gadget
    {
        [SerializeField] private Bullet bulletPrefab;//存储的子弹,依据不同种类的阴阳玉分别设置
        public Bullet GetBulletPrefab() => bulletPrefab;
        [SerializeField] private uint bulletNumberPreRound;//每回合产生多少颗对应的子弹
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
        //
        public override IEnumerator OnEveryRoundEnd()
        {
            //触发一次攻击
            yield return base.OnEveryRoundEnd();
            if(!enabled) yield break;
            yield return OnGadgetEffect();
        }

        public override IEnumerator OnEveryRoundStart()
        {
            //无功能
            yield return base.OnEveryRoundStart();
            if(!enabled) yield break;
            yield return null;
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
                "Effect"
            );
        }
    }
}
