using UnityEngine;
using BulletSystem;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Bullet))]
    public class SolemnBulletAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private Bullet bullet;
        [SerializeField] private ParticleSystem particleStm;//关联的粒子系统
        [SerializeField] private int particleCount = 1000;
        void OnEnable()
        {
            if(bullet == null) bullet = GetComponent<Bullet>();
            if(particleStm == null) particleStm = GetComponentInChildren<ParticleSystem>();
        }    

        public void DestoryBullet()
        {
            Destroy(bullet?.gameObject);
        }

        public void EmitParticle()
        {
            particleStm?.Emit(particleCount);
        }

        public void HaltBullet()//让子弹停下来
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if(rb!=null) rb.linearVelocity = Vector3.zero;
        }
    }
}