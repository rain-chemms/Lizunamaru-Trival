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
        Vector2Int index = (Vector2Int)role?.GetGridIndex();
        //依据棋盘信息计算目标位置
        BattleBoard board = BattleBoard.instance;
        Vector2 _gaps = (Vector2)board?.GetGapsOfGrid();
        
        target =
            board.transform.position +
            board.GetGrid00LocalPosition() +
            new Vector3(_gaps.x * index.x, heightOffset, _gaps.y * index.y);
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
