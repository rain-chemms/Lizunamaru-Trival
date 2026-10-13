using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapSystem.MapNodeSystem
{
    //地图节点图标设置器
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(MapNode))]
    [RequireComponent(typeof(Button))]
    public class MapNodeSelectStateChecker : MonoBehaviour
        ,IPointerEnterHandler
        ,IPointerExitHandler
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Button button;        
        [SerializeField] private MapNode node;
        void OnEnable()
        {
            if(node == null) node = GetComponent<MapNode>();
            if(button == null) button = GetComponent<Button>();
            if(animator == null) animator = GetComponent<Animator>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            notBlink = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            notBlink = false;
        }

        void Update()
        {
            CheckSelectState();
            SetStateToAnimator();
        }

        private void SetStateToAnimator()
        {
            animator?.SetBool("CouldBeSelect",couldSelect);
            animator?.SetBool("NotBlink",notBlink);
        }

        [SerializeField] private bool couldSelect = false;
        [SerializeField] private bool notBlink = false;
        //检查当前节点是否处于被当前位置连接的状态
        private void CheckSelectState()
        {
            Vector2Int index = (Vector2Int)Map.instance?.GetPlayerPos();
            Vector2Int nodeIndex = node.GetIndex();
            var links = Map.instance?.GetLinkData();
            if(!(bool)links?.ContainsKey(index))
            {
                couldSelect = false;
                return;
            }
            var eLink = links[index];
            if(eLink!=null && eLink.Contains(nodeIndex)) 
            {
                if(!couldSelect) notBlink = false;
                couldSelect = true;
            }
            else 
            {
                if(couldSelect) 
                {
                    notBlink = true;
                }
                animator.SetTrigger("Normal");
                couldSelect = false;
            }
        }
    }
}