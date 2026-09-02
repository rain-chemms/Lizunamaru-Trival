//卡片功能接口
using System.Collections;
namespace CardSystem
{
    public interface ICardFunctioner
    {
        //在卡牌镶嵌入槽位后调用
        IEnumerator AfterInsertToSolt();
        //在卡牌从槽位中取出后调用
        IEnumerator AfterRemoveFromSolt();
        //再激活后产生的效果:针对攻击卡的效果
        IEnumerator AfterTriggerEffective();
        //卡牌打出后调用:针对Effective卡牌打出后的效果
        IEnumerator AfterPlay();
        //回合结束后卡牌在手上时触发
        IEnumerator AfterRoundEnd();
        //回合开始时触发
        IEnumerator AfterRoundStart();
        //在你的回合丢弃时触发
        IEnumerator AfterDiscard();
        //在抽到卡牌时触发
        IEnumerator AfterDraw();
        //在卡牌消耗时触发
        IEnumerator AfterExhaust();
        //在一张卡牌打出的时候的效果
        IEnumerator AfterACardPlayed_WhenCardEveryWhere();//在一张牌被打出后的效果,触发条件时卡牌在任何牌堆中时
        IEnumerator AfterACardPlayed_WhenCardInHand();//在一张牌被打出后的效果,触发条件时卡牌在手牌中时
        IEnumerator AfterACardPlayed_WhenCardExhausted();//在一张牌被打出后的效果,触发条件时卡牌消耗后
        IEnumerator AfterACardPlayed_WhenCardInDiscardStack();//在一张牌被打出后的效果,触发条件时卡牌在弃牌堆中时
        IEnumerator AfterACardPlayed_WhenCardInDrawStack();//在一张牌被打出后的效果,触发条件时卡牌在抽牌堆中时
        IEnumerator AfterRetained();//在一张牌被保留后触发
    }
}