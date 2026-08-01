using UnityEngine;

namespace AnimatorEventSystem
{
    public class SolemnLamentAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private AudioSource shootAudio;//音频源
        
        void OnEnable()
        {
            if(shootAudio == null) shootAudio = GetComponent<AudioSource>();
        }

        public void PlayShootAudio()
        {
            shootAudio?.Play();
        }
    }
}