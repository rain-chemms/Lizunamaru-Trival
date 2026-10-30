using UnityEngine;
using System.Collections;
using BonusSystem.BonusFuncs;
using System.Collections.Generic;
using CardSystem;
using RoleChoiceUISystem;

namespace BonusSystem.BonusFuncs
{
    //卡牌奖励生效器
    public class CardBonus : Bonus
    {
        //出现在筛选器内的稀有度不会生成
        [SerializeField] private List<CardRarity> rariteFilter;//卡牌稀有度筛选器
        public List<CardRarity> RariteFilter { get => rariteFilter; }
        [SerializeField] private  uint generateCount;//生成数量
        public uint GenerateCount { get => generateCount; set => generateCount = value; }
        [SerializeField] private uint choiceCount;//选择数量
        public uint ChoiceCount { get => choiceCount; set => choiceCount = value; }
        
        public override IEnumerator OnEffective()
        {
            EffectiveSuccess = true;
            yield return null;
        }

        public IEnumerator UseFilterToGenerateBonus()
        {
            //获取当前角色可以获取的卡池中的卡牌
            RoleChoiceData roleData = BattleMessage.instance?.GetRoleChoiceData();
            List<string> cardPool = roleData?.UseCardPool;
            //使用随机种子随机选取卡牌
            

            yield return null;
        }
    }
}