using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//战斗装饰地形大小修改器,依据BattleBoard的信息修改Terrian的大小
[RequireComponent(typeof(BattleBoard))]
public class BattleTerrianSizeChanger : MonoBehaviour
{
    [SerializeField] private List<BattleTerrain> battleTerrainList;
    [SerializeField] private float upDownHeight = 5.0f;///上下方向Terrain块的默认高度 
    public float GetUpDownHeight()
    {
        return upDownHeight;
    }
    public void SetUpDownHeight(float height)
    {
        upDownHeight = height;
    }
    [SerializeField] private float leftRightWidth = 5.0f;//左右方向Terrain块的默认宽度
    [SerializeField] private float positionOffsetHeight = 0.0f;//棋盘装饰地块的偏移高度
    public void SetPositionOffsetHeight(float offsetHeight)
    {
        positionOffsetHeight = offsetHeight;
    }
    public float GetPositionOffsetHeight()
    {
        return positionOffsetHeight;
    }
    [Header("设置地形时的高度值")]
    [SerializeField] private float terrainSetHeight;//设置地形时的高度值
    public float GetTerrainSetHeight()
    {
        return terrainSetHeight;
    }

    public void SetTerrainSetHeight(float setHeight)
    {
        terrainSetHeight = setHeight;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //获取所有BoardTerrian地形子节点
        battleTerrainList = GetComponentsInChildren<BattleTerrain>().ToList();
        SetBoardParentAsBoard();//设置所有BoardTerrian的父节点为当前Board
    }

    void Update()
    {
        ChangeTheTerrainSizeByBoard();//修改BattleTerrain的尺寸
        ChangeTheTerrainPositionByBoard();//修改BattleTerrain的位置
    }

    //将地形的父物体设置为棋盘
    public void SetBoardParentAsBoard()
    {
        BattleBoard btb = BattleBoard.instance;
        if (btb == null) return;
        if (battleTerrainList == null) return;
        foreach (BattleTerrain battleTerrain in battleTerrainList)
        {
            if (battleTerrain == null) continue;
            battleTerrain.transform.SetParent(btb.transform);
        }
    }

    //修改BattleTerrain的尺寸
    public void ChangeTheTerrainSizeByBoard()
    {
        BattleBoard btb = BattleBoard.instance;
        if (btb == null) return;
        if (battleTerrainList == null) return;
        Vector2 _00LocalPos = btb.GetGrid00LocalPosition();//获取BattleBoard的00格子位置
        Vector2Int _wh = btb.GetWidthAndHeight();//获取BattleBoard的宽高
        Vector2 _gaps = btb.GetGapsOfGrid();//获取格子之间的间隔

        foreach (BattleTerrain battleTerrain in battleTerrainList)
        {
            Terrain terrain = battleTerrain.GetTerrain();
            if (terrain == null) return;
            switch (battleTerrain.GetDirection())
            {
                case BattleDirection.LEFT:
                case BattleDirection.RIGHT:
                    float height_self = _gaps.y * _wh.y + upDownHeight * 2;
                    terrain.terrainData.size = new Vector3(
                        leftRightWidth,
                        terrainSetHeight,
                        height_self
                    );
                    break;
                case BattleDirection.UP:
                case BattleDirection.DOWN:
                default:
                    float width_self = _gaps.x * _wh.x;
                    terrain.terrainData.size = new Vector3(
                        width_self,
                        terrainSetHeight,
                        upDownHeight
                    );
                    break;
            }
            /*
            //设置地形的的坐标原点位置到中心
            //计算偏移量
            float offset_x = -terrain.terrainData.size.x / 2;
            float offset_z = -terrain.terrainData.size.z / 2;
            //应用偏移量
            terrain.transform.position = new Vector3(offset_x, 0f, offset_z);
            */
        }
    }
    
    //依据BattleBoard的位置
    public void ChangeTheTerrainPositionByBoard()
    {
        BattleBoard btb = BattleBoard.instance;
        if (btb == null) return;
        if (battleTerrainList == null) return;
        Vector2 _00LocalPos = btb.GetGrid00LocalPosition();//获取BattleBoard的00格子位置
        Vector2Int _wh = btb.GetWidthAndHeight();//获取BattleBoard的宽高
        Vector2 _gaps = btb.GetGapsOfGrid();//获取格子之间的间隔
        //依据方向改变battleTerrain的位置
        foreach (BattleTerrain battleTerrain in battleTerrainList)
        {
            Transform tf = battleTerrain.GetComponent<Transform>();
            Vector2 nowPos = new Vector3(0, 0);
            /*参考表:

                Down/Up方向:
                Down-Z:_00Pos.y - _gaps.y/2 - upDownHeight/2
                Up-Z:_00Pos.y + ((float)_wh.y-1) * _gaps.y + _gaps.y/2 + upDownHeight/2
                Down-X/Up-X:_00Pos.x + ((float)_wh.x-1)/2 * (_gaps.x)

                Left/Right方向:
                Left-X:_00Pos.x - _gaps.x/2 - leftRightWidth/2
                Right-X:_00Pos.x + ((float)_wh.x-1) * _gaps.x + _gaps.x/2 + leftRightWidth/2
                Left-Z/Right-Z:_00Pos.y + ((float)_wh.y-1)/2 * (_gaps.y)

            */
            //直接加的索引的值不乘棋盘的宽高,他妈的的你(我自己)也是个人物
            switch (battleTerrain.GetDirection())
            {
                case BattleDirection.DOWN:
                    nowPos = new Vector2(
                        _00LocalPos.x + ((float)_wh.x - 1) / 2 * _gaps.x,
                        _00LocalPos.y - _gaps.y / 2 - upDownHeight / 2
                    );
                    break;
                case BattleDirection.LEFT:
                    nowPos = new Vector2(
                        _00LocalPos.x - _gaps.x / 2 - leftRightWidth / 2,
                        _00LocalPos.y + ((float)_wh.y - 1) / 2 * _gaps.y
                    );
                    break;
                case BattleDirection.RIGHT:
                    nowPos = new Vector2(
                        _00LocalPos.x + ((float)_wh.x - 1) * _gaps.x + _gaps.x / 2 + leftRightWidth / 2,
                        _00LocalPos.y + ((float)_wh.y - 1) / 2 * _gaps.y
                    );
                    break;
                case BattleDirection.UP:
                default:
                    nowPos = new Vector2(
                        _00LocalPos.x + ((float)_wh.x - 1) / 2 * _gaps.x,
                        _00LocalPos.y + ((float)_wh.y - 1) * _gaps.y + _gaps.y / 2 + upDownHeight / 2
                    );
                    break;
            }
            //计算坐标相对于左下角的偏移量
            Terrain terrain = battleTerrain.GetTerrain();
            if (terrain == null) return;
            float offset_x = -terrain.terrainData.size.x / 2;
            float offset_z = -terrain.terrainData.size.z / 2;
            // 计算位置
            if (tf != null)
            {
                tf.localPosition = new Vector3(
                    nowPos.x + offset_x, 
                    _00LocalPos.y,
                    nowPos.y + offset_z
                );
            }
        }
    }
}
