using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Animator GetAnimator() => animator;
    void OnEnable()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }
    public void TriggerAnim(string animName)
    {
        animator?.SetTrigger(animName);
    }
}
