using System;
using System.Drawing;
using UnityEngine;

//控制地图物理边界的脚本,某些子弹会检测并与这些边界产生对应的物理效果
[RequireComponent(typeof(BattleBoard))]
public class BattleBoardPhysicBoarderController : MonoBehaviour
{
    [SerializeField] private BattleBoard board;
    void OnEnable()
    {
        if(board == null) board = GetComponent<BattleBoard>();
    }

    void Start()
    {
        ResizeTheBoundaryByBoardSize();
    }

    [SerializeField] private BoxCollider upBoundary;
    public BoxCollider GetUpBoundary() => upBoundary;
    
    [SerializeField] private BoxCollider downBoundary;
    public BoxCollider GetDownBoundary() => downBoundary;

    [SerializeField] private BoxCollider rightBoundary;
    public BoxCollider GetRightBoundary() => rightBoundary;
    
    [SerializeField] private BoxCollider leftBoundary;
    public BoxCollider GetLeftBoundary() => leftBoundary;

    [SerializeField] private BoxCollider frontBoundary;
    public BoxCollider GetFrontBoundary() => frontBoundary;

    [SerializeField] private BoxCollider backBoundary;
    public BoxCollider GetBackBoundary() => backBoundary;

    [SerializeField] private float heightOffset = 0.0f;//距离00格子的偏移量
    [SerializeField] private float heightOfBoundary = 10.0f;//边界的最大高度
    public float GetHeightOfBoundary() => heightOfBoundary;
    public void SetHeightOfBoundary(float height) => heightOfBoundary = height;

    [SerializeField] private float thickOfBoundary = 0.01f;
    public float GetThickOfBoundary() => thickOfBoundary;
    public void SetThickOfBoundary(float th) => thickOfBoundary = th;

    private void ResizeTheBoundaryByBoardSize()
    {
        if(board == null) return;
        Vector2Int size = board.GetWidthAndHeight();
        Vector3 bodPos = board.transform.position;
        Vector3 _00Pos = board.GetGrid00LocalPosition();//获取00格子的世界坐标 
        Vector2 gaps = board.GetGapsOfGrid();
        //优先处理:Left & Right => Unity Axis: X
        //Left(左侧)
        Vector3 centerL = new Vector3(
            _00Pos.x - gaps.x / 2,//位于左侧边界
            heightOffset + heightOfBoundary / 2,//高度为中心处
            _00Pos.z - gaps.y / 2 + gaps.y * (size.y / 2.0f)//纵向Z方向为列中心处
        );
        //Right(右侧)
        Vector3 centerR = new Vector3(
            _00Pos.x - gaps.x / 2 + gaps.x * size.x,//位于右侧边界
            heightOffset + heightOfBoundary / 2,//高度为中心处
            _00Pos.z - gaps.y / 2 + gaps.y * (size.y / 2.0f)//纵向Z方向为列中心处
        );
        //计算规格大小
        Vector3 sizeLR = new Vector3(
            thickOfBoundary,//左侧的X方向为厚度
            heightOfBoundary,//高度为整体的高度
            size.y * gaps.y//Z长度为格子索引Y向的长度
        );
        //应用大小
        leftBoundary.size = sizeLR;
        leftBoundary.center = centerL;
        rightBoundary.size = sizeLR;
        rightBoundary.center = centerR;

        //之后是:Front & Back => Unity Axis: Z
        //Front(前侧)
        Vector3 centerF = new Vector3(
            _00Pos.x - gaps.x / 2 + gaps.x * (size.x / 2.0f),
            heightOffset + heightOfBoundary / 2,
            _00Pos.z - gaps.y / 2 + gaps.y * size.y 
        );
        //Back(后侧)
        Vector3 centerB = new Vector3(
            _00Pos.x - gaps.x / 2 + gaps.x * (size.x / 2.0f),
            heightOffset + heightOfBoundary / 2,
            _00Pos.z - gaps.y / 2
        );
        //计算规格大小
        Vector3 sizeFB = new Vector3(
            size.x * gaps.x,
            heightOfBoundary,
            thickOfBoundary
        );
        //应用大小
        frontBoundary.size = sizeFB;
        frontBoundary.center = centerF;
        backBoundary.size = sizeFB;
        backBoundary.center = centerB;
        //最后是:Up & Down => UNity Axis: Y
        //UP(上侧)
        Vector3 centerU = new Vector3(
            _00Pos.x - gaps.x / 2 + gaps.x * (size.x / 2.0f),
            heightOffset + heightOfBoundary,
            _00Pos.z - gaps.y / 2 + gaps.y * (size.y / 2.0f)
        );
        //Down(下侧)
        Vector3 centerD = new Vector3(
            _00Pos.x - gaps.x / 2 + gaps.x * (size.x / 2.0f),
            heightOffset,
            _00Pos.z - gaps.y / 2 + gaps.y * (size.y / 2.0f)
        );
        //计算规格大小
        Vector3 sizeUD = new Vector3(
            size.x * gaps.x,
            thickOfBoundary,
            size.y * gaps.y
        );

        upBoundary.size = sizeUD;
        upBoundary.center = centerU;
        downBoundary.size = sizeUD;
        downBoundary.center = centerD;
    }


}
