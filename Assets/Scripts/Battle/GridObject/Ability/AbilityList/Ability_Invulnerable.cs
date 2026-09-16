using System.Collections;
using UnityEngine;
using GridObjectSystem.RoleSystem.PlayerSystem;
using GridObjectSystem.RoleSystem;

namespace GridObjectSystem.AbilitySystem.AllAbilities
{
    //能力->无敌: 当前玩家受到的所有伤害变为0
    public class Ability_Invulnerable : Ability
    {
        public Ability_Invulnerable() : base()
        {
            AbilityName = "Ability_Invulnerable";//能力名称
            canStack = true;//能力可以叠加
            canNegative = true;//能力不能小于0
            isDebuff = false;//能力不是debuff
        }

        public void SetDamageToZero(ref float damage,ref bool checkDefend)
        {
            damage = 0.0f;
        }

        public override IEnumerator AfterAbilityAdded(GridObject effectObject = null)//能力被添加后的效果
        {
            effectObject.GetComponent<RoleDamageGetter>().onPreProductData += SetDamageToZero;
            yield return base.AfterAbilityAdded(effectObject);
        }

        public override IEnumerator AfterAbilityRemoved(GridObject effectObject = null)
        {
            effectObject.GetComponent<RoleDamageGetter>().onPreProductData -= SetDamageToZero;
            return base.AfterAbilityRemoved(effectObject);
        }

        public override IEnumerator AfterRoundEnd(GridObject effectObject = null)
        {
            //if(BattleMessage.instance?.IsPlayerTurn() != effectObject?.GetSide())
            yield return effectObject?.AddAbility<Ability_Invulnerable>(-1);//自身回合结束的时候减少一层
            yield return base.AfterRoundEnd(effectObject);//运行父类方法
        }
    }
}