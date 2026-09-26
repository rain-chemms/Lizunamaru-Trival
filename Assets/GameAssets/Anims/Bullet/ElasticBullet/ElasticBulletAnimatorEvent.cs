using System;
using System.Collections.Generic;
using BulletSystem;
using UnityEngine;


namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Bullet))]
    public class ElasticBulletAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private Bullet bullet;
        [SerializeField] private List<Collider> cldList;
        void OnEnable()
        {
            foreach(Collider c in GetComponentsInChildren<Collider>())
            {
                if(c == null) continue;
                if(cldList.Contains(c))
                {
                    cldList.Add(c);
                }
            }
        }

        private void DestroyBullet()
        {
            Destroy(bullet?.gameObject);
        }

        private void CloseBulletCollider()
        {
            foreach(Collider c in cldList)
            {
                if(c == null) continue;
                c.enabled = false;
            }
        }
    }
}
