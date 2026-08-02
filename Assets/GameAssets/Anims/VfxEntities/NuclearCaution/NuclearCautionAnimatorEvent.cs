using UnityEngine;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Animator))]
    public class NuclearCautionAnimatorEvent : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private AudioSource audioSource;
        void OnEnable()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }
        void PlayerVoice()
        {
            audioSource?.Play();
        }
    }
}