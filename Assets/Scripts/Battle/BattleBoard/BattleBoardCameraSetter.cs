using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(BattleBoard))]
public class BattleBoardCameraSetter : MonoBehaviour
{
    [SerializeField] private Vector3 posOffset = new Vector3(0, 0, 0);
    public Vector3 GetPosOffset()
    {
        return posOffset;
    }
    public void SetPosOffset(Vector3 posOffset)
    {
        this.posOffset = posOffset;
    }
    [SerializeField] private Vector3 rotOffset = new Vector3(0, 0, 0);
    public Vector3 GetRotOffset()
    {
        return rotOffset;
    }
    public void SetRotOffset(Vector3 rotOffset)
    {
        this.rotOffset = rotOffset;
    }
    [SerializeField] private bool resetPos = false;
    [SerializeField] private bool resetRot = false;
    public void ResetCamera()
    {
        resetPos = true;
        resetRot = true;
    }
    [SerializeField] private float lerpSpeed = 5f;
    public float GetLerpSpeed()
    {
        return lerpSpeed;
    }
    public void SetLerpSpeed(float lerpSpeed)
    {
        this.lerpSpeed = lerpSpeed;
    }
    [SerializeField] private float rotateSpeed = 5f;
    public float GetRotateSpeed()
    {
        return rotateSpeed;
    }
    public void SetRotateSpeed(float rotateSpeed)
    {
        this.rotateSpeed = rotateSpeed;
    }
    [SerializeField] private float stopDistance = 0.1f;//摄像机停止移动的距离
    [SerializeField] private float stopRotateDistance = 0.1f;
    [SerializeField] private BattleBoard board;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (board == null) board = GetComponent<BattleBoard>();
        AutoSetCineCamera();
        ResetCamera();//启动时重置摄像机
    }

    [SerializeField] private CinemachineCamera cinemachineCamera;
    public CinemachineCamera GetCineCamera()
    {
        return cinemachineCamera;
    }
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
            end = new Vector3(
                pos.x + (board.GetWidthAndHeight().x - 1) * gaps.x / 2,
                pos.y,
                pos.z - gaps.y / 2
            ) + posOffset + board.transform.position;
        }
        return end;
    }

    void Update()
    {
        CheckResetPos();
        CheckResetRot();
    }

    private void CheckResetRot()
    {
        ////摄像机旋转处理
        if (board == null) resetRot = false;
        Vector3 nowRot = cinemachineCamera.transform.rotation.eulerAngles;
        Quaternion endRot = Quaternion.Euler(
            rotOffset.x,
            rotOffset.y,
            0
        ) * board.transform.rotation;
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

    private void CheckResetPos()
    {
        if (cinemachineCamera == null) {resetPos = false;return;}
        ////摄像机位置处理
        Vector3 end = GetResetPosition();
        if (end == null) {resetPos = false;return;}
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


