using System.Collections;
using System.Collections.Generic;

namespace GridObjectSystem.AbilitySystem
{
    //能力抽象接口,包含了能力的作用效果
    public interface IAbilityFunctioner
    {
        IEnumerator AfterRoundEnd(GridObject effectObject = null);//某方结束一回合产生的效果,效果可以对多个对象产生效果
        IEnumerator AfterRoundStart(GridObject effectObject = null);//某方开始一回合产生的效果,效果可以对多个对象产生效果
        IEnumerator AfterAbilityAmountChanged(GridObject effectObject = null);//能力数量改变后的效果,效果可以对多个对象产生效果
        IEnumerator AfterACardPlayed(GridObject effectObject = null);//在一张牌被打出后的效果
        IEnumerator AfterAbilityRemoved(GridObject effectObject = null);//能力被移除后的效果
        IEnumerator AfterAbilityAdded(GridObject effectObject = null);//能力被添加后的效果
    }
}