using System;
using System.Collections;
using UnityEngine;
using GridObjectSystem.RoleSystem;
using BulletSystem;

namespace CardSystem.AllCardHub
{
    //卡牌效果:在角色前面一格产生一个剑刃子弹
    public class StrikeCard : Card, CardBulletDamageGetter
    {
        [NonSerialized] private float displayDamage;
        public float GetBulletDamage()
        {
            displayDamage = (float)swordEdge?.GetDamage();
            return (float)swordEdge?.GetDamage();
        }
        [SerializeField] private Bullet swordEdge;
        public void SetSwordEdge(Bullet swordEdge)
        {
            this.swordEdge = swordEdge;
        }
        public Bullet GetSwordEdge()
        {
            return swordEdge;
        }
        //卡牌接口的空实现
        public override IEnumerator AfterInsertToSolt()
        {
            yield return null;
            yield return base.AfterInsertToSolt();
        }
        public override IEnumerator AfterPlay()
        {
            yield return base.AfterPlay();
            Role player = BattleMessage.instance?.GetRole(
                (uint)BattleMessage.instance?.GetControlPlayerID(),
                true
            );
            if (player == null) yield break;
            BattleDirection dr = player.GetDirection();
            Vector2Int targetIndex = player.GetGridIndex();
            Vector3 offset = Vector3.zero;
            Vector2 _gaps = (Vector2)BattleBoard.instance?.GetGapsOfGrid();
            switch (dr)
            {
                case BattleDirection.DOWN:
                    targetIndex.y -= 1;
                    offset = new Vector3(0.0f, 0.0f, -_gaps.y);
                    break;
                case BattleDirection.LEFT:
                    targetIndex.x -= 1;
                    offset = new Vector3(-_gaps.x, 0.0f, 0.0f);
                    break;
                case BattleDirection.RIGHT:
                    targetIndex.x += 1;
                    offset = new Vector3(_gaps.x, 0.0f, 0.0f);
                    break;
                case BattleDirection.UP:
                default:
                    targetIndex.y += 1;
                    offset = new Vector3(0.0f, 0.0f, _gaps.y);
                    break;
            }
            yield return BattleMessage.instance?.GenerateBullet(
                player,//传入产生的Role信息,包含位置等
                swordEdge,//子弹预设体
                targetIndex,//目标位置
                offset,
                true,
                "Attack"
            );
            yield return null;
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

        void Update()
        {
            GetBulletDamage();
        }
    }
}