using System.Collections;
using UnityEngine;
using CardSystem;
using System.Linq;
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

        public void AtEndOfExhaust()
        {
            StartCoroutine(ReopenCardDisplay(card));
        }

        [SerializeField] private float waitAfterExhaustToReDisplay = 0.8f;
        private IEnumerator ReopenCardDisplay(Card card)
        {
            yield return new WaitForSeconds(waitAfterExhaustToReDisplay);
            CardInStackChecker inStacker = card.GetComponent<CardInStackChecker>();
            if(inStacker != null) inStacker.enabled = true;//重新打开卡槽检测器
            CardHandler handler = card.GetComponent<CardHandler>();
            if(handler != null) handler.enabled = true;//重新打开拖拽检测器            
            card?.GetComponent<Animator>()?.SetBool("IsHidden", false);
        }

        public void PlayExhaustAudio()
        {
            card?.GetComponent<CardVoiceController>()?.PlayCardVoice("Exhaust");
        }

        public void TriggerAllDissolveController()
        {
            foreach(var disCtrl in card?.GetComponentsInChildren<CardDissolveController>().ToList())
            {
                Debug.Log("[CardAnimatorEvents]: Trigger Dissolve Controller:"+ disCtrl.name);
                disCtrl?.SetDissolveMaterial();//刷新所有DissolveController的材质
            }
        }

        public void RevertAllDissolveController()
        {
            foreach (var disCtrl in card?.GetComponentsInChildren<CardDissolveController>().ToList())
            {
                disCtrl?.RevertMaterial();//刷新所有DissolveController的材质
            }
        }
    }
}