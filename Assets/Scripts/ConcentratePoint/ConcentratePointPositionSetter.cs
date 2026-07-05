using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ConcentratePoint))]
public class ConcentratePointPositionSetter : MonoBehaviour
{
    [SerializeField] private ConcentratePoint concentratePoint;
    void Start()
    {
        if(concentratePoint == null) concentratePoint = GetComponent<ConcentratePoint>();
    }
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] private float heightOffset = 0.5f;//高度的偏移
    public float GetLerpSpeed()
    {
        return lerpSpeed;
    }
    public void SetLerpSpeed(float newLerpSpeed)
    {
        lerpSpeed = newLerpSpeed;
    }
    void Update()
    {
        GetTargetPosition();
        LerpToTargetPosition();
    }

    [SerializeField] private Vector3 target = Vector3.zero;
    private void GetTargetPosition()
    {
        if(concentratePoint == null) return;
        Vector2 index = concentratePoint.GetIndex();//获取索引
        if(index == null) return;
        //获取棋盘中所有格子
        List<BattleGrid> grids = BattleBoard.instance?.GetBattleGridList();
        if(grids == null) return;
        target = 
            (Vector3)BattleBoard.instance?.GetGrid00LocalPosition() + 
            (Vector3)BattleBoard.instance?.transform.position + 
            new Vector3(0.0f,heightOffset,0.0f);
        foreach(BattleGrid grid in grids)
        {
            if(grid == null) continue;//忽略空格
            if(grid.GetIndex().x == index.x && grid.GetIndex().y == index.y)
            {
                target = grid.transform.position + new Vector3(0.0f,heightOffset,0.0f);
                break;//找到目标格子后跳出循环
            }
        }
    }

    private void LerpToTargetPosition()
    {
        if(concentratePoint == null) return;
        concentratePoint.transform.position = Vector3.Lerp(
            concentratePoint.transform.position,
            target,
            lerpSpeed * Time.deltaTime
        );
    }

}