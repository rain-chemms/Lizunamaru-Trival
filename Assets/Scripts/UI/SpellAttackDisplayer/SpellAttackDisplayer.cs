using UnityEngine;
using System.Linq;
using System.Collections.Generic;


[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
public class SpellAttackDisplayer : MonoBehaviour
{
    public static SpellAttackDisplayer instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    public List<AudioSource> GetAudioSources()
    {
        return audioSources;
    }
    void Start()
    {
        //自动获取全部的音频源
        List<AudioSource> AS = GetComponentsInChildren<AudioSource>().ToList();
        foreach (AudioSource aS in AS)
        {
            if(aS == null) continue;
            if(audioSources.Contains(aS)) continue;
            audioSources.Add(aS);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
