using System.Collections;
using UnityEngine;
using CardSystem;
namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Card))]
    public class CardAnimatorEvents : MonoBehaviour
    {
        [SerializeField] private Card card;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            //尝试自动获取
            if (card == null) card = GetComponent<Card>();
        }

        //关闭一些组件
        public void CloseComponentBeforeExhaust()
        {
            CardHandler handler = card.GetComponent<CardHandler>();
            if(handler != null) handler.enabled = false;
            CardInsertSlotChecker inserter = card.GetComponent<CardInsertSlotChecker>();
            if(inserter != null) inserter.enabled = false;
            CardReturnHandChecker returner = card.GetComponent<CardReturnHandChecker>();
            if(returner != null) returner.enabled = false;
            InHandCardOverrideSortingController sorter = card.GetComponent<InHandCardOverrideSortingController>();
            if(sorter != null) sorter.enabled = false;
            CardInStackChecker inStacker = card.GetComponent<CardInStackChecker>();
            if(inStacker != null) inStacker.enabled = false;
            CardPlayAreaChecker playAreaChecker = card.GetComponent<CardPlayAreaChecker>();
            if(playAreaChecker != null) playAreaChecker.enabled = false;
            UnderCardUIChecker underCardUIChecker = card.GetComponent<UnderCardUIChecker>();
            if(underCardUIChecker != null) underCardUIChecker.enabled = false;
        }

        //再消耗动画结束时调用
        public void AfterExhaust()
        {
            card?.GetComponent<Animator>()?.SetBool("IsHidden", true);
        }

        public void PlayExhaustAudio()
        {
            card?.GetComponent<CardVoiceController>()?.PlayCardVoice("Exhaust");
        }
    }
}