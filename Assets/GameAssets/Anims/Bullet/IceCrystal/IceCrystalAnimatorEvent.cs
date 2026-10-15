using UnityEngine;
using BulletSystem;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Bullet))]
    public class IceCrystalAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Bullet bullet;
        void OnEnable()
        {
            if(bullet == null) bullet = GetComponent<Bullet>();
            if(animator == null) animator = GetComponent<Animator>();
        }    

        public void DestoryBullet()
        {
            Destroy(bullet?.gameObject);
        }

        [SerializeField] private float diffuseParameter = 0.1f;
        public void GetAndSetRotateSpeed()
        {
            Rigidbody rb = bullet?.GetRigidBody();
            if(rb != null)
            {
                float speed = rb.linearVelocity.sqrMagnitude;
                animator.SetFloat("RotateSpeed",diffuseParameter * speed);
            }
        }
    }
}
