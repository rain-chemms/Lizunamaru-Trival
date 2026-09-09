using Unity.Cinemachine;
using UnityEngine;
using GridObjectSystem;
using GridObjectSystem.RoleSystem;

[RequireComponent(typeof(BattleBoard))]
public class BattleBoardCameraSetter : MonoBehaviour
{
    [Header("注:若对应量启用syncToControlPlayer选项,则当前代表的是角色面向UP方向的偏移")]
    [SerializeField] private Vector3 posOffset = new Vector3(0, 0, 0);//若同步角色位置,则当前代表的是角色面向UP方向的偏移
    public Vector3 GetPosOffset() => posOffset;
    public void SetPosOffset(Vector3 posOffset) => this.posOffset = posOffset;

    [SerializeField] private Vector3 rotOffset = new Vector3(0, 0, 0);//若同步角色位置,则当前代表的是角色面向UP方向的偏移
    public Vector3 GetRotOffset() => rotOffset;
    public void SetRotOffset(Vector3 rotOffset) => this.rotOffset = rotOffset;

    [SerializeField] private bool resetPos = false;
    [SerializeField] private bool resetRot = false;

    public void ResetCamera()
    {
        resetPos = true;
        resetRot = true;
    }

    [SerializeField] private float lerpSpeed = 5f;
    public float GetLerpSpeed() => lerpSpeed;
    public void SetLerpSpeed(float lerpSpeed) => this.lerpSpeed = lerpSpeed;

    [SerializeField] private float rotateSpeed = 5f;
    public float GetRotateSpeed() => rotateSpeed;
    public void SetRotateSpeed(float rotateSpeed) => this.rotateSpeed = rotateSpeed;

    [SerializeField] private float stopDistance = 0.1f;//摄像机停止移动的距离
    [SerializeField] private float stopRotateDistance = 0.1f;
    [SerializeField] private BattleBoard board;

    void Start()
    {
        if (board == null) board = GetComponent<BattleBoard>();
        AutoSetCineCamera();
        ResetCamera();//启动时重置摄像机
    }

