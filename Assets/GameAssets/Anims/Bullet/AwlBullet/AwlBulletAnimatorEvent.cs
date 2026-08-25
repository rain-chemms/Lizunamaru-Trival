using UnityEngine;
using BulletSystem;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Bullet))]
    public class AwlBulletAnimatorEvent : MonoBehaviour
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