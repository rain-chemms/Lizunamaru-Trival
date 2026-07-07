using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(SpellAttackDisplayer))]
public class SpellActtackDisplayerAnimatorEvent : MonoBehaviour
{
    [SerializeField] private SpellAttackDisplayer displayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        if(displayer == null) displayer = GetComponent<SpellAttackDisplayer>();
    }

    //播放某一个符卡释放的音频    
    public void PlayTheSpellAttackVoice(string audioName)
    {
        if(displayer == null) return;
        foreach (AudioSource aS in displayer.GetAudioSources()?.ToList())
        {
            if(aS == null) continue;
            if (aS.name.Equals(audioName))
            {
                aS.Play();
            }
        }
    }
}
