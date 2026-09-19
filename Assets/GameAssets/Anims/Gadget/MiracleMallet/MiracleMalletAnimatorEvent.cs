using UnityEngine;
using BulletSystem;
using GridObjectSystem.GadgetSystem;
using System.Linq;
using System.Collections;
using System;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Gadget))]
    public class MiracleMalletAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private AudioSource malletVoice;
        public AudioSource GetMalletVoice() => malletVoice;

        [SerializeField] private AudioSource whipVoice;
        public AudioSource GetWhipVoice() => whipVoice;

        [SerializeField] private Gadget gadget;
        void OnEnable()
        {
            if(gadget == null) gadget = GetComponent<Gadget>();
            if(malletVoice == null)
            {
                malletVoice = GetComponentsInChildren<AudioSource>().Where(x => x.name.Equals("MalletVoice")).FirstOrDefault();
            }
            if(whipVoice == null)
            {
                whipVoice = GetComponentsInChildren<AudioSource>().Where(x => x.name.Equals("WhipVoice")).FirstOrDefault();
            }
        }

        private void GenerateMalletVoice()
        {
            malletVoice?.Play();
        }

        private void GenerateWhipVoice()
        {
            whipVoice?.Play();
        }

        [SerializeField] private float waitTimeAfterOverToDestroy = 1.0f;
        private void DestroyTheGadget()
        {
            StartCoroutine(DestroyGd());
        }

        private IEnumerator DestroyGd()
        {
            yield return new WaitForSeconds(waitTimeAfterOverToDestroy);
            Destroy(gadget.gameObject);
        }
    }
}