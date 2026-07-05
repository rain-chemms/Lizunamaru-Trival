using UnityEngine;

public class MasterSparkAnimatorEvent : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform masterSpark;
    public void OnMAsterSparkAnimaOver()//播放结束时候销毁特效物体
    {
        Destroy(masterSpark.gameObject);
    }

    public void StopAudio()
    {
        if(audioSource != null) audioSource.Stop();
    }

    public void PlayAudio()
    {
        if(audioSource != null) audioSource.Play();
    }
}
