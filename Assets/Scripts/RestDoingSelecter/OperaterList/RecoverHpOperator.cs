using GameSaveSystem;
using UnityEngine;

namespace RestDoingSystem
{
    public class RecoverHpOperator : RestOperater
    {
        //一次恢复多少血量
        public static float recoverPrecent = 1;
        protected override void OperateEffect()
        {
            base.OperateEffect();
            //恢复玩家的生命值
            var data = SaveSystem.instance.GamePlayDatas[SaveSystem.instance.NowSaveIndex];
            float hp = data.currentHp;
            float maxHp = data.maxHp;
            float recover = maxHp * recoverPrecent;
            float endHp = hp + recover;
            if(endHp > maxHp) endHp = maxHp;
            data.currentHp = endHp;
            //依据当前的血量刷新UI,但是不保存到文件中,只有下一关开始的时候才保存到文件中
            
        }
    }


}