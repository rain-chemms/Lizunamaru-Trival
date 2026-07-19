using UnityEngine;

[RequireComponent(typeof(Role))]
[RequireComponent(typeof(Animator))]
public class RoleAnimTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Animator GetAnimator() => animator;
    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }
    public void TriggerAnim(string animName)
    {
        animator?.SetTrigger(animName);    
    }
}
