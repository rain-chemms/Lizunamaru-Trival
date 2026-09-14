using UnityEngine;
using BulletSystem;
using System.Collections;

namespace GridObjectSystem.GadgetSystem.KeyStones
{
    /// <summary>
    /// 要石: 相关角色:比那名居天子
    /// 效果: 抵挡一定次数的子弹攻击
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class KeyStone : Gadget
    {
        [SerializeField] private int defendTimes = 10;//抵挡次数
        public int GetDefendTimes() => defendTimes;
        public void SetDefendTimes(int times) => defendTimes = times;
        [SerializeField] private int defendTimesCounter = 0;//抵挡次数计数器
        public int GetDefendTimesCounter() => defendTimesCounter;
        public int SetDefendTimesCounter(int counter) => defendTimesCounter = counter;

        protected override void OnEnable()
        {
            if(animator == null) animator = GetComponent<Animator>();
            base.OnEnable();
        }

        //检测子弹进入
        void OnTriggerEnter(Collider other)
        {
            Bullet bt = other.GetComponent<Bullet>();
            if(bt != null)
            {
                if(bt.GetSide() != GetSide())//若进入的是敌方子弹
                {
                    BulletDestroyController bdc = bt.GetComponent<BulletDestroyController>();
                    if(bdc != null)
                    {
                        bdc.TriggerTheDestroy();
                    }
                    else
                    {
                        //直接消除子弹物体
                        Destroy(bt.gameObject);
                    }
                    defendTimesCounter ++;
                    if(defendTimesCounter < defendTimes) animator?.SetTrigger("Blocked");//小于抵挡次数,播放抵挡成功动画
                }
            }
            StartCoroutine(CheckAndDestroy());//检测是否要销毁
        }

        //要石的销毁由自身控制,卡牌只是负责产生要石
        [SerializeField] private Animator animator;
        private IEnumerator CheckAndDestroy()
        {
            bool willDestroy = defendTimesCounter >= defendTimes;
            if(willDestroy)
            {
                if(animator != null)
                {
                    animator.SetBool("Open", false);
                    yield return null;
                    AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                    yield return new WaitForSeconds(info.length / info.speed);//等待播放动画
                }
                Destroy(gameObject);
            }
        }
    }
}
