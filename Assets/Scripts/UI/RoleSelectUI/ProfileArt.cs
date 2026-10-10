using UnityEngine;

namespace RoleChoiceUISystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Canvas))]
    public class ProfileArt : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Canvas canvas;
        void OnEnable()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (canvas == null) canvas = GetComponent<Canvas>();
        }
    }
}
