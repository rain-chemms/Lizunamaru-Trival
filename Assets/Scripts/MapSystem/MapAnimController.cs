using UnityEngine;

namespace MapSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Map))]
    public class MapAnimController : MonoBehaviour
    {
        [SerializeField] private Map map;
        [SerializeField] private Animator animator;
        void OnEnable()
        {
            if(map == null) map = GetComponent<Map>();
            if(animator == null) animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            animator?.SetBool("IsDisplay", (bool)map?.IsDisplay());
        }
    }
}