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
    //该脚本规定了AI操控的Role在一个回合内执行的行为
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "InTurnAutoAction/InTurnAutoAction")]
    public class InTurnAutoAction : ScriptableObject
    {
        [SerializeField] private List<TurnAction> turnActions = new List<TurnAction>();
        public List<TurnAction> GetActionList() => turnActions;
        public List<TurnAction> GetActionList_Copy() => turnActions.ToList();
        //一下部分用于扩展重写和循环跳转的实现
        [Header("是否开启InTurnAutoAction跳转逻辑")]
        [SerializeField] protected bool jumpLogicOpen = false;//是否开启跳转逻辑
        public bool IsJumpLogicOpen() => jumpLogicOpen;
        [SerializeField] protected int nextLogicIndex = -1;//下一个InTurnAutoAction的索引
        public virtual int GetNextLogicIndex() => nextLogicIndex;
        //专注于执行Role的行为,不对Role中的状态进行修改
        public virtual IEnumerator ActionExcute(Role role)
        {
            if (role == null) yield break;

            foreach(TurnAction action in turnActions)
            {
                yield return action?.Excute(role);
            }
        }
    }
}