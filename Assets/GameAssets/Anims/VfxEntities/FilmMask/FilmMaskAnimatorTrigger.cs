using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AnimatorEventSystem
{
    public class FilmMaskAnimatorTrigger : MonoBehaviour
    {
        [SerializeField] private List<AudioSource> audioList = new List<AudioSource>();
        void OnEnable()
        {
            foreach (AudioSource item in GetComponentsInChildren<AudioSource>().ToList())
            {
                if(item == null) continue;
                if(!audioList.Contains(item))
                {
                    audioList.Add(item);
                }   
            }
        }
        
        public void PlayerVoice()
        {
            foreach (AudioSource aS in audioList) aS?.Play();
        }

        public void PlayVoiceByIndex(int index)
        {
            if(audioList.Count > index)
            {
                audioList[index]?.Play();
            }
        }
    }
}