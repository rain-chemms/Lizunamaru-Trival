using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BulletSystem;
using GridObjectSystem.GadgetSystem;
using System.Linq;
using VfxDisplaySystem;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "Scriptable Objects/RandInTurnAutoAction")]
    public class RandInTurnAutoAction : InTurnAutoAction
    {
        //依据种子和回合数随机选择一个InturnAction执行
        [Header("随机选择列表")]
        [SerializeField] private List<InTurnAutoAction> randActions = new List<InTurnAutoAction>();
        public override IEnumerator ActionExcute(Role role)
        {
            int seed = (int)SeedSetter.instance?.GetSeed_Int() + (int)BattleMessage.instance?.GetRound();
            System.Random random = new System.Random(seed);//创建一个随机数生成器
            int index = random.Next(randActions.Count);//随机获取一个索引
            yield return randActions[index].ActionExcute(role);
        }
    }
}