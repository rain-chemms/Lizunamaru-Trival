using System.Linq;
using UnityEngine;
using GlobalSystem;
namespace GridObjectSystem.RoleSystem
{

    //角色生命恢复器,用于触发角色回血效果
    [RequireComponent(typeof(Role))]
    [RequireComponent(typeof(AnimTrigger))]
    public class RoleHealther : MonoBehaviour
    {    
        [SerializeField] private Role role;
        [SerializeField] private ParticleSystem recoverHpVfx;
        void OnEnable()
        {
            //尝试自动获取
            if (role == null) role = GetComponent<Role>();
            if (recoverHpVfx == null)
            {
                foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>().ToList())
                {
                    if(p == null) continue;
                    if(p.name.Equals("RecoverHpVfx"))//用名字进行默认匹配
                    {
                        recoverHpVfx = p;
                    }
                }
            }
        }
        //回血
        public void GetHealth(float recoverHp)
        {
            //触发角色回血
            if(role == null) return;
            if(recoverHp < 0) recoverHp = 0;
            float endHp = (float)role?.GetHp() + recoverHp;
            endHp = endHp > role.GetMaxHp() ? role.GetMaxHp() : endHp;
            role.SetHp(endHp);
            //触发回血特效
            recoverHpVfx?.Play();
        }
    }
    
}