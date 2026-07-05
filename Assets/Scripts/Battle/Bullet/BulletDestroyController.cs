using UnityEngine;

[RequireComponent(typeof(Bullet))]
[RequireComponent(typeof(BulletDamageTriggger))]
[RequireComponent(typeof(Animator))]
public class BulletDestroyController : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    [SerializeField] private Animator animator;
    [SerializeField] private BulletDamageTriggger damageTriggger;
    [SerializeField] private bool animatorControl = false;// 是否由动画控制销毁
    void Start()
    {
        //尝试自动获取
        if (bullet == null) bullet = GetComponent<Bullet>();
        if (animator == null) animator = GetComponent<Animator>();
        if (damageTriggger == null) damageTriggger = GetComponent<BulletDamageTriggger>();
        haveTriggered = false;//设置为未触发
    }
    // Update is called once per frame
    void Update()
    {
        CheckOverLifeTimeDestroy();
        CheckPierceOverDestroy();
    }

    private bool haveTriggered = false;
    private void CheckOverLifeTimeDestroy()
    {
        if (bullet == null) return;
        if (haveTriggered) return;//已经触发过则不再触发
        //触发销毁
        if (bullet.GetLifeTime() > 0.0f && bullet.GetLifeRecorder() >= bullet.GetLifeTime())//小于等于0.0f代表不会自动销毁
        {
            if (animatorControl)//动画器控制的条件下
            {
                animator?.SetTrigger("Destroy");//触发动画器
            }
            else//非动画器控制的条件下
            {
                //超过生命周期直接销毁子弹
                Destroy(bullet.gameObject);
            }
            haveTriggered = true;
        }
    }

    private void CheckPierceOverDestroy()//检查穿透数目的结束
    {
        if((bool)damageTriggger?.GetLaserMode()) return;//激光模式下不检查
        if (haveTriggered) return;//已经触发过则不再触发
        if (bullet.GetPierce() < 0)
        {
            if (animatorControl)//动画器控制的条件下
            {
                animator?.SetTrigger("Destroy");//触发动画器
            }
            else//非动画器控制的条件下
            {
                //超过生命周期直接销毁子弹
                Destroy(bullet.gameObject);
            }
            haveTriggered = true;
        }
    }
}
