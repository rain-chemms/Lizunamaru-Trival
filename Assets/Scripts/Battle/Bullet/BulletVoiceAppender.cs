using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BulletVoiceAppender : BulletAppender
{
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioSource> GetAudioSources()
    {
        return audioSources;
    }
    public List<AudioSource> GetAudioSources_Copy()
    {
        return audioSources.ToList();
    }
    protected override void Start()
    {
        base.Start();
        if(audioSources == null) audioSources = new List<AudioSource>();
        List<AudioSource> allAss = GetComponentsInChildren<AudioSource>().ToList();
        foreach (AudioSource aus in allAss)
        {
            if(aus == null) continue;
            if(audioSources.Contains(aus)) continue;
            audioSources.Add(aus);
        }
        havePlay = false;
    }

    protected override void Update()
    {
        base.Update();
    }
    [SerializeField] private List<string> playList = new List<string>();
    [SerializeField] private bool havePlay = false;
    protected override void AfterHaltFunctionPreFrame()
    {
        if(!havePlay)
        {
            if(audioSources == null) 
            {
                havePlay = true;
                return;
            }
            foreach(string str in playList)
            {
                foreach(AudioSource aus in audioSources)
                {
                    if(aus == null) continue;
                    if(aus.clip == null) continue;
                    if(aus.name.Equals(str))
                    {
                        aus.Play();
                        break;
                    }
                }
            }
            havePlay = true;
        }
    }
}