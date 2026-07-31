using UnityEngine;
using BulletSystem;
namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Bullet))]
    public class SwordEdgeBulletAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private Bullet bullet;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            if (bullet == null) bullet = GetComponent<Bullet>();
        }

        public void AfterBulletMoveOver()
        {
            Destroy(bullet?.gameObject);
        }
    }
}