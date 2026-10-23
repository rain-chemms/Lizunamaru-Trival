using UnityEngine;
using System.Collections;
using GridObjectSystem.GadgetSystem;
using System.Collections.Generic;
using System.Linq;
using GridObjectSystem;
using GridObjectSystem.GadgetSystem.Tnts;

namespace CardSystem.AllCardHub
{
    public class Gadget_TNT : Card
    {
        //TNT道具的预制体
        [SerializeField] private TNT tntPrefab;
        public TNT GetTntPrefab() => tntPrefab;
        public TNT SetTntPrefab(TNT tnt) => tntPrefab = tnt;
        [SerializeField] private int clockNumber = 3;
        public int GetClockNumber() => clockNumber;
        public void SetClockNumber(int number) => clockNumber = number;
        [SerializeField] private float damage = 30;//TNT道具的伤害
        public float GetDamage() => damage;
        public void SetDamage(float damage) => this.damage = damage;
        
        [SerializeField] private const float initialHeight = 3.0f;
        public override IEnumerator AfterPlay()
        {
            //产生炸弹实体,设置初始位置阵营和伤害,并添加到游戏场景中
            TNT tnt = Instantiate(tntPrefab);
            tnt?.SetDamage(damage);//设置炸弹的伤害
            tnt?.SetClockNumber(clockNumber);//设置炸弹的计时器
            tnt?.SetSide((bool)BattleMessage.instance?.GetControlPlayer()?.GetSide());
            //获取ConcentratePoint的位置
            Vector2Int targetIndex = (Vector2Int)ConcentratePoint.instance?.GetIndex();
            
            //如果当前位置的格子为空,选取距离自身最近的非空位置安放
            BattleBoard board = BattleBoard.instance;
            List<Vector2Int> emptyGrids = board?.GetEmptyGridIndex();
            if (emptyGrids != null && emptyGrids.Contains(targetIndex))
            {
                Vector2Int boardSize = board.GetWidthAndHeight();
                int maxDist = Mathf.Max(boardSize.x, boardSize.y);
                for (int dist = 1; dist <= maxDist; dist++)
                {
                    bool found = false;
                    for (int dx = -dist; dx <= dist && !found; dx++)
                    {
                        int dy = dist - Mathf.Abs(dx);
                        foreach (int s in new int[] { 1, -1 })
                        {
                            if (dy == 0 && s == -1) continue;
                            Vector2Int candidate = targetIndex + new Vector2Int(dx, dy * s);
                            if (candidate.x >= 0 && candidate.x < boardSize.x &&
                                candidate.y >= 0 && candidate.y < boardSize.y &&
                                !emptyGrids.Contains(candidate))
                            {
                                targetIndex = candidate;
                                found = true;
                                break;
                            }
                        }
                    }
                    if (found) break;
                }
            }

            tnt?.SetGridIndex(targetIndex);
            //设置位置
            Vector3 pos = Vector3.zero;
            Vector2 xz = (Vector2)board?.GetGapsOfGrid() * (Vector2)targetIndex;
            if(board!=null)
            {
                pos = (Vector3)board?.GetGrid00LocalPosition() 
                    + new Vector3(xz.x, 0, xz.y)
                    + new Vector3(0,initialHeight,0);
            }
            //设置其父物体为棋盘
            tnt.transform.SetParent(board?.transform);
            tnt.transform.position = pos;
            //添加到道具管理器中
            BattleMessage.instance?.GetGadgetList()?.Add(tnt);
            //尝试播放安置炸弹音效
            GetComponent<CardVoiceController>()?.PlayCardVoice("TNT_Planing");
            yield return base.AfterPlay();
        }
    }
}