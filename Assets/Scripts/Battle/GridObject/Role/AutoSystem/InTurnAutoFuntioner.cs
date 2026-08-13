using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    //该脚本用于在敌人AI自动在回合内执行相关的功能
    [RequireComponent(typeof(Role))]
    public class InTurnAutoFuntioner : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private float waitTime = 1.5f;//给予玩家的反应时间
        public float GetWaitTime() => waitTime;
        public void SetWaitTime(float newWaitTime) => waitTime = newWaitTime;
        [SerializeField] private int index = 0;
        public void SetIndex(int idx) => index = idx;
        public int GetIndex() => index;
        [SerializeField] private List<InTurnAutoAction> actionList;
        public List<InTurnAutoAction> GetActionList() => actionList;
        public List<InTurnAutoAction> GetActionList_Copy() => new List<InTurnAutoAction>(actionList);
        public void SetActionList(List<InTurnAutoAction> newActionList) => actionList = newActionList;
        [SerializeField] private Role role = null;
        public Role GetRole() => role;
        void Start()
        {
            if (role == null) role = GetComponent<Role>();
        }

        void OnEnable()
        {
            index = 0;//初始化索引
        }

        //用于在切换到相应Role回合是调用
        public IEnumerator Excute()
        {
            yield return new WaitForSeconds(waitTime);//延迟执行
            if (actionList != null && actionList.Count > 0)
            {
                InTurnAutoAction action = actionList?[index > actionList.Count - 1 ? actionList.Count - 1 : index < 0 ? 0 : index];
                if(action != null)
                {
                    yield return action?.ActionExcute(role);
                    if ((bool)action?.IsJumpLogicOpen())//若跳转逻辑开启
                    {
                        index = (int)action?.GetNextLogicIndex();//索引跳转
                    }
                    else index = ++index % actionList.Count;//不跳转则索引+1
                }
            }
            role?.SetRoundOperateEnd(true);//设置当前角色的回合结束
        }
    }
}