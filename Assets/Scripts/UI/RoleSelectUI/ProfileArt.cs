using UnityEngine;

namespace RoleChoiceUISystem
{
    [RequireComponent(typeof(Animator))]
    public class ProfileArt : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        void OnEnable()
        {
            if (animator == null) animator = GetComponent<Animator>();
        }

        void Start()
        {

        }
    }
}
