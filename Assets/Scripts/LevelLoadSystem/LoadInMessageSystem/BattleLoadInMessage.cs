using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GridObjectSystem.RoleSystem;
using GridObjectSystem;
using MapSystem;


namespace LevelLoadSystem.LoadInMessageSystem
{
    //这是战斗的加载数据
    [CreateAssetMenu(fileName = "BattleLodeInMessage", menuName = "LevelLoadInMessage/BattleLodeInMessage")]
    public class BattleLoadInMessage : LevelLoadInMessage
    {
        //棋盘格相关数据
        [Header("棋盘格相关数据")]
        [Header("棋盘宽&高")]
        [SerializeField] private Vector2Int widthAndHeight;//棋盘格宽高
        public Vector2Int GetWidthAndHeight() => widthAndHeight;
        [Header("棋盘格间隙大小")]
        [SerializeField] private Vector2 gapsOfGrids = new Vector2(2.75f, 2.75f);//棋盘格间距
        public Vector2 GetGapsOfGrids() => gapsOfGrids;
        [Header("棋盘格00相对位置")]
        [SerializeField] private Vector3 grid00LocalPosition = new Vector3(0, 0, 0);//棋盘格00相对位置
        public Vector3 GetGrid00LocalPosition() => grid00LocalPosition;
        [Header("空格列表(表示该Index不生成棋盘格)")]
        [SerializeField] private List<Vector2Int> emptyGridsIndex = new List<Vector2Int>();//控制哪些棋盘格不自动生成
        public List<Vector2Int> GetEmptyGridsIndex() => emptyGridsIndex;
        [Header("玩家起始位置")]
        [Header("注:玩家的预制体由选择角色的阶段确定")]
        [SerializeField] private Vector2Int playerStartIndex;//玩家起始位置
        public Vector2Int GetPlayerStartIndex() => playerStartIndex;
        //敌人相关数据
        [Header("敌人相关数据")]
        [Header("敌人生成位置字典")]
        [SerializeField] private SerializableDictionary<Vector2Int, Role> enermyDict;//敌人索引字典
        public SerializableDictionary<Vector2Int, Role> GetEnermyDict() => enermyDict;

        public BattleLoadInMessage() : base()
        {
            //强制限制类别
            MapNodeCategory ctg = category;
            switch(ctg)
            {
                case MapNodeCategory.STORE:
                case MapNodeCategory.CHAPTER_START:
                case MapNodeCategory.EVENT:
                case MapNodeCategory.CAMPFIRE:
                case MapNodeCategory.BONUS:
                    category = MapNodeCategory.BATTLE_NORMAL;
                    break;
            }
        }
    }
}
