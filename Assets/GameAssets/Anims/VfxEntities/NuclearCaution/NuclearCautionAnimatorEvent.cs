using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Animator))]
    public class NuclearCautionAnimatorEvent : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }
}