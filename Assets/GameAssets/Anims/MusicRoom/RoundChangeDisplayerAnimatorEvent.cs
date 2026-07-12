using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RoundChangeDisplayer))]
public class RoundChangeDisplayerAnimatorEvent : MonoBehaviour
{
    [SerializeField] private RoundChangeDisplayer roundChangeDisplayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(roundChangeDisplayer == null) roundChangeDisplayer = GetComponent<RoundChangeDisplayer>();
    }
    [SerializeField] private AudioSource audioSource;//播放音效
    public void PlayChangeVoice()
    {
        if(audioSource != null)
        {
            audioSource.Play();
        }
    }
}
