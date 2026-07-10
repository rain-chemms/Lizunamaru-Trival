using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class BulletAppender : MonoBehaviour
{
    [SerializeField] protected Bullet bullet;
    protected virtual void Start()
    {
        timeRocorder = 0.0f;
        //尝试自动获取
        if(bullet == null) bullet = GetComponent<Bullet>();
    }
    [SerializeField] protected float haltTime;//子弹创建后的停顿时间
    public float GetHaltTime()
    {
        return haltTime;
    }
    public void SetHaltTime(float haltTime)
    {
        this.haltTime = haltTime;
    }
    [SerializeField] protected bool openHalt = false;//是否开启停顿
    public bool IsOpenHalt()
    {
        return openHalt;
    }
    public void SetOpenHalt(bool openHalt)
    {
        this.openHalt = openHalt;
    }
    [SerializeField] protected float timeRocorder = 0.0f;
    protected virtual void Update()
    {
        timeRocorder += Time.deltaTime;
        if(timeRocorder >= haltTime)
        {
            /*
            if(!persistTrace)
            {
                if(!haveTrace)
                {
                    TrackTheEnermy();
                    haveTrace = true;
                }
            }
            else
            {
                TrackTheEnermy();
            }
            */
            AfterHaltFunctionPreFrame();
        }
    }

    protected virtual void AfterHaltFunctionPreFrame(){}
}