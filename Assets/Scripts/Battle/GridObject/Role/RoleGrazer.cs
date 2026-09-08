using UnityEngine;
using BulletSystem;
using System.Collections.Generic;
using System.Linq;

namespace GridObjectSystem.RoleSystem
{
    [RequireComponent(typeof(Collider))]
    public class RoleGrazer : MonoBehaviour
    {
        [SerializeField] private Role effectRole;
        public void SetEffectRole(Role newRole) => effectRole = newRole;
        public Role GetEffectRole() => effectRole;
        
        [SerializeField] private ParticleSystem grazeVfx = null;
        public void SetGrazeVfx(ParticleSystem newVfx) => grazeVfx = newVfx;
        public ParticleSystem GetGrazeVfx() => grazeVfx;

        [SerializeField] private AudioSource grazeVoice = null;
        public void SetGrazeVoice(AudioSource newVoice) => grazeVoice = newVoice;
        public AudioSource GetGrazeVoice() => grazeVoice;
        void OnEnable()
        {
            //尝试从自身获取GrazeVoice
            if (grazeVoice == null) grazeVoice = GetComponent<AudioSource>();
            //尝试从父级获取Role
            if (effectRole == null) effectRole = GetComponentInParent<Role>();
            //尝试从父级的所有子集的粒子系统中获取名为GrazeVfx的粒子系统
            if (grazeVfx == null)
            {
                List<ParticleSystem> pl = transform.parent.GetComponentsInChildren<ParticleSystem>().ToList();
                foreach(ParticleSystem p in pl)
                {
                    if(p == null) continue;
                    if(p.name.Equals("GrazeVfx"))//用名字进行默认匹配
                    {
                        grazeVfx = p;
                        break;
                    }
                }
            }
        }

        [SerializeField] private float enterRecoverSpellPrecent = 0.02f;//每擦弹一次增加符卡点数百分比
        public void SetEnterRecoverSpellPrecent(float newPrecent) => enterRecoverSpellPrecent = newPrecent;
        public float GetEnterRecoverSpellPrecent() => enterRecoverSpellPrecent;

        [SerializeField] private float laserStateRecoverSpellPrecent = 0.015f;
        public void SetLaserStateRecoverSpellPrecent(float newPrecent) => laserStateRecoverSpellPrecent = newPrecent;
        public float GetLaserStateRecoverSpellPrecent() => laserStateRecoverSpellPrecent;

        //当子弹进入当前区域是,增加擦弹数和对应角色的符卡点数
        void OnTriggerEnter(Collider other)
        {
            //获取子弹
            if(effectRole == null) return ;
            Bullet bt = other.GetComponent<Bullet>();
            //只有子弹不为空且为控制角色的敌方的子弹才产生擦弹效果
            if(bt != null && effectRole.GetSide() != bt.GetSide())
            {
                //增加SpellPrecent
                float newSpellPrecent = effectRole.GetSpellPrecent() + enterRecoverSpellPrecent;
                effectRole.SetSpellPrecent(newSpellPrecent);
                //增加擦单数
                if(effectRole == BattleMessage.instance?.GetControlPlayer())//若擦弹的为当前控制的玩家
                {
                    //增加擦单数
                    BattleMessage.instance?.SetGrazeCount(BattleMessage.instance.GetGrazeCount() + 1);
                }
                //激活特效
                grazeVfx?.Play();
                grazeVoice?.Play();
            }
        }
        
        void OnTriggerStay(Collider other)
        {
            if(effectRole == null) return ;
            Bullet bt = other.GetComponent<Bullet>();
            //只有子弹不为空且为控制角色的敌方的子弹才产生擦弹效果
            if(bt != null && effectRole.GetSide() != bt.GetSide())
            {
                BulletDamageTrigger btTrigger = bt.GetComponent<BulletDamageTrigger>();
                if(btTrigger == null) return ;
                if(btTrigger.IsLaserMode())
                {
                    float newSpellPrecent = effectRole.GetSpellPrecent() + laserStateRecoverSpellPrecent;
                    effectRole.SetSpellPrecent(newSpellPrecent);
                    //这个就不必增加擦弹数了
                    grazeVfx?.Play();
                    grazeVoice?.Play();   
                }
            }
        }
    }
    
}
