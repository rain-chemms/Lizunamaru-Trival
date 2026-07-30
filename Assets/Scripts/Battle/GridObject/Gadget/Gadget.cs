using UnityEngine;
using System.Collections;
using System;
using GridObjectSystem.RoleSystem.AutoSystem;
using GridObjectSystem.RoleSystem;

namespace GridObjectSystem.GadgetSystem
{
    //道具系统基础
    public class Gadget : GridObject,GadgetFunctioner
    {
        [SerializeField] private Role belongRole;//所属的玩家,可以为空
        public Role GetBelongRole() => belongRole;
        public void SetBelongRole(Role role) => belongRole = role;
        // Update is called once per frame
        protected virtual void Update()
        {
            base.Update();
        }
        [SerializeField] public Action effectAction;
        public Action GetEffectAction() => effectAction;
        //在道具生效的时候调用
        public virtual IEnumerator OnGadgetEffect()
        {
            GetComponent<AnimTrigger>()?.TriggerAnim("Effect");//尝试激活触发动画
            effectAction?.Invoke();//触发额外的委托
            yield return null;
        }
        [SerializeField] public Action roundStartAction;
        public Action GetRoundStartAction() => roundStartAction;
        //当自身回合结束时调用
        public virtual IEnumerator OnEveryRoundStart()
        {
            yield return null;
        }
        [SerializeField] public Action roundEndAction;
        public Action GetRoundEndAction() => roundEndAction;
        //当前自身回合开始时调用
        public virtual IEnumerator OnEveryRoundEnd()
        {
            yield return null;
        }
    }
}