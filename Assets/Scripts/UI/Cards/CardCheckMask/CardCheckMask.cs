using UnityEngine;
using CardSystem;
using UnityEngine.UI;

//卡牌遮罩器,用于在卡牌被选中时进行显示
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Image))]
public class CardCheckMask : MonoBehaviour
{
    [SerializeField] private Card card;
    public Card GetCard() => card;
    public void SetCard(Card card) => this.card = card;

    [SerializeField] private Animator animator;

    void OnEnable()
    {
        if(card == null) card = GetComponentInParent<Card>();
        if(animator == null)animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckToDisplay();
    }

    private void CheckToDisplay()
    {
        if(card == null) return;
        CardHoverChecker hck = card.GetComponent<CardHoverChecker>();
        //鼠标悬停时激活动画器
        if(hck != null)
        {
            bool isHovering = hck.IsHovering();
            animator?.SetBool("isOpen",isHovering);
        }
        else
        {
            animator?.SetBool("isOpen",false);
        }
    }

}
