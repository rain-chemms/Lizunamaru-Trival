using UnityEngine;
using BulletSystem;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Bullet))]
    public class ExplosionAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private Bullet bullet;
        void OnEnable()
        {
            if(bullet == null) bullet = GetComponent<Bullet>();
            if(explosionAudio == null) explosionAudio = GetComponent<AudioSource>();
        }
        
        [SerializeField] private AudioSource explosionAudio;
        [SerializeField] private AudioSource stayAudio;
        public void PlayExplosionAudio()
        {
            explosionAudio?.Play();
        }

        public void PlayStayAudio()
        {
            stayAudio?.Play();
        }
        
        public void DestroyBullet()
        {
            Destroy(bullet?.gameObject);
        }


    }
}
