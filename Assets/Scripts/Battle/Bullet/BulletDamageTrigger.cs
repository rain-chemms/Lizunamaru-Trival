using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class BulletDamageTriggger : MonoBehaviour
{
    /*
        激光模式下,造成的伤害按帧计算,每一帧造成一次伤害
    */
    [SerializeField] private bool laserMode = false;//激光模式
    public void SetLaserMode(bool mode)
    {
        laserMode = mode;
    }
    public bool GetLaserMode()
    {
        return laserMode;
    }
    [SerializeField] private Bullet bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if (bullet == null) bullet = GetComponent<Bullet>();
    }

    void OnTriggerEnter(Collider other)
    {
        //单次伤害再非激光模式下有效逻辑中只处理一次
        //只检测含Role标签的物体
        if (!laserMode)
        {
            if (other.gameObject.tag == "Role")
            {
                Role role = other.gameObject.GetComponent<Role>();
                RoleDamageGetter damageGetter = other.gameObject.GetComponent<RoleDamageGetter>();
                //伤害只对不同阵营的物体有效
                if (role?.GetSide() != bullet?.GetSide()) 
                {
                    damageGetter?.GetDamage(bullet.GetDamage());
                    bullet.SetPierce(bullet.GetPierce() - 1);//减穿透数
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (laserMode)
        {
            //只检测含Role标签的物体
            if (other.gameObject.tag == "Role")
            {
                Role role = other.gameObject.GetComponent<Role>();
                RoleDamageGetter damageGetter = other.gameObject.GetComponent<RoleDamageGetter>();
                //如果可以产生伤害
                if (role?.GetSide() != bullet?.GetSide())
                {
                    damageGetter?.GetDamage(bullet.GetDamage());
                }
            }
        }
    }
}
