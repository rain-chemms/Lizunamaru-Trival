using UnityEngine;
using System.Collections;
using BulletSystem;

namespace CardSystem.AllCardHub
{
    public class ShootCard : Card
    {
        [SerializeField] private Bullet bulletPrefab;
        public Bullet GetBulletPrefab()
        {
            return bulletPrefab;
        }
        public void SetBulletPrefab(Bullet newBulelt)
        {
            this.bulletPrefab = newBulelt;
        }
        //卡牌接口的空实现
        public override IEnumerator AfterInsertToSolt()
        {
            yield return null;
            yield return base.AfterInsertToSolt();
        }
        public override IEnumerator AfterPlay()
        {
            /*
                整个方法可以包装未一个从玩家处产生子弹的协程
            */
            yield return base.AfterPlay();
            //调起子弹产生协程并传入参数
            yield return BattleMessage.instance?.GenerateBullet(
                BattleMessage.instance?.GetRole(
                    (uint)BattleMessage.instance?.GetControlPlayerID(),
                    true
                ),//传入产生的Role信息,包含位置等
                bulletPrefab,//子弹预设体
                (Vector2Int)ConcentratePoint.instance?.GetIndex(),//目标位置
                default
            );
        }
        public override IEnumerator AfterRemoveFromSolt()
        {
            yield return null;
            yield return base.AfterRemoveFromSolt();
        }
        public override IEnumerator AfterTriggerEffective()
        {
            yield return null;
            yield return base.AfterTriggerEffective();
        }
        public override IEnumerator AfterRoundEnd()
        {
            yield return null;
            yield return base.AfterRoundEnd();
        }
        //回合开始时触发
        public override IEnumerator AfterRoundStart()
        {
            yield return null;
            yield return base.AfterRoundStart();
        }

        //在你的回合丢弃时触发
        public override IEnumerator AfterDiscard()
        {
            yield return null;
            yield return base.AfterDiscard();
        }

        //在抽到卡牌时触发
        public override IEnumerator AfterDraw()
        {
            yield return null;
            yield return base.AfterDraw();
        }
    }
}