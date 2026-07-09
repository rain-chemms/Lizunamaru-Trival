using UnityEngine;

[RequireComponent(typeof(Role))]
[RequireComponent(typeof(RoleAnimTrigger))]
public class RoleDamageGetter : MonoBehaviour
{
    [SerializeField] private Role role;
    [SerializeField] private RoleAnimTrigger animTrigger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(role == null) role = GetComponent<Role>();
        if(animTrigger == null) animTrigger = GetComponent<RoleAnimTrigger>();
    }

    //下面的函数中存放角色收到伤害时的逻辑
    /*
        逻辑如下:
            1.角色的defend值会在每回合开始时清空,每1点defend可以格挡一次伤害
                checkDefend:是否检查并计算defend值,默认为true
    */
    public void GetDamage(float damage,bool checkDefend = true)
    {
        if(role == null) return ;
        if(damage <= 0) return ;//伤害小于等于0时返回
        //拥有防御点数时
        if(role.GetDefend() > 0 && checkDefend)
        {
            role.SetDefend(role.GetDefend() - 1);
            if(animTrigger != null) animTrigger.TriggerAnim("Defend");
            return;
        }
        //无防御点数或不检查防御值时直接受到伤害
        role.SetHp(role.GetHp() - damage);
        if(animTrigger != null) animTrigger.TriggerAnim("Behit");
    }
}
