using UnityEngine;
using BulletSystem;
using System.Collections;
using Unity.VisualScripting;

namespace GridObjectSystem.GadgetSystem.Hakerros
{
    //万宝槌:相关角色:少名针妙丸
    public class MiracleMallet : Gadget
    {
        [SerializeField] private uint coinPreBullet = 1;//每消除一个子弹产生的金币数
        
        [SerializeField] public Bullet explosionPrefab;
        public Bullet GetExplosionPrefab() => explosionPrefab;

        //产生爆炸
        public void GenerateExplosion()
        {
            //生成子弹物体
            StartCoroutine(BattleMessage.instance?.GenerateBullet(
                this,
                explosionPrefab,
                (Vector2Int)this?.GetGridIndex(),
                default,
                false,
                "",
                PostProcess
                )
            );
        }

        //为爆炸子弹添加上可以消除非激光子弹并且增加金币的脚本
        private void PostProcess(Bullet bullet)
        {
            GameObject childChecker = new GameObject();
            childChecker.name = "BulletCrackChecker";
            childChecker.layer = LayerMask.NameToLayer("BulletCrackChecker");
            childChecker.transform.SetParent(bullet.transform);//添加子物体
            childChecker.transform.localPosition = Vector3.zero;
            //添加Rigidbody使子物体成为独立物理体,避免被父级Rigidbody合并为复合碰撞体
            Rigidbody childRb = childChecker.AddComponent<Rigidbody>();
            childRb.useGravity = false;
            childRb.isKinematic = true;//运动学模式,跟随父物体移动
            SphereCollider sc = childChecker.AddComponent<SphereCollider>();
            sc.isTrigger = true;//必须为true才能触发OnTriggerEnter
            //安全获取半径,避免对值类型使用?.后强转
            float baseRadius = 0.25f;
            SphereCollider sphere = bullet.GetComponent<SphereCollider>();
            if (sphere != null) baseRadius = sphere.radius;
            else
            {
                CapsuleCollider capsule = bullet.GetComponent<CapsuleCollider>();
                if (capsule != null) baseRadius = capsule.radius;
            }
            sc.radius = baseRadius * 1.5f;//爆炸判定范围略大于子弹本体
            BulletEnermyBulletCrackChecker cck = childChecker.AddComponent<BulletEnermyBulletCrackChecker>();
            cck.Bullet = bullet;//设置子弹
            //添加响应事件
            cck.AppendAction += GenerateCoin;
            cck.EffectToEnermyButtlet += DestroyEnermyBullet;
        }

        private void DestroyEnermyBullet(Bullet bt)
        {
            //不会消除激光子弹
            BulletDamageTrigger bdt = bt?.GetComponent<BulletDamageTrigger>();
            if(bdt != null && (bool)bdt?.IsLaserMode()) return;
            //尝试使用BulletDestroyController消除
            BulletDestroyController dc = bt?.GetComponent<BulletDestroyController>();
            if(dc != null)//存在销毁控制器
            {
                //触发销毁
                dc.TriggerTheDestroy();
            }
            else//没有销毁控制器
            {
                //销毁子弹
                Destroy(bt);
            }
        }

        private void GenerateCoin()
        {
            //增加金币量
            BattleMessage.instance?.AddCoins(coinPreBullet);
        }
    }
}