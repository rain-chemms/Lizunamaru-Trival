using UnityEngine;
using BulletSystem;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Bullet))]
    public class MasterSpark_SmallAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Transform masterSpark_Small;
        
        void OnEnable()
        {
            if(audioSource == null) audioSource = GetComponent<AudioSource>();
            if(masterSpark_Small == null) masterSpark_Small = GetComponent<Transform>();
        }
        
        public void OnMAsterSparkAnimaOver()//播放结束时候销毁特效物体
        {
            Destroy(masterSpark_Small?.gameObject);
        }

        public void StopAudio()
        {
            if (audioSource != null) audioSource.Stop();
        }

        public void PlayAudio()
        {
            if (audioSource != null) audioSource.Play();
        }
    }
}