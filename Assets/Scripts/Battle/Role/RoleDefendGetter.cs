using System.Collections;
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
    public IEnumerator GetOrLoseDefend(int defendPoint)
    {
        //只对数值产生变化,操纵动画器
        if(role == null) yield break;
        string animName = "Defend";
        if(defendPoint > 0)//获取格挡值
        {
            role.SetDefend(role.GetDefend() + (uint)defendPoint);//格挡值增加
            animName = "Defend";
        }
        else if(defendPoint < 0)
        {
            int temp = (int)role.GetDefend() - defendPoint;
            //格挡值最低为0
            if(temp < 0) role.SetDefend(0);//格挡值减少
            else role.SetDefend((uint)temp);
            animName = "Skill";
        }
        else yield break;//0时返回
        role?.GetComponent<RoleAnimTrigger>()?.TriggerAnim(animName);
        yield return null;
        AnimatorStateInfo info = (AnimatorStateInfo) role?.GetComponent<RoleAnimTrigger>()?.GetAnimator().GetCurrentAnimatorStateInfo(0);
        if(info.speed != 0.0f) yield return info.length * (float)info.normalizedTime;
        else yield return null;
    }
}
