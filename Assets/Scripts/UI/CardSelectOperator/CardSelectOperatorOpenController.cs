using UnityEngine;

//卡牌选择执行器的开启控制器
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CardSelectOperator))]
[RequireComponent(typeof(Canvas))]
public class CardSelectOperatorOpenController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Animator animator;
    [SerializeField] private CardSelectOperator cardSelectOperator;
    void OnEnable()
    {
       if(animator == null) animator = GetComponent<Animator>();
        if(cardSelectOperator == null) cardSelectOperator = GetComponent<CardSelectOperator>();
        if(canvas == null) canvas = GetComponent<Canvas>();
    }
    //开启控制器
    [SerializeField] private bool isOpen = false;
    public void SetOpen(bool isOpen) => this.isOpen = isOpen;
    public bool IsOpen() => isOpen;
    [SerializeField] private int openSortOrder = 105;
    public int GetOpenSortOrder() => openSortOrder;
    [SerializeField] private int closeSortOrder = 100;
    public int GetCloseSortOrder() => closeSortOrder;
    void Update()
    {
        CheckOpenState();
    }

    public void CheckOpenState()
    {
        animator.SetBool("IsOpen",isOpen);
        if(canvas!=null) canvas.sortingOrder = isOpen ? openSortOrder : closeSortOrder;
    }
}