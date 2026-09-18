using System.Collections;
using UnityEngine;
using CardSystem;

[RequireComponent(typeof(CardSlot))]
public class CardSlotEffectTriggerController : MonoBehaviour
{
    [SerializeField] private CardSlot cardSlot;
    public CardSlot GetCardSlot() => cardSlot;
    void OnEnable()
    {
        if(cardSlot == null) cardSlot = GetComponent<CardSlot>();
    }

    void Start()
    {
        //round = (uint)BattleMessage.instance?.GetRound();
    }

    [SerializeField] private int recoverPreRound = 1;//回合恢复次数
    public void SetRecoverPreRound(int count) => recoverPreRound = count;
    public int GetRecoverPreRound() => recoverPreRound;
    //每当回合切换的时候,重置可触发次数
    public void RecoverTriggerCount()
    {
        remainTriggerCount = recoverPreRound;
    }
    /*
    private uint round = 0;
    private void CheckRoundRecoverTriggerCount()
    {
        uint currentRound = (uint)BattleMessage.instance?.GetRound();
        if(round != currentRound)
        {
            round = currentRound;
            remainTriggerCount = recoverPreRound;    
        }
    }

    void Update()
    {
        CheckRoundRecoverTriggerCount();
    }
    */

    //这个在每个回合之后会重置
    [SerializeField] private int remainTriggerCount;//剩余可触发的次数
    public void SetRemainTriggerCount(int count) => remainTriggerCount = count;
    public int GetRemainTriggerCount() => remainTriggerCount;

    public IEnumerator TriggerInnerCard()
    {
        if(cardSlot == null) yield break;
        Card innerCard = cardSlot?.GetInnerCard();
        if(innerCard == null || remainTriggerCount <= 0) yield break;
        yield return innerCard?.AfterTriggerEffective();//触发效果
        remainTriggerCount--;//减去一次可触发次数
    }

    
}
