using UnityEngine;

[RequireComponent(typeof(Role))]
[RequireComponent(typeof(RoleAnimTrigger))]
public class RoleDefendGetter : MonoBehaviour
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

    //下面的函数中存放角色获取护盾时的逻辑
    /*
        逻辑如下:
            1.角色的defend值会在每回合开始时清空,每1点defend可以格挡一次伤害
                defendPoint为负数时,表示格挡值减少
    */
    public void GetDefend(int defendPoint)
    {
        if(role == null) return ;
        if(defendPoint > 0)//获取格挡值
        {
            role.SetDefend(role.GetDefend() + (uint)defendPoint);//格挡值增加
            if(animTrigger != null) animTrigger.TriggerAnim("Defend");//触发格挡动画  
        }
        else if(defendPoint < 0)
        {
            int temp = (int)role.GetDefend() - defendPoint;
            //格挡值最低为0
            if(temp < 0) role.SetDefend(0);//格挡值减少
            else role.SetDefend((uint)temp);
            if(animTrigger != null) animTrigger.TriggerAnim("Skill");
        }
        else return;//0时返回
    }
}
