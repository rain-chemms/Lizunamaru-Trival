using UnityEngine;
using BulletSystem;
using System.Collections;
using LuaGameSystem;

namespace GridObjectSystem.GadgetSystem.Guns
{
    public class SolemnLament : Gadget
    {
        [SerializeField] private Bullet bulletPrefab;//存储的子弹,依据不同种类的阴阳玉分别设置
        public Bullet GetBulletPrefab() => bulletPrefab;
        [SerializeField] private uint bulletNumberPreRound = 2;//每回合产生多少颗对应的子弹
        public uint GetBulletNumberPreRound() => bulletNumberPreRound;
        public void SetBulletNumberPreRound(uint preRound) => bulletNumberPreRound = preRound;
        [SerializeField] private float shootInterval;//两发子弹的射击间隔
        public float GetShootInterval() => shootInterval;
        public override IEnumerator OnGadgetEffect()
        {
            yield return base.OnGadgetEffect();
            //朝当前forward方向射击子弹
            if(!enabled) yield break;
            for (uint i = 0; i < bulletNumberPreRound; i++)
            {
                GenerateBullet();
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

        private BulletGenerator bulletGenerator = new BulletGenerator();
        new void Start()
        {
            base.Start();
            if(bulletGenerator == null) bulletGenerator = new BulletGenerator();
            bulletGenerator.SetGenerateObject(this);
            bulletGenerator.SetBullet(bulletPrefab);
        }

        private void GenerateBullet()
        {
            bulletGenerator?.GenerateBullet((Vector3)GetRigidBody()?.transform.forward);
            GetComponent<Animator>().SetTrigger("Shoot");
        }
    }
}