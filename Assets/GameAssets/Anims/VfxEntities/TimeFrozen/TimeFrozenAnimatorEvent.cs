using UnityEngine;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Animator))]
    public class TimeFrozenAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private AudioSource clockVoice;
        public void PlayClockVoice()
        {
            clockVoice?.Play();
        }
    }
}
