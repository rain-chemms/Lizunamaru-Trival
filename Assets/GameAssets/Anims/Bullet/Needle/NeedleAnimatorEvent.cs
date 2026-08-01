using UnityEngine;
using BulletSystem;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Bullet))]
    public class NeedleAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private Bullet bullet;
        void OnEnable()
        {
            if(bullet == null) bullet = GetComponent<Bullet>();
        }    

        public void DestoryBullet()
        {
            Destroy(bullet?.gameObject);
        }
    }
}