using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GridObjectSystem.RoleSystem.PlayerSystem
{
    //玩家控制器,负责检查并控制玩家移动
    [RequireComponent(typeof(Role))]
    [RequireComponent(typeof(PlayerMover))]
    public class PlayerMoveController : MonoBehaviour
    {
        [SerializeField] private uint switchFlyCosy = 0;
        public uint GetSwitchFlyCost() => switchFlyCosy;
        public void SetSwitchFlyCost(uint newCost) => switchFlyCosy = newCost;
        [SerializeField] private float normalSpeed = 5.0f;//高速移动时的速度
        public float GetNormalSpeed() => normalSpeed;
        public float SetNormalSpeed(float newSpeed) => normalSpeed = newSpeed;
        [SerializeField] private float lowSpeed = 2.0f;//低速移动时的速度
        public float GetLowSpeed() => lowSpeed;
        public float SetLowSpeed(float newSpeed) => lowSpeed = newSpeed;
        [SerializeField] private bool isLowSpeed = false;//是否处于低速移动状态
        public bool GetIsLowSpeed() => isLowSpeed;
        [SerializeField] private InputActionAsset inputSystem;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private Role role;
        [SerializeField] private PlayerMover roleMover;
        // 移动距离
        [SerializeField] private int moveDistance = 1;
        public int GetMoveDistance() => moveDistance;
        public void SetMoveDistance(int newDistance) => moveDistance = newDistance;
        // 移动消耗点
        [SerializeField] private uint moveCostPoint = 1;
        public uint GetMoveCostPoint() => moveCostPoint;
        public void SetMoveCostPoint(uint newPoint) => moveCostPoint = newPoint;
        //是否为第一人称模式的移动
        [SerializeField] private bool firstPresentMode = false;
        public bool IsFirstPresentMode() => firstPresentMode;
        public void SetFirstPresentMode(bool firstPresentMode) => this.firstPresentMode = firstPresentMode;
        async void Start()
        {
            if (inputSystem == null)
            {
                AsyncOperationHandle<InputActionAsset> handle = Addressables.LoadAssetAsync<InputActionAsset>("InputSystem");
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    inputSystem = handle.Result;//获取输入系统
                    Debug.Log("[PlayerMoveController] Auto Get The InputActionAsset: " + inputSystem.name);
                }
                else
                {
                    Debug.LogError("[PlayerMoveController] Failed To Get The InputActionAsset!");
                }
            }
            if (roleMover == null) roleMover = GetComponent<PlayerMover>();
            if (role == null) role = GetComponent<Role>();
        }

        void Awake()
        {
            InitInputActionSystem();
        }

        private InputActionMap inputMap;
        private InputAction moveUp;
        private InputAction moveDown;
        private InputAction moveLeft;
        private InputAction moveRight;
        private InputAction lowSpeeder;
        private InputAction flyShifter;
        private void InitInputActionSystem()
        {
            inputMap = inputSystem?.FindActionMap("PlayerMoveController");
            moveUp = inputMap?.FindAction("MoveUp");
            moveDown = inputMap?.FindAction("MoveDown");
            moveLeft = inputMap?.FindAction("MoveLeft");
            moveRight = inputMap?.FindAction("MoveRight");
            lowSpeeder = inputMap?.FindAction("LowSpeeder");
            flyShifter = inputMap?.FindAction("FlyShifter");
        }

        // 当脚本被启用时,连接监听
        void OnEnable()
        {
            moveDown.performed += OnMoveDown;
            moveUp.performed += OnMoveUp;
            moveLeft.performed += OnMoveLeft;
            moveRight.performed += OnMoveRight;
            lowSpeeder.performed += OpenLowSpeed;
            lowSpeeder.canceled += CloseLowSpeed;
            flyShifter.performed += SwitchFlyState;
        }
        private void SwitchFlyState(InputAction.CallbackContext context)
        {
            roleMover?.SwitchFlyState(switchFlyCosy);
        }
        private void OpenLowSpeed(InputAction.CallbackContext context)
        {
            isLowSpeed = true;
            role?.SetSpeed(lowSpeed);
        }
        private void CloseLowSpeed(InputAction.CallbackContext context)
        {
            isLowSpeed = false;
            role?.SetSpeed(normalSpeed);
        }

        // 当脚本被禁用时,断开监听
        void OnDisable()
        {
            moveDown.performed -= OnMoveDown;
            moveUp.performed -= OnMoveUp;
            moveLeft.performed -= OnMoveLeft;
            moveRight.performed -= OnMoveRight;
            lowSpeeder.performed -= OpenLowSpeed;
            lowSpeeder.canceled -= CloseLowSpeed;
            flyShifter.performed -= SwitchFlyState;
        }

        //下面这些RoleMover的输入参数可以使用Role进行修改
        private void OnMoveRight(InputAction.CallbackContext context)
        {
            //对应键盘D
            BattleDirection targetDir = BattleDirection.RIGHT;
            if(firstPresentMode)
            {
                BattleDirection dir = (BattleDirection)role?.GetDirection();//目前玩家的方向 
                switch(dir)
                {
                    case BattleDirection.DOWN:
                        targetDir = BattleDirection.LEFT;
                        break;
                    case BattleDirection.LEFT:
                        targetDir = BattleDirection.UP;
                        break;
                    case BattleDirection.RIGHT:
                        targetDir = BattleDirection.DOWN;
                        break;
                    case BattleDirection.UP:
                    default:
                        targetDir = BattleDirection.RIGHT;
                        break;
                }
            }
            //应用移动
            roleMover?.ChangeRoleDirection(targetDir);
            roleMover?.MoveRole(targetDir, moveDistance, moveCostPoint);
            
        }

        private void OnMoveLeft(InputAction.CallbackContext context)
        {
            //对应键盘A
            BattleDirection targetDir = BattleDirection.LEFT;
            if(firstPresentMode)
            {
                BattleDirection dir = (BattleDirection)role?.GetDirection();//目前玩家的方向 
                switch(dir)
                {
                    case BattleDirection.DOWN:
                        targetDir = BattleDirection.RIGHT;
                        break;
                    case BattleDirection.LEFT:
                        targetDir = BattleDirection.DOWN;
                        break;
                    case BattleDirection.RIGHT:
                        targetDir = BattleDirection.UP;
                        break;
                    case BattleDirection.UP:
                    default:
                        targetDir = BattleDirection.LEFT;
                        break;
                }
            }
            //应用移动
            roleMover?.ChangeRoleDirection(targetDir);
            roleMover?.MoveRole(targetDir, moveDistance, moveCostPoint);
        }
        private void OnMoveUp(InputAction.CallbackContext context)
        {
            BattleDirection targetDir = BattleDirection.UP;
            //对应键盘W
            if(firstPresentMode)
            {
                BattleDirection dir = (BattleDirection)role?.GetDirection();//目前玩家的方向 
                switch(dir)
                {
                    case BattleDirection.DOWN:
                        targetDir = BattleDirection.DOWN;
                        break;
                    case BattleDirection.LEFT:
                        targetDir = BattleDirection.LEFT;
                        break;
                    case BattleDirection.RIGHT:
                        targetDir = BattleDirection.RIGHT;
                        break;
                    case BattleDirection.UP:
                    default:
                        targetDir = BattleDirection.UP;
                        break;
                }
            }
            //应用移动
            roleMover?.ChangeRoleDirection(targetDir);
            roleMover?.MoveRole(targetDir, moveDistance, moveCostPoint);
        }
        private void OnMoveDown(InputAction.CallbackContext context)
        {
            BattleDirection targetDir = BattleDirection.DOWN;
            //对应键盘S
            if(firstPresentMode)
            {
                BattleDirection dir = (BattleDirection)role?.GetDirection();//目前玩家的方向 
                switch(dir)
                {
                    case BattleDirection.DOWN:
                        targetDir = BattleDirection.UP;
                        break;
                    case BattleDirection.LEFT:
                        targetDir = BattleDirection.RIGHT;
                        break;
                    case BattleDirection.RIGHT:
                        targetDir = BattleDirection.LEFT;
                        break;
                    case BattleDirection.UP:
                    default:
                        targetDir = BattleDirection.DOWN;
                        break;
                }
            }
            //应用移动
            roleMover?.ChangeRoleDirection(targetDir);
            roleMover?.MoveRole(targetDir, moveDistance, moveCostPoint);
        }

    }
}