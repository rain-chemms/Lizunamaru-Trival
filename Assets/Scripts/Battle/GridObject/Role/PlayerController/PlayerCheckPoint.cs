using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GridObjectSystem.RoleSystem.PlayerSystem
{
    [RequireComponent(typeof(Animator))]
    public class PlayerCheckPoint : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        void OnEnable()
        {
            if(animator == null) animator = GetComponent<Animator>();
        }
        //关联的玩家移动控制器
        //读取其中的信息以开启或关闭判定点显示器
        [SerializeField] private PlayerMoveController playerMoveController;
        public PlayerMoveController GetPlayerMoveController() => playerMoveController;

        void Update()
        {
            CheckAndSetTheAnimator();
        }
        
        //持续监测并设置动画器
        void CheckAndSetTheAnimator()
        {
            if(animator == null) return ;
            if(playerMoveController == null) animator?.SetBool("IsOpen",false);
            else animator?.SetBool("IsOpen",(bool)playerMoveController?.GetIsLowSpeed());
        } 
    }
}