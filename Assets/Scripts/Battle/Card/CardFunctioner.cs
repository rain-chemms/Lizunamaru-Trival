//卡片功能接口
using System.Collections;
using UnityEngine.EventSystems;

public interface CardFunctioner
{
    //在卡牌镶嵌入槽位后调用
    IEnumerator AfterInsertToSolt();
    //在卡牌从槽位中取出后调用
    IEnumerator AfterRemoveFromSolt();
    
    //再激活后产生的效果:针对攻击卡的哦那估计效果
    IEnumerator AfterTriggerEffective();
    
    //卡牌打出后调用:针对Effective卡牌打出后的效果
    IEnumerator AfterPlay();
}