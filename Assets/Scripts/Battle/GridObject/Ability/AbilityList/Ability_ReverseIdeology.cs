using System.Collections;
using UnityEngine;
using GridObjectSystem.RoleSystem.PlayerSystem;
using GridObjectSystem.RoleSystem;

namespace GridObjectSystem.AbilitySystem.AllAbilities
{
    //能力->无敌: 当前玩家受到的所有伤害变为0
    public class Ability_ReverseIdeology : Ability
    {
        public Ability_ReverseIdeology() : base()
        {
            AbilityName = "Ability_ReverseIdeology";//能力名称
            canStack = true;//ReverseIdeology可以叠加
            canNegative = true;//ReverseIdeology不能小于0
            isDebuff = true;//ReverseIdeology是debuff
        }

        public override IEnumerator AfterAbilityAdded(GridObject effectObject = null)//能力被添加后的效果
        {
            //将主CinemachineCamera控制脚本反转
            GridObject player = BattleMessage.instance?.GetControlPlayer();
            if(effectObject == player)//是当前游戏玩家具有的能力
            {
                BattleBoard.instance?.GetComponent<BattleBoardCameraSetter>()?.SetReverseIdeology(true);
            }
            yield return base.AfterAbilityAdded(effectObject);
        }

        public override IEnumerator AfterAbilityRemoved(GridObject effectObject = null)
        {
            //将主CinemachineCamera控制脚本重置
            GridObject player = BattleMessage.instance?.GetControlPlayer();
            if(effectObject == player)//是当前游戏玩家具有的能力
            {
                BattleBoard.instance?.GetComponent<BattleBoardCameraSetter>()?.SetReverseIdeology(false);
            }
            yield return base.AfterAbilityRemoved(effectObject);
        }

        public override IEnumerator AfterRoundEnd(GridObject effectObject = null)
        {
            //if(BattleMessage.instance?.IsPlayerTurn() != effectObject?.GetSide())
            yield return effectObject?.AddAbility<Ability_ReverseIdeology>(-1);//自身回合结束的时候减少一层
            yield return base.AfterRoundEnd(effectObject);//运行父类方法
        }
    }
}