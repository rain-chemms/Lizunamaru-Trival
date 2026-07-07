using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Card))]
public class CardVoiceController : MonoBehaviour
{
    [SerializeField] private Card card;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(card == null) card = GetComponent<Card>();
        //尝试获取所有的音频源
        List<AudioSource> aS = GetComponents<AudioSource>().ToList();
        foreach (AudioSource a in aS)
        {
            if(a == null) continue;
            if(audioSources.Contains(a)) continue;
            audioSources.Add(a);
        }
    }
    [SerializeField] private List<AudioSource> audioSources;
    public void AddAudioSource(AudioSource audioSource)
    {
        audioSources.Add(audioSource);
    }
    public List<AudioSource> GetAudioSources()
    {
        return audioSources;
    }
    public List<AudioSource> GetAudioSources_Copy()
    {
        return audioSources.ToList();
    }
    public void PlayCardVoice(string voiceName)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if(audioSource == null) continue;
            if(audioSource.name.Equals(voiceName))
            {
                audioSource.Play();
            }
        }
    }
}
