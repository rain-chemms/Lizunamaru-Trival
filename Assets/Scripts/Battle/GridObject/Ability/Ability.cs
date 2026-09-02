using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GridObjectSystem.AbilitySystem
{
    //能力对象的基类,代表一种能力
    //一个GridObject对象的同一种能力只能有一种
    public class Ability : IAbilityFunctioner
    {
        public Ability()
        {
            abilityName = GetType().Name;//能力名称默认就是类名
        }

        private bool canStack = true;//能否叠加,当前字段为负数时,当前能力最多只能有一层
        public bool CanStack
        {
            get => canStack; 
            set => canStack = value; 
        }
        private bool canNegative = true;//能否为负数
        public bool CanNegative
        {
            get => canNegative; 
            set => canNegative = value; 
        }
        private string abilityName = "";//能力名称
        public string AbilityName
        {
            get => abilityName; 
            set => abilityName = value; 
        }
        
        public virtual IEnumerator AfterRoundEnd(GridObject effectObject = null)
        {
            yield return null;
        }

        public virtual IEnumerator AfterRoundStart(GridObject effectObject = null)
        {
            yield return null;
        }
        public virtual IEnumerator AfterAbilityAmountChanged(GridObject effectObject = null)
        {
            yield return null;
        }
        public virtual IEnumerator AfterACardPlayed(GridObject effectObject = null)
        {
            yield return null;
        }
        public virtual IEnumerator AfterAbilityRemoved(GridObject effectObject = null)
        {
            yield return null;
        }
        public virtual IEnumerator AfterAbilityAdded(GridObject effectObject = null)
        {
            yield return null;
        }
    }
}