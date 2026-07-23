using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Role role = null;
    void Start()
    {
        if(role == null) role = GetComponent<Role>();
    }

    void OnEnable()
    {
        index = 0;//初始化索引
    }

    //用于在切换到相应Role回合是调用
    public IEnumerator Excute()
    {
        yield return new WaitForSeconds(waitTime);//延迟执行
        yield return actionList[index]?.ActionExcute(role);
        index = ++index % actionList.Count;//索引改变
        role?.SetRoundOperateEnd(true);//设置当前角色的回合结束
    }
}
