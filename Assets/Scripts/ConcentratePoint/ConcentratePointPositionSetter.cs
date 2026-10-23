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
        //依据棋盘信息计算目标位置
        BattleBoard board = BattleBoard.instance;
        Vector2 _gaps = (Vector2)board?.GetGapsOfGrid();
        target =
            board.transform.position +
            board.GetGrid00LocalPosition() +
            new Vector3(_gaps.x * index.x, heightOffset, _gaps.y * index.y);
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