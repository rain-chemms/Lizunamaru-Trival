using UnityEngine;
using MapSystem;

namespace AnimatorEventSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Map))]
    public class MapAnimatorEvent : MonoBehaviour
    {
        [SerializeField] private Map map;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            if(map == null) map = GetComponent<Map>();
        }

        public void OpenMapCanvas()
        {
            Canvas cvs = map.GetCanvas();
            if(cvs != null) cvs.enabled = true;
        }

        public void CloseMapCanvas()
        {
            Canvas cvs = map.GetCanvas();
            if(cvs != null) cvs.enabled = false;
        }

    }
}
