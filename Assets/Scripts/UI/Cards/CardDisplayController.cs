using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 卡牌显示器
// 必须挂载在卡牌和含动画器的节点上
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Card))]
public class CardDisplayer : MonoBehaviour
{
    [SerializeField] private bool isFront = true;// true:正面，false:反面
    public bool IsDisplayFront()
    {
        return isFront;
    }
    public void FlipTo(bool backOrFront)
    {
        isFront = backOrFront;
    }

    [SerializeField] private float flipSpeed = 3.0f;// 翻转速度
    public float GetFlipSpeed()
    {
        return flipSpeed;
    }
    public void SetFlipSpeed(float speed)
    {
        flipSpeed = speed;
    }
    
    [SerializeField] private Animator animator;// 关联的卡牌
    [SerializeField] private Card card;
    
    private void Start()
    {
        //尝试自动获取
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
        //尝试自动获取
        if(card == null)
        {
            card = GetComponent<Card>();
        }
    
    }

    void Update()
    {
        //控制动画器参数
        if(animator?.GetBool("IsFront") != isFront) animator?.SetBool("IsFront", isFront);
        if(animator?.GetFloat("FlipSpeed") != flipSpeed) animator?.SetFloat("FlipSpeed", flipSpeed);
    }
}