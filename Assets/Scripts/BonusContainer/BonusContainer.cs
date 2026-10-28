using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace BonusSystem
{
    [RequireComponent(typeof(Canvas))]
    public class BonusContainer : MonoBehaviour
    {
        public static BonusContainer instance;
        void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [SerializeField] private Canvas canvas;
        [SerializeField] private Animator animator;
        void OnEnable()
        {
            if(canvas == null) canvas = GetComponent<Canvas>();
            if(animator == null) animator = GetComponent<Animator>();
            if(bonusView == null) bonusView = GetComponentsInChildren<ScrollRect>().Where(x => x.name.Equals("BonusView")).FirstOrDefault();
        }

        [SerializeField] private bool isDisplay = false;
        public bool IsDisplay { get => isDisplay; set => isDisplay = value; }

        [SerializeField] private ScrollRect bonusView;
        public ScrollRect BonusView { get => bonusView; }
        private void SyncDisplayState()
        {
            if(animator == null) return;
            animator?.SetBool("IsDisplay",isDisplay);
        }

        void Update()
        {
            SyncDisplayState();
        }
    }
}