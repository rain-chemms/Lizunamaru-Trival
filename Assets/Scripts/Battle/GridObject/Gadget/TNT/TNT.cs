using UnityEngine;
using BulletSystem;
using System.Collections;

namespace GridObjectSystem.GadgetSystem.Tnts
{
    //TNT: 放置后第i个回合爆炸
    public class TNT : Gadget
    {
        [SerializeField] private float damage = 30;//爆炸伤害
        public float GetDamage() => damage;
        public void SetDamage(float damage) => this.damage = damage;
        [SerializeField] private int clockNumber = 3;//计时器时长
        public int GetClockNumber() => clockNumber;
        public void SetClockNumber(int number) => clockNumber = number;
        [SerializeField] private int roundRecorder = int.MaxValue;//计时器记录
        public int GetRoundRecorder() => roundRecorder;
        //激活时设置计时器
        protected override void OnEnable()
        {
            base.OnGadgetEffect();
            TryGetTheExplosion();
            roundRecorder = 0;
        }

        //每回合结束时计数器减一,并使用计时器播放音效
        [SerializeField] private AudioSource ticker = null;
        [SerializeField] private ParticleSystem particle_ticker = null;
        //并检测是否爆炸
        public override IEnumerator OnEveryRoundEnd()
        {
            if(BattleMessage.instance.IsPlayerTurn() == GetSide())
            {
                roundRecorder += 1;
                ticker?.Play();
                particle_ticker?.Play();
                if(roundRecorder >= clockNumber) yield return OnGadgetEffect();
            }
            yield return base.OnEveryRoundEnd();
        }

        //启动效果时爆炸
        [SerializeField] private Bullet explosion = null;
        private void TryGetTheExplosion()
        {
            if(explosion == null) explosion = GetComponentInChildren<Bullet>();
        }

        public override IEnumerator OnGadgetEffect()
        {
            //设置子弹爆炸器的阵营
            if(explosion == null) TryGetTheExplosion();
            explosion?.SetDamage(damage);//设置子弹伤害
            explosion?.SetSide(GetSide());
            return base.OnGadgetEffect();
        }

    }
}