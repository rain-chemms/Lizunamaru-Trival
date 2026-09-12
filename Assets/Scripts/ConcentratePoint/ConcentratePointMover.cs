using UnityEngine;
using UnityEngine.InputSystem;

//聚集点移动器,用与直接设置ConcentratePoint的坐标和状态
[RequireComponent(typeof(ConcentratePoint))]
public class ConcentratePointMover : MonoBehaviour
{
    [SerializeField] private ConcentratePoint point;
    //启用时初始化输入绑定
    void OnEnable()
    {
        if(point == null)
        {
            point = GetComponent<ConcentratePoint>();
            if(point == null) point = ConcentratePoint.instance;//二段获取
        }
        InitInput();
        LinkFunc();
    }

    //禁用时取消输入绑定
    void OnDisable()
    {
        DisLinkFunc();
    }

    [SerializeField] private InputActionAsset inputAsset;//输入动作资源映射
    private InputActionMap inputMap;
    private InputAction moveUp;
    private InputAction moveDown;
    private InputAction moveLeft;
    private InputAction moveRight;
    private InputAction lockShift;
    private InputAction displayShift;

    private void InitInput()
    {
        inputMap = inputAsset?.FindActionMap("ConcentratePointMover");
        moveUp = inputMap?.FindAction("MoveUp");
        moveDown = inputMap?.FindAction("MoveDown");
        moveLeft = inputMap?.FindAction("MoveLeft");
        moveRight = inputMap?.FindAction("MoveRight");
        lockShift = inputMap?.FindAction("LockShift");
        displayShift = inputMap?.FindAction("DisplayShift");
        inputMap?.Enable();//启用输入
    }

    private void LinkFunc()
    {
        if(moveUp != null) moveUp.performed += OnMoveUp;
        if(moveDown != null) moveDown.performed += OnMoveDown;
        if(moveLeft != null) moveLeft.performed += OnMoveLeft;
        if(moveRight != null) moveRight.performed += OnMoveRight;
        if(lockShift != null) lockShift.performed += OnLockShift;
        if(displayShift != null) displayShift.performed += OnDisplayShift;
        
    }

    private void DisLinkFunc()
    {
        if(moveUp != null) moveUp.performed -= OnMoveUp;
        if(moveDown != null) moveDown.performed -= OnMoveDown;
        if(moveLeft != null) moveLeft.performed -= OnMoveLeft;
        if(moveRight != null) moveRight.performed -= OnMoveRight;
        if(lockShift != null) lockShift.performed -= OnLockShift;
        if(displayShift != null) displayShift.performed -= OnDisplayShift;
        inputMap?.Disable();//关闭输入
    }

    private void OnMoveUp(InputAction.CallbackContext context)
    {
        //获取边界
        if((bool)point?.IsLocked()) return;//锁定时禁止移动
        Vector2Int widthAndHeight = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();
        Vector2Int index = (Vector2Int)point?.GetIndex();
        if(index.y + 1 >= widthAndHeight.y) return;
        point?.SetIndex(new Vector2Int(index.x, index.y + 1));
    }

    private void OnMoveDown(InputAction.CallbackContext context)
    {
        //获取边界
        if((bool)point?.IsLocked()) return;//锁定时禁止移动
        Vector2Int widthAndHeight = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();
        Vector2Int index = (Vector2Int)point?.GetIndex();
        if(index.y - 1 < 0) return;
        point?.SetIndex(new Vector2Int(index.x, index.y - 1));
    }
    
    private void OnMoveLeft(InputAction.CallbackContext context)
    {
        //获取边界
        if((bool)point?.IsLocked()) return;//锁定时禁止移动
        Vector2Int widthAndHeight = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();
        Vector2Int index = (Vector2Int)point?.GetIndex();
        if(index.x - 1 < 0) return;
        point?.SetIndex(new Vector2Int(index.x - 1, index.y));
    }

    private void OnMoveRight(InputAction.CallbackContext context)
    {
        //获取边界
        if((bool)point?.IsLocked()) return;//锁定时禁止移动
        Vector2Int widthAndHeight = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();
        Vector2Int index = (Vector2Int)point?.GetIndex();
        if(index.x + 1 >= widthAndHeight.x) return;
        point?.SetIndex(new Vector2Int(index.x + 1, index.y));
    }

    private void OnLockShift(InputAction.CallbackContext context)
    {
        point?.SetIsLocked(!(bool)point?.IsLocked());
    }

    private void OnDisplayShift(InputAction.CallbackContext context)
    {
        bool willSet = !(bool)point?.IsDisplay();
        //若当前不显示则要将焦点锁定,防止它乱跑
        if(!willSet && !(bool)point?.IsLocked()) point?.SetIsLocked(true);
        point?.SetDisplay(willSet);
    }
}
