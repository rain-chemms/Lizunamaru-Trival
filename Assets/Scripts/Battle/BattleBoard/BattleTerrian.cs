using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//战斗装饰地形大小修改器,依据BattleBoard的信息修改Terrian的大小
[RequireComponent(typeof(Terrain))]
//BattleTerrain默认位置为Terrain的中心
public class BattleTerrain : MonoBehaviour
{
    //代表装饰Terrain的所处方向
    [SerializeField] private BattleDirection direction;
    public BattleDirection GetDirection()
    {
        return direction;
    }
    public void SetDirection(BattleDirection direction)
    {
        this.direction = direction;
    }
    [SerializeField] private Terrain terrain;
    public Terrain GetTerrain()
    {
        return terrain;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(terrain==null) terrain = GetComponent<Terrain>();
    }
}
