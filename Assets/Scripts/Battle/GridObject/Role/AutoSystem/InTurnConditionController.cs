using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    //该脚本用于控制自动角色功能的条件转换
    [RequireComponent(typeof(InTurnAutoFuntioner))]
    public class InTurnConditionController : MonoBehaviour
    {
        [Serializable]
        internal struct ConditionStatement
        {
            //血量控制类
            public bool openBloodCheck;//是否开启血量检测
            [ShowIfBool("openBloodCheck")] [SerializeField] public float belowPrecent;//低于血量百分比时触发
            //角色数量检测控制类
            public bool openRoleNumberCheck;//是否开启角色数量
            [ShowIfBool("openRoleNumberCheck")] [SerializeField] public int checkNumber;//检测数量
            [ShowIfBool("openRoleNumberCheck")] [SerializeField] public bool checkSelfSide;//检测己方还是对方的角色
            [ShowIfBool("openRoleNumberCheck")] [SerializeField] public bool uptoOrBelow;//少于还是多于时有效
            public bool CheckStatement(Role role)
            {
                if(role == null) return false;
                bool bloodOk = true;
                if(openBloodCheck)
                {
                    float nowPrecent = role.GetHp() / (role.GetMaxHp()+0.001f);
                    if(nowPrecent > belowPrecent) bloodOk = false;
                }
                bool roleNumberOk = true;
                if(openRoleNumberCheck)
                {
                    bool side = (bool)role?.GetSide();
                    side = checkSelfSide ? side : !side; 
                    int num = (int)BattleMessage.instance?.GetRoleList_Copy()?.Where(x => x.GetSide() == side).ToList()?.Count;
                    if(uptoOrBelow)
                    {
                        if(num < checkNumber) roleNumberOk = false;
                    }
                    else
                    {
                        if(num > checkNumber) roleNumberOk = false;
                    }
                }
                return bloodOk && roleNumberOk;
            }
        }

        [Serializable]
        internal struct ConditionChecker//条件检测器
        {
            public ConditionStatement state;//条件声明器
            public List<InTurnAutoAction> activateActionList;//条件检测成功后的行为列表
        }
        
        //类的主体功能如下
        [SerializeField] private InTurnAutoFuntioner autoFuntioner;
        void OnEnable()
        {
            if(autoFuntioner == null) autoFuntioner = GetComponent<InTurnAutoFuntioner>();
            //重置激活字典
            haveCheckedDict.Clear();
            foreach(ConditionChecker condition in conditions)
            {
                haveCheckedDict.Add(condition,false);
            }
        }
        
        [SerializeField] private List<ConditionChecker> conditions = new List<ConditionChecker>();
        [NonSerialized] private SerializableDictionary<ConditionChecker,bool> haveCheckedDict = new SerializableDictionary<ConditionChecker,bool>();
        public IEnumerator CheckAndShiftCondition()
        {
            if(autoFuntioner == null) yield break;
            Role role = autoFuntioner?.GetRole();
            if(role == null) yield break;
            foreach (ConditionChecker condition in conditions)
            {
                List<InTurnAutoAction> actList = condition.activateActionList;
                bool haveChecked = true;
                haveCheckedDict.TryGetValue(condition,out haveChecked);
                List<InTurnAutoAction> currentActionList = autoFuntioner.GetActionList_Copy();    
                if(condition.state.CheckStatement(role) && !haveChecked)//检测条件成立且当前行为列表不是这个列表时
                {
                    //切换当前的行为列表
                    autoFuntioner.SetActionList(actList);//设置行为列表
                    autoFuntioner.SetIndex(0);//索引归零
                    //设置当前行为已激活
                    haveCheckedDict[condition] = true;
                    break;
                }
            }
            yield return null;
        }
    }
}
