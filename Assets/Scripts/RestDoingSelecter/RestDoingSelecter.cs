using UnityEngine;
using UnityEngine.UI;

namespace RestDoingSystem
{
    //休息选择器,是一个UI单例物体
    [RequireComponent(typeof(Canvas))]
    public class RestDoingSelecter : MonoBehaviour
    {
        public static RestDoingSelecter instance;
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
        [SerializeField] private Canvas cvs;
        [SerializeField] private Animator animator;
        void OnEnable()
        {
            if(cvs == null) cvs = GetComponent<Canvas>();
            if(animator == null) animator = GetComponent<Animator>();
        }
        
        [SerializeField] private bool operateOver = false;//玩家是否已经选择完了
        public bool OperateOver { get => operateOver; set => operateOver = value; }
        //控制UI显示的开关
        [SerializeField] private bool isOpen;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        void Update()
        {
            if(cvs != null) cvs.enabled = isOpen;
            animator.SetBool("OperateOver",operateOver);
        }
    }
}
