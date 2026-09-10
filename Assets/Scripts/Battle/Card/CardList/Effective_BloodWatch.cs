using UnityEngine;
using System.Collections;
using GridObjectSystem.RoleSystem;


namespace CardSystem.AllCardHub
{ 
    //月神之钟:相关角色:十六夜咲夜
    //打出时,结束当前回合,进行一个额外的回合
    public class Effective_BloodWatch : Card
    {
        public override IEnumerator AfterPlay()
        {
            yield return BattleMessage.instance?.ChangeTurn(false);//不切换控制权切换回合
            yield return base.AfterPlay();
        }
    }
}