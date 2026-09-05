using UnityEngine;
using GridObjectSystem.RoleSystem;
using System;

namespace BulletSystem
{
    [RequireComponent(typeof(BulletDamageTrigger))]
    public class BulletHpRecover : MonoBehaviour
    {
        [SerializeField] private BulletDamageTrigger damageTrigger;

        void OnEnable()
        {
            if(damageTrigger == null) damageTrigger = GetComponent<BulletDamageTrigger>();
            if(damageTrigger!=null)
            {
                damageTrigger.EnterTrigger += TriggerTheRecover;
            }
        }

        void OnDisable()
        {
            if(damageTrigger!=null)
            {
                damageTrigger.EnterTrigger -= TriggerTheRecover;
            }
        }

        [SerializeField] private float hpRecoverPoint = 0.0f;
        public float GetRecoverPoint() => hpRecoverPoint;
        public void SetRecoverPoint(float point) => hpRecoverPoint = point;

        [SerializeField] private Role effectiveRole;
        public Role GetEffectiveRole() => effectiveRole;
        public void SetEffectiveRole(Role role) => effectiveRole = role;

        public void TriggerTheRecover()
        {
            if(effectiveRole == null) return;
            // 回血量不能小于0
            if(hpRecoverPoint <= 0.0f) return;
            //尝试使用RoleHealther回血
            RoleHealther roleHealther = effectiveRole.GetComponent<RoleHealther>();
            if(roleHealther != null)
            {
                roleHealther.GetHealth(hpRecoverPoint);
                return;//回血成功
            }
            //else
            //{
                //若无RoleHealther则使用默认计算血量回血
                Debug.Log("[BulletHpRecover]: 触发回血量: " + hpRecoverPoint + " HP, 回血角色: " + effectiveRole.name);
                float hp = (float)effectiveRole?.GetHp() + hpRecoverPoint;
                hp = hp > effectiveRole.GetMaxHp() ? effectiveRole.GetMaxHp() : hp;
                effectiveRole.SetHp(hp);//回血
            //}
        }

    }

}