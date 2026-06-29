using UnityEngine;

[RequireComponent(typeof(Role))]
[RequireComponent(typeof(Animator))]
public class RoleAnimTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }
    public void TriggerAnim(string animName)
    {
        animator?.SetTrigger(animName);    
    }
}
