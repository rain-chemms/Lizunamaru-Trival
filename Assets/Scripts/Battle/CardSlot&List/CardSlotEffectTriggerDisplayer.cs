using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;


//用于显示卡槽的可触发情况
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(Animator))]
public class CardSlotEffectTriggerDisplayer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Canvas canvas;

    [SerializeField] private CardSlotEffectTriggerController triggerController;
    void OnEnable()
    {
        if(animator == null) animator = GetComponent<Animator>();
        if(canvas == null) canvas = GetComponent<Canvas>();
        if(triggerController == null) triggerController = GetComponentInParent<CardSlotEffectTriggerController>();
        if(mask == null) mask = GetComponentsInChildren<Image>().Where(x => x.name.Equals("Mask")).FirstOrDefault();
        if(counter == null) counter = GetComponentsInChildren<TMP_Text>().Where(x => x.name.Equals("Counter")).FirstOrDefault();
        //无triggerController,则不显示
        if(triggerController == null) 
        {
            animator?.SetBool("UseOver", false);
            if(mask != null)
            {
                mask.raycastTarget = false;
                mask.maskable = false;
            }
            gameObject?.SetActive(false);
        }
    }

    void Start()
    {
        SetCanvasSort();
    }

    private void SetCanvasSort()
    {
        CardSlot slot = triggerController?.GetCardSlot();        
        if(canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = (int)slot?.GetLayerOrder() + 2;//=内部卡牌的层级+1
        }
    }

    [SerializeField] private TMP_Text counter;
    [SerializeField] private Image mask;

    private void CheckTriggerCount()
    {
        bool useOver = triggerController.GetRemainTriggerCount() <= 0;
        animator.SetBool("UseOver", useOver);
        if(useOver)
        {
            //将Mask的遮罩打开,阻止玩家对卡牌的操作
            if(mask != null)
            {
                mask.raycastTarget = true;
                mask.maskable = true;
            }
        }
        else
        {
            //将Mask的遮罩关闭,允许玩家对卡牌进行操作
            if(mask != null)
            {
                mask.raycastTarget = false;
                mask.maskable = false;
            }
        }
    }

    private void SetCounter()
    {
        if(counter != null)
        {
            counter.text = triggerController?.GetRemainTriggerCount().ToString();
        }
    }

    void Update()
    {
        SetCounter();
        CheckTriggerCount();
    }
    
}
