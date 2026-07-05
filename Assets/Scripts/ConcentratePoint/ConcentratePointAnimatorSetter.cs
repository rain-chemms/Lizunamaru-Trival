using UnityEngine;

[RequireComponent(typeof(ConcentratePoint))]
[RequireComponent(typeof(Animator))]
public class ConcentratePointAnimatorSetter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ConcentratePoint point;
    void Start()
    {
        //尝试自动获取
        if(animator == null) animator = GetComponent<Animator>();
        if(point == null) point = GetComponent<ConcentratePoint>();
    }

    // Update is called once per frame
    void Update()
    {
        animator?.SetBool("Locked",(bool)point?.IsLocked());       
    }
}
