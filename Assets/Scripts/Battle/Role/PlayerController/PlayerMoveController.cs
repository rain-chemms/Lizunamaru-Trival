using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


//玩家控制器,负责检查并控制玩家移动
[RequireComponent(typeof(Role))]
[RequireComponent(typeof(RoleMover))]
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
    [SerializeField] private RoleMover roleMover;
    async void Start()
    {
        if(inputSystem == null) 
        {
            AsyncOperationHandle<InputActionAsset> handle = Addressables.LoadAssetAsync<InputActionAsset>("InputSystem");
            await handle.Task;
            if(handle.Status == AsyncOperationStatus.Succeeded)
            {
                inputSystem = handle.Result;//获取输入系统
                Debug.Log("[PlayerMoveController] Auto Get The InputActionAsset: " + inputSystem.name);
            }
            else
            {
                Debug.LogError("[PlayerMoveController] Failed To Get The InputActionAsset!");
            }
        }
        if(roleMover == null) roleMover = GetComponent<RoleMover>();
        if(role == null) role = GetComponent<Role>();
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
        roleMover?.ChangeRoleDirection(BattleDirection.RIGHT);
        roleMover?.MoveRole(BattleDirection.RIGHT,1,1);
    }
    private void OnMoveLeft(InputAction.CallbackContext context)
    {
        roleMover?.ChangeRoleDirection(BattleDirection.LEFT);
        roleMover?.MoveRole(BattleDirection.LEFT,1,1);
    }
    private void OnMoveUp(InputAction.CallbackContext context)
    {
        roleMover?.ChangeRoleDirection(BattleDirection.UP);
        roleMover?.MoveRole(BattleDirection.UP,1,1);
    }
    private void OnMoveDown(InputAction.CallbackContext context)
    {
        roleMover?.ChangeRoleDirection(BattleDirection.DOWN);
        roleMover?.MoveRole(BattleDirection.DOWN,1,1);
    }
    
}
