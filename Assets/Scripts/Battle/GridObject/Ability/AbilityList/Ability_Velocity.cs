using System.Collections;
using UnityEngine;
using GridObjectSystem.RoleSystem.PlayerSystem;

namespace GridObjectSystem.AbilitySystem.AllAbilities
{
    //能力->速度: 每一点速度可以增加获得一点移动的距离
    public class Ability_Velocity : Ability
    {
        public Ability_Velocity() : base()
        {
            AbilityName = "Ability_Velocity";//能力名称
            canStack = true;//能力可以叠加
            canNegative = false;//能力不能小于0
            isDebuff = false;//能力不是debuff
        }

        private void IncreaseMoveDistance(GridObject effectObject)
        {
            Debug.Log("[Ability_Velocity]: Increase MoveDistance Called!");
            //设置玩家移动距离:增大
            PlayerMoveController mover = effectObject.GetComponent<PlayerMoveController>();
            if(mover != null)
            {
                int newDistance = (int)mover?.GetMoveDistance() + (int)effectObject?.GetAbilityDict()[this];//获取移动距离
                Debug.Log("[Ability_Velocity]: Increase "+ effectObject.name +" MoveDistance To:" + newDistance.ToString());
                mover.SetMoveDistance(newDistance);
            }    
        }

        private void DecreaseMoveDistance(GridObject effectObject)
        {
            Debug.Log("[Ability_Velocity]: Decrease MoveDistance Called!");
            PlayerMoveController mover = effectObject.GetComponent<PlayerMoveController>();
            if(mover!=null)
            {
                int newDistance = (int)mover?.GetMoveDistance() - (int)effectObject?.GetAbilityDict()[this];//获取移动距离
                if(newDistance <= 0) newDistance = 1;//移动距离不能小于1
                Debug.Log("[Ability_Velocity]: Decrease "+ effectObject.name +" MoveDistance To:" + newDistance.ToString());
                mover.SetMoveDistance(newDistance);
            }
        }

        //每回合开始时设置玩家的移动距离大小,仅对含有PlayerMover脚本的玩家有效
        public override IEnumerator AfterRoundStart(GridObject effectObject = null)
        {
            IncreaseMoveDistance(effectObject);
            yield return base.AfterRoundStart(effectObject);//运行父类方法
        }

        public override IEnumerator AfterAbilityRemoved(GridObject effectObject = null)
        {
            DecreaseMoveDistance(effectObject);
            return base.AfterAbilityRemoved(effectObject);
        }

        public override IEnumerator AfterRoundEnd(GridObject effectObject = null)
        {
            /*
                减少逻辑
                effectObject.AddAbility<Ability_Velocity>(-1);//减少一层
            */
            DecreaseMoveDistance(effectObject);
            yield return base.AfterRoundEnd(effectObject);//运行父类方法
        }
    }
}