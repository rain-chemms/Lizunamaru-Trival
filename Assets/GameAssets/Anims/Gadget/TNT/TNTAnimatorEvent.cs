using GridObjectSystem.GadgetSystem.Tnts;
using UnityEngine;

[RequireComponent(typeof(TNT))]
[RequireComponent(typeof(Animator))]
public class TNTAnimatorEvent : MonoBehaviour
{
    [SerializeField] private TNT tnt;
    void OnEnable()
    {
        if(tnt == null) tnt = GetComponent<TNT>();    
    }

    [SerializeField] private AudioSource explosionVoice;
    [SerializeField] private AudioSource willExplosionVoice;
    public void PlayTheExplosionVoice()
    {
        explosionVoice?.Play();
    }

    public void PlayWillExplosionVoice()
    {
        willExplosionVoice?.Play();
    }

    public void DestroyTheTNT()
    {
        Destroy(tnt?.gameObject);
    }
    [SerializeField] private ParticleSystem explosionParticle;
    public void TriggerExplosionPartcle()
    {
        explosionParticle?.Play();
    }
}

