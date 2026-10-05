using UnityEngine;
using System.Collections;
using System;
using GridObjectSystem.RoleSystem;

namespace CardSystem.AllCardHub
{
    //准备大叫:相关角色:幽谷响子
    //{0}个回合后,将{1}张"大叫"加入手中,随后消耗这张卡
    //打出时无任何效果
    //具有保留,消耗
    //升级后抽到这张牌的时候再抽一张牌,等待回合数减一
    public class Effective_ScreamPrepare : Card
    {
        [SerializeField] private int giveCardWaitRound = 2;
        public int GiveCardWaitRound { get => giveCardWaitRound; }
        [SerializeField] private int roundRecorder = 0;
        public float RoundRecorder { get => roundRecorder; }

        [SerializeField] private uint addCardNumber = 2;
        public uint AddCardNumber { get => addCardNumber; }
        [SerializeField] private Card addCardPrefab;
        public Card AddCardPrefab { get => addCardPrefab; }

        public override IEnumerator AfterDraw()
        {
            if (cardUpgradeLevel > 0)
            {
                yield return BattleMessage.instance.DrawCard(1);//抽一张卡
            }
            yield return base.AfterDraw();
        }
        [NonSerialized] private bool haveGiven = false; 
        public override IEnumerator AfterRoundEnd()
        {
            Role player = BattleMessage.instance?.GetControlPlayer();
            if (BattleMessage.instance?.IsPlayerTurn() == player?.GetSide())
            {
                roundRecorder++;
                if (roundRecorder >= giveCardWaitRound && !haveGiven)
                {
                    yield return AddNewCardToHand();
                    //消耗这张卡
                    yield return BattleMessage.instance?.ExhaustCard(this);
                    haveGiven = true;
                }
                //可以在这里播放一些特效
                /*
                    暂时还没有实现
                */
            }
            yield return base.AfterRoundEnd();
        }

        private IEnumerator AddNewCardToHand()
        {
            for (int i = 0; i < addCardNumber; i++)
            {
                Card newCard = Instantiate(addCardPrefab, BattleMessageDisplayer.instance.transform);
                if (newCard != null)
                    yield return BattleMessage.instance.AddExistCardToHand(newCard);
            }
        }

        [NonSerialized] private int sourceWaitRound = 0;
        new void OnEnable()
        {
            haveGiven = false;
            sourceWaitRound = giveCardWaitRound;
            base.OnEnable();
        }

        public override IEnumerator UpgradeEffective()
        {
            giveCardWaitRound = sourceWaitRound - 1;
            yield return base.UpgradeEffective();
        }
    }
}