    [SerializeField] private CinemachineCamera cinemachineCamera;
    public CinemachineCamera GetCineCamera() => cinemachineCamera;
    public void AutoSetCineCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            CinemachineBrain brain = cam.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                cinemachineCamera = brain.ActiveVirtualCamera as CinemachineCamera;
            }
        }
    }

    [SerializeField] private bool syncToControlPlayerPos = false;//是否同步到控制的玩家
    public bool IsSyncToControlPlayerPos() => syncToControlPlayerPos;
    public void SetSyncToControlPlayerPos(bool syncToControlPlayer) => this.syncToControlPlayerPos = syncToControlPlayer;
    [SerializeField] private bool syncToControlPlayerRot = false;
    public bool IsSyncToControlPlayerRot() => syncToControlPlayerRot;
    public void SetSyncToControlPlayerRot(bool syncToControlPlayerRot) => this.syncToControlPlayerRot = syncToControlPlayerRot;

    [SerializeField] private Vector2 appendBoardOffset;
    public Vector2 GetAppendBoardOffset() => appendBoardOffset;
    public void SetAppendBoardOffset(Vector2 appendBoardOffset) => this.appendBoardOffset = appendBoardOffset;

    private Vector3 GetResetPosition()
    {
        BattleBoard board = BattleBoard.instance;
        if (board == null) return Vector3.zero;//没有棋盘物体
        Vector3 end = Vector3.zero;
        if (cinemachineCamera != null)
        {
            //将摄像机放到z方向的0层的下面X轴的中间
            Vector3 pos = board.GetGrid00LocalPosition();
            Vector2 gaps = board.GetGapsOfGrid();
            end = pos + board.transform.position;

            //额外偏移量计算
            Vector3 temp = new Vector3(posOffset.x, posOffset.y, posOffset.z);
            Role player = BattleMessage.instance?.GetControlPlayer();

            if (syncToControlPlayerPos && player != null)
            {
                //同步方式:y轴不动,分别计算x和z轴的数据
                Quaternion r = GetRotateYByRole(player);
                temp = r * temp;//计算旋转后的偏移量
            }
            end += temp;//添加偏移量

            //计算额外的棋盘偏移
            end += new Vector3(
                appendBoardOffset.x * gaps.x,
                0,
                appendBoardOffset.y * gaps.y
            );

            //若开启同步到控制玩家
            if (syncToControlPlayerPos)
            {
                //player上面已经获取了
                Vector2Int playerIndex = (Vector2Int)player?.GetGridIndex();
                //获取玩家的实际飞行高度
                bool isFly = (bool)player?.IsFly();
                GridObjectPositionSetter posSetter = player?.GetComponent<GridObjectPositionSetter>();
                float flyHeight = isFly ? 
                    (float)posSetter?.GetFlyHeight() + (float)posSetter?.GetLandHeightOffset() : 
                    (float)posSetter?.GetLandHeightOffset();
                
                //计算玩家对应的偏移量
                Vector3 playerOffset = new Vector3(
                    playerIndex.x * gaps.x,
                    flyHeight,
                    playerIndex.y * gaps.y
                );
                end += playerOffset;//添加玩家位移
            }
        }
        return end;
    }

    [SerializeField] private bool alawysReset = false;
    public bool IsAlawysReset() => alawysReset;
    public void SetAlawysReset(bool alawysReset) => this.alawysReset = alawysReset;

    void Update()
    {
        CheckResetPos();
        CheckResetRot();
        if (alawysReset)
        {
            ResetCamera();
        }
    }

    private Quaternion GetRotateYByRole(Role player)
    {
        if (player == null) return Quaternion.Euler(0, 0, 0);
        Quaternion r = Quaternion.Euler(0, 0, 0);
        switch (player.GetDirection())
        {
            case BattleDirection.LEFT:
                r = Quaternion.Euler(0, -90, 0);
                break;
            case BattleDirection.RIGHT:
                r = Quaternion.Euler(0, 90, 0);
                break;
            case BattleDirection.DOWN:
                r = Quaternion.Euler(0, 180, 0);
                break;
            case BattleDirection.UP:
            default:
                r = Quaternion.Euler(0, 0, 0);
                break;
        }
        return r;
    }


    /*
    private void CheckResetRot()
    {
        ////摄像机旋转处理
        if (board == null) resetRot = false;
        Vector3 nowRot = (Vector3)cinemachineCamera?.transform?.rotation.eulerAngles;
        Role player = BattleMessage.instance?.GetControlPlayer();
        //构建绕世界Y轴的旋转增量
        Quaternion worldYRotation = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, Vector3.up);
        //初始旋转方向
        Quaternion endRot = board.transform.rotation;
        //基础旋转
        Quaternion temp = Quaternion.Euler(
            rotOffset.x,
            rotOffset.y,
            rotOffset.z
        );

        if (syncToControlPlayerRot && player != null)
        {
            //同步旋转器,让其绕Y轴旋转
            Quaternion r = GetRotateYByRole(player);
            //转为世界旋转增量
            r = r * worldYRotation;
            temp *= r;
        }

        endRot *= temp;

        //玩家旋转同步
        /*
        if (syncToControlPlayerRot)
        {
            if (player != null)
            {
                Quaternion yRotate = Quaternion.Euler(
                    0,
                    player.transform.rotation.eulerAngles.y,
                    0
                );
                endRot *= yRotate;//旋转
            }
        }
        

        if (Vector3.Distance(nowRot, endRot.eulerAngles) < stopRotateDistance)
        {
            resetRot = false;
        }
        //旋转摄像机
        if (resetRot)
        {

            cinemachineCamera.transform.rotation = Quaternion.Lerp(
                cinemachineCamera.transform.rotation,
                endRot,
                rotateSpeed * Time.deltaTime
            );
        }
    }
    */
    private void CheckResetRot()
    {
        if (board == null || cinemachineCamera == null) { resetRot = false; return; }
        Role player = BattleMessage.instance?.GetControlPlayer();
        // 玩家朝向对应的世界 Y 轴偏航
        Quaternion yaw = Quaternion.identity;
        if (syncToControlPlayerRot && player != null)
            yaw = GetRotateYByRole(player);

        // 先绕世界(棋盘)竖直轴 yaw,再绕 yaw 之后的局部水平轴 pitch
        Quaternion endRot = board.transform.rotation * yaw * Quaternion.Euler(rotOffset);

        if (Quaternion.Angle(cinemachineCamera.transform.rotation, endRot) < stopRotateDistance)
            resetRot = false;

        if (resetRot)
        {
            cinemachineCamera.transform.rotation = Quaternion.Slerp(
                cinemachineCamera.transform.rotation,
                endRot,
                rotateSpeed * Time.deltaTime
            );
        }
    }

    private void CheckResetPos()
    {
        if (cinemachineCamera == null) { resetPos = false; return; }
        ////摄像机位置处理
        Vector3 end = GetResetPosition();
        if (end == null) { resetPos = false; return; }
        //若摄像机已经移动到指定位置则停止移动
        if (Vector3.Distance(cinemachineCamera.transform.position, end) < stopDistance)
        {
            resetPos = false;
        }
        //移动摄像机
        if (resetPos)
        {
            cinemachineCamera.transform.position = Vector3.Lerp(
                cinemachineCamera.transform.position,
                end,
                lerpSpeed * Time.deltaTime
            );
        }
    }
}


