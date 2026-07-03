using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(StackCardDisplayer))]
public class StackCardDisplayerAnimatorController : MonoBehaviour
{
    [SerializeField] private StackCardDisplayer stackCardDisplayer;
    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(animator == null) animator = GetComponent<Animator>();
        if(stackCardDisplayer == null) stackCardDisplayer = GetComponent<StackCardDisplayer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimatorParameters();
    }

    private void SetAnimatorParameters()
    {
        animator?.SetBool("IsDisplay", (bool)stackCardDisplayer?.IsDisplay());
    }
}
