using UnityEngine;

public class MasterSparkAnimatorEvent : MonoBehaviour
{
    [SerializeField] private Transform masterSpark;
    public void OnMAsterSparkAnimaOver()//播放结束时候销毁特效物体
    {
        Destroy(masterSpark.gameObject);
    }
}
