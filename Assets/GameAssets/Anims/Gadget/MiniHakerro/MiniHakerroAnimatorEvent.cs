using UnityEngine;

public class MiniHakerroAnimatorEvent : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    void OnEnable()
    {
        if(audioSource == null) audioSource = GetComponent<AudioSource>(); 
    }

    public void PlayShootAudio()
    {
        audioSource?.Play();
    }
}
