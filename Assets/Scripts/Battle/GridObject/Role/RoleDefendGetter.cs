using System.Collections;
using UnityEngine;
using System.Linq;
using GlobalSystem;
namespace GridObjectSystem.RoleSystem
{
    [RequireComponent(typeof(Role))]
    [RequireComponent(typeof(AnimTrigger))]
    public class RoleDefendGetter : MonoBehaviour
    {
        [SerializeField] private Role role;
        [SerializeField] private AnimTrigger animTrigger;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            //尝试自动获取
            if (role == null) role = GetComponent<Role>();
            if (animTrigger == null) animTrigger = GetComponent<AnimTrigger>();
            if(shieldVfx == null)
            {
                foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>().ToList())
                {
                    if(p == null) continue;
                    if(p.name.Equals("ShieldVfx"))//用名字进行默认匹配
                    {
                        shieldVfx = p;
                    }
                }
            }
        }

        IEnumerator Start()
        {
            yield return CheckDefendPointAndControlVfx();
        }

        [SerializeField] private ParticleSystem shieldVfx;
        //检测玩家格挡点数触发护盾特效
        private IEnumerator CheckDefendPointAndControlVfx()
        {
            if(shieldVfx == null) yield break;
            if(role == null) yield break;
            if(role?.GetDefend() > 0)
            {
                shieldVfx?.Play();
            }
            else
            {
                shieldVfx?.Stop();
            }
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
            if (role == null) yield break;
            string animName = "Defend";
            if (defendPoint > 0)//获取格挡值
            {
                Debug.Log("[RoleDefendGetter]: Role:"+ role?.name +" Get Defend Point: " + Mathf.Abs(defendPoint).ToString());
                role.SetDefend(role.GetDefend() + (uint)defendPoint);//格挡值增加
                animName = "Defend";
            }
            else if (defendPoint < 0)
            {
                Debug.Log("[RoleDefendGetter]: Role:"+ role?.name +" Lose Defend Point: " + Mathf.Abs(defendPoint).ToString());
                int temp = (int)role.GetDefend() - Mathf.Abs(defendPoint);
                //格挡值最低为0
                if (temp < 0) role.SetDefend(0);//格挡值减少
                else role.SetDefend((uint)temp);
                animName = "Skill";
            }
            else yield break;//0时返回
            yield return CheckDefendPointAndControlVfx();
            role?.GetComponent<AnimTrigger>()?.TriggerAnim(animName);
            yield return null;
            AnimatorStateInfo info = (AnimatorStateInfo)role?.GetComponent<AnimTrigger>()?.GetAnimator().GetCurrentAnimatorStateInfo(0);
            if (info.speed != 0.0f) yield return info.length * (float)info.normalizedTime;
            else yield return null;
        }
    }
}