using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CineShakeSourceController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource source;
    [SerializeField] private bool isOpen = false;//当前是否处于开启状态
    [SerializeField] private float interval = 2.0f;//两次震动间的间隔
    void Start()
    {
        //尝试自动获取
        if(source == null) source = GetComponent<CinemachineImpulseSource>();
    }



    private float timeRocorder = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {
            timeRocorder += Time.deltaTime;
            if(timeRocorder >= interval)
            {
                GenerateOneShake();    
                timeRocorder = 0.0f;
            }
        }
    }

    public void GenerateOneShake()
    {
        if(source!=null)
        {
            source.GenerateImpulse();
        }
    }
}
