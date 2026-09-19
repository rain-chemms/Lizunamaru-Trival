using UnityEngine;
using System;
using BulletSystem;

namespace BulletSystem
{
    public class BulletEnermyBulletCrackChecker : MonoBehaviour
    {
        //发生碰撞后的附加
        private event Action appendAction;
        public Action AppendAction
        {
            get => appendAction;
            set => appendAction = value;
        }

        //子弹发生碰撞后,对自身子弹的操作
        private event Action<Bullet> effectToSelfButtlet;
        public Action<Bullet> EffectToSelfButtlet
        {
            get => effectToSelfButtlet;
            set => effectToSelfButtlet = value;
        }

        //子弹发生碰撞后,对敌人子弹的操作
        private event Action<Bullet> effectToEnermyButtlet;
        public Action<Bullet> EffectToEnermyButtlet
        {
            get => effectToEnermyButtlet;
            set => effectToEnermyButtlet = value;
        }

        [SerializeField] private Bullet bullet;
        public Bullet Bullet
        {
            get => bullet;
            set => bullet = value;
        }

        void OnEnable()
        {
            if(bullet == null) bullet = GetComponentInParent<Bullet>();
        }

        private void OnTriggerEnter(Collider other)             
        {
            Bullet bt = other.GetComponent<Bullet>();  
            if(bullet == bt) return;
            Debug.Log("[BulletEnermyBulletCrackChecker]: TriggerEnter : <" + bt.name + ">");        
            if(bt?.GetSide() != bullet?.GetSide())//子弹阵营不同的时候才能触发
            {
                Debug.Log("[BulletEnermyBulletCrackChecker]: Cracked : <" + bt.name + "> and <" + bullet.name + ">");
                effectToEnermyButtlet?.Invoke(bt);
                effectToSelfButtlet?.Invoke(bullet);
                appendAction?.Invoke();
            }
        }

    }
}