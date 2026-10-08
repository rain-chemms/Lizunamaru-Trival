using System;
using UnityEngine;

namespace GameSaveSystem
{
    //玩家的档案数据
    [Serializable]
    public class GameSave
    {
        [SerializeField] public long createTime;//创建存档的时间
        [SerializeField] public long lastPlayTime;//上次游玩的时间
        [SerializeField] public string saveName;//存档的名字
        [SerializeField] public int havePlayRound;//当前存档的游玩局数
        //后期还会保留玩家的等级,进度等
    }
}
