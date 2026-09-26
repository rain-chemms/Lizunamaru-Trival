using UnityEngine;
using GridObjectSystem.RoleSystem;
using System.Collections.Generic;
using System.Collections;
//用于显示玩家的当前位置点
//ConcentratePoint代表当前瞄准位置,是单例物体
//而这个脚本是每个角色都要有的,不是单例物体

public class RoleGridIndexDisplayer : MonoBehaviour
{
    [SerializeField] private Role role;//关联的玩家 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float lerpSpeed;//变换速度
    [SerializeField] private float heightOffset;//显示的高度
    void OnEnable()
    {
        if (role == null) role = GetComponentInParent<Role>();
        lastPos = (Vector2Int)role?.GetGridIndex();
        lastIsFly = (bool)role?.IsFly();
        lerpOver = true;
    }

    void Start()
    {
        StartCoroutine(FirstFrame());
    }

    private IEnumerator FirstFrame()
    {
        yield return null;          // 等到下一帧
        GetTargetPos();
        lerpOver = false;
    }

    private void SetPosInstantly()
    {
        GetTargetPos();
        transform.position = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckPlayerPosChanged())
        {
            lerpOver = false;
            GetTargetPos();
        }
        if (!lerpOver)
        {
            DoLerp();
        }
        //y方向的到小直接设置
        transform.position = new Vector3(
            transform.position.x,
            target.y,
            transform.position.z
        );
    }

    [SerializeField] private bool lerpOver = true;
    [SerializeField] private float minDistance = 0.01f;
    private void DoLerp()
    {
        Vector3 nowPos = transform.position;
        if (Vector3.Distance(nowPos, target) <= minDistance)
        {
            lerpOver = true;
            return;
        }
        transform.position = Vector3.Lerp(
            nowPos,
            target,
            lerpSpeed * Time.deltaTime
        );
    }

    [SerializeField] private Vector3 target = Vector3.zero;
    private void GetTargetPos()
    {
        if (role == null) return;
        Vector2Int index = role.GetGridIndex();
        if (index == null) return;
        //获取棋盘中所有格子
        List<BattleGrid> grids = BattleBoard.instance?.GetBattleGridList();
        if (grids == null) return;
        target =
            (Vector3)BattleBoard.instance?.GetGrid00LocalPosition() +
            (Vector3)BattleBoard.instance?.transform.position +
            new Vector3(0.0f, heightOffset, 0.0f);
        foreach (BattleGrid grid in grids)
        {
            if (grid == null) continue;//忽略空格
            if (grid.GetIndex().x == index.x && grid.GetIndex().y == index.y)
            {
                target = grid.transform.position + new Vector3(0.0f, heightOffset, 0.0f);
                break;//找到目标格子后跳出循环
            }
        }
    }

    [SerializeField] private Vector2Int lastPos = Vector2Int.zero;
    [SerializeField] private bool lastIsFly = false;
    private bool CheckPlayerPosChanged()
    {
        bool nowIsFly = (bool)role?.IsFly();
        Vector2Int nowIndex = (Vector2Int)role?.GetGridIndex();
        if (lastPos.x != nowIndex.x || lastPos.y != nowIndex.y || nowIsFly != lastIsFly)
        {
            lastIsFly = nowIsFly;
            lastPos.x = nowIndex.x;
            lastPos.y = nowIndex.y;
            return true;
        }
        else return false;
    }
}
