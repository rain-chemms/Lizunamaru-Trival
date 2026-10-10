using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(RectTransform))]
    //卡牌的属性和功能全在这个类及其继承中实现
    public class Card : MonoBehaviour, ICardFunctioner
    {
        [SerializeField] protected bool openCardArrowLine = false;
        public bool IsOpenCardArrowLine() => openCardArrowLine;
        public void SetOpenCardArrowLine(bool open) => openCardArrowLine = open;

        [SerializeField] protected uint riceCost = 0;//打出这张牌需要消耗的ricePoint数
        public void SetRiceCost(uint cost)
        {
            riceCost = cost;
        }
        public uint GetRiceCost()
        {
            return riceCost;
        }
        //卡牌的稀有度
        [SerializeField] protected CardRarity rarity;
        public CardRarity GetRarity() => rarity;
        public void SetCardRarity(CardRarity rty) => rarity = rty;
        //卡牌类别
        [SerializeField] protected CardCategory cardCategory;
        public void SetCardCategory(CardCategory ctg)
        {
            cardCategory = ctg;
        }
        public CardCategory GetCardCategory()
        {
            return cardCategory;
        }
        //卡牌关键字列表
        [SerializeField] protected List<CardKeyWord> cardKeyWords = new List<CardKeyWord>();
        public List<CardKeyWord> GetCardKeyWords()
        {
            return cardKeyWords;
        }
        public void AddCardKeyWord(CardKeyWord kw)
        {
            if (cardKeyWords == null) return;
            if (!cardKeyWords.Contains(kw)) cardKeyWords.Add(kw);
        }
        [SerializeField] protected bool canRepeatUpgrade = false;//是否可以重复升级
        public bool CanRepeatUpgrade() => canRepeatUpgrade;
        public void SetRepeatUpgrade(bool canRepeat) => canRepeatUpgrade = canRepeat;
        [SerializeField] protected uint cardUpgradeLevel = 0;//卡牌的等级,0等级代表并未升级
        public void SetCardUpgradeLevel(uint level) => cardUpgradeLevel = level;
        public uint GetCardUpgradeLevel() => cardUpgradeLevel;
        //卡牌接口的空实现
        public virtual IEnumerator AfterRetained()//在一张牌被保留后触发
        {
            yield return null;
        }
        //在一张牌被打出后的效果,触发条件时卡牌在任何牌堆中时
        public virtual IEnumerator AfterACardPlayed_WhenCardEveryWhere()
        {
            yield return null;
        }
        //在一张牌被打出后的效果,触发条件时卡牌在手牌中时
        public virtual IEnumerator AfterACardPlayed_WhenCardInHand()
        {
            yield return null;
        }
        //在一张牌被打出后的效果,触发条件时卡牌消耗后
        public virtual IEnumerator AfterACardPlayed_WhenCardExhausted()
        {
            yield return null;
        }
        //在一张牌被打出后的效果,触发条件时卡牌在弃牌堆中时
        public virtual IEnumerator AfterACardPlayed_WhenCardInDiscardStack()
        {
            yield return null;
        }
        //在一张牌被打出后的效果,触发条件时卡牌在抽牌堆中时
        public virtual IEnumerator AfterACardPlayed_WhenCardInDrawStack()
        {
            yield return null;
        }
        public virtual IEnumerator AfterInsertToSolt()
        {
            Debug.Log("[Card]:" + name + " have InsertToSolt!");
            yield return null;
        }
        public virtual IEnumerator AfterPlay()
        {
            //尝试播放打出音效
            GetComponent<CardVoiceController>()?.PlayCardVoice("Play");
            //现在声音由动画时间触发
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
        
        public virtual IEnumerator UpgradeEffective()
        {
            yield return null;
        }
        
        protected virtual void Update()
        {
            //检测并设置卡牌等级变化
            CheckUpgradeChange();
        }

        private uint lastLevel;
        private void CheckUpgradeChange()
        {
            if(lastLevel != cardUpgradeLevel)
            {   
                StartCoroutine(UpgradeEffective());//同步升级后的效果
                lastLevel = cardUpgradeLevel;
            }
        }

        //不在卡牌列表中且有效的卡牌默认加入弃牌堆中
        protected virtual void OnEnable()
        {
            //在卡牌初始化的时候应用升级的效果
            StartCoroutine(UpgradeEffective());
            lastLevel = cardUpgradeLevel;
        }

        protected virtual void OnDisable()//非激活状态的牌移除出控制列表
        {
        }
    }
}