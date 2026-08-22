using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RectTransform))]
    //卡牌的属性和功能全在这个类及其继承中实现
    public class Card : MonoBehaviour, CardFunctioner
    {
        [SerializeField] private uint riceCost = 0;//打出这张牌需要消耗的ricePoint数
        public void SetRiceCost(uint cost)
        {
            riceCost = cost;
        }
        public uint GetRiceCost()
        {
            return riceCost;
        }
        //卡牌类别
        [SerializeField] private CardCategory cardCategory;
        public void SetCardCategory(CardCategory ctg)
        {
            cardCategory = ctg;
        }
        public CardCategory GetCardCategory()
        {
            return cardCategory;
        }
        //卡牌关键字列表
        [SerializeField] private List<CardKeyWord> cardKeyWords = new List<CardKeyWord>();
        public List<CardKeyWord> GetCardKeyWords()
        {
            return cardKeyWords;
        }
        public void AddCardKeyWord(CardKeyWord kw)
        {
            if (cardKeyWords == null) return;
            if (!cardKeyWords.Contains(kw)) cardKeyWords.Add(kw);
        }

        //卡牌接口的空实现
        public virtual IEnumerator AfterInsertToSolt()
        {
            Debug.Log("[Card]:" + name + " have InsertToSolt!");
            yield return null;
        }
        public virtual IEnumerator AfterPlay()
        {
            //尝试播放打出音效
            GetComponent<CardVoiceController>()?.PlayCardVoice("Play");
            //当有消耗词条是将触发卡牌的
            if ((bool)cardKeyWords?.Contains(CardKeyWord.EXHAUST))
            {
                yield return BattleMessage.instance?.ExhaustCard(this);//消耗这张卡
            }
            yield return null;

        }
        public virtual IEnumerator AfterRemoveFromSolt()
        {
            yield return null;
        }
        public virtual IEnumerator AfterTriggerEffective()
        {
            yield return null;
        }
        public virtual IEnumerator AfterRoundEnd()
        {
            yield return null;
        }
        //回合开始时触发
        public virtual IEnumerator AfterRoundStart()
        {
            yield return null;
        }

        //在你的回合丢弃时触发
        public virtual IEnumerator AfterDiscard()
        {
            //尝试播放丢弃音效
            GetComponent<CardVoiceController>()?.PlayCardVoice("Discard");
            yield return null;
        }

        //在抽到卡牌时触发
        public virtual IEnumerator AfterDraw()
        {
            //尝试播放抽卡音效
            GetComponent<CardVoiceController>()?.PlayCardVoice("Draw");
            yield return null;
        }

        public virtual IEnumerator AfterExhaust()
        {
            //尝试播放卡片的消耗音效
            GetComponent<CardVoiceController>()?.PlayCardVoice("Exhaust");
            yield return null;
        }
        
        
        //不在卡牌列表中且有效的卡牌默认加入弃牌堆中
        protected virtual void OnEnable()
        {
            /*
            BattleMessage bi = BattleMessage.instance;//不在卡槽,手中,三个牌堆中的卡牌
            if(bi == null) return;
            if(!(bool)bi?.IsCardInDiscardStack(this)
             && !(bool)bi?.IsCardInDrawStack(this)
             && !(bool)bi?.IsCardInHand(this)
             && !(bool)bi?.IsCardInExhaustStack(this)
             && !(bool)bi?.IsCardInSlot(this))
            {
                //将其加入底牌堆中
                bi?.GetDiscardCardList().Add(this);
            }*/
            
        }

        protected virtual void OnDisable()//非激活状态的牌移除出控制列表
        {
            /*
            BattleMessage bi = BattleMessage.instance;//不在卡槽,手中,三个牌堆中的卡牌
            if((bool)bi?.IsCardInDiscardStack(this)) bi?.GetDiscardCardList().Remove(this);
            if((bool)bi?.IsCardInDrawStack(this)) bi?.GetDrawCardList().Remove(this);
            if((bool)bi?.IsCardInHand(this)) bi?.GetHandCardList().Remove(this);
            if((bool)bi?.IsCardInExhaustStack(this)) bi?.GetExhaustCardList().Remove(this);
            if((bool)bi?.IsCardInSlot(this))
            {
                foreach (CardSlot slot in bi?.GetAllCardSlot())
                {
                    if((bool)slot.GetInnerCard() == this) slot.SetInnerCard(null);
                }
            }
            */
        }
    }
}