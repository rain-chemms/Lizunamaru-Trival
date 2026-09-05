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
            tnt?.SetGridIndex(targetIndex);
            //设置位置
            BattleBoard board = BattleBoard.instance;
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