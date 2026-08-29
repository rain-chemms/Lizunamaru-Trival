using UnityEngine;
using System.Collections;
using GridObjectSystem.RoleSystem;
using BulletSystem;

namespace CardSystem.AllCardHub
{
    public class SpellCard_MasterSpark : Card
    {
        [SerializeField] private Bullet bulletPrefab;
        //卡牌接口的空实现
        public override IEnumerator AfterInsertToSolt()
        {
            yield return null;
            yield return base.AfterInsertToSolt();
        }
        public override IEnumerator AfterPlay()
        {
            yield return base.AfterPlay();
            yield return GenertaeMasterSpark();
            yield return null;
        }
        public override IEnumerator AfterRemoveFromSolt()
        {
            yield return null;
            yield return base.AfterRemoveFromSolt();
        }
        public override IEnumerator AfterTriggerEffective()
        {
            base.AfterTriggerEffective();
            yield return GenertaeMasterSpark();
            yield return null;
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

        private IEnumerator GenertaeMasterSpark()
        {
            Role role = BattleMessage.instance?.GetRole(
                (uint)BattleMessage.instance?.GetControlPlayerID(),
                true
            );
            yield return GetComponent<CardSpellAttackWaker>()?.WakeSpellAttackDisplayer((bool)role?.GetSide());
            yield return BattleMessage.instance?.GenerateBullet(
                role,//传入产生的Role信息,包含位置等
                bulletPrefab,//子弹预设体
                (Vector2Int)ConcentratePoint.instance?.GetIndex(),//目标位置
                default
            );
        }
    }
}