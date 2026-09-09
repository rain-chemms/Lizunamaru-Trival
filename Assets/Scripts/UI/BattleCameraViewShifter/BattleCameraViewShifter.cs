using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using GridObjectSystem.RoleSystem;
using GridObjectSystem.RoleSystem.PlayerSystem;


[RequireComponent(typeof(Button))]
public class BattleCameraViewShifter : MonoBehaviour
{
    [SerializeField] private Button button;
    void OnEnable()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(ShiftCameraView);
        index = 0;// 初始化索引器
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(ShiftCameraView);
    }

    void Start()
    {
        InitTheItem();
    }

    //默认要加入一些变换
    private void InitTheItem()
    {
        //首先,清除默认选项
        foreach(CameraViewItem item in cameraViewList.ToList())
        {
            if(item.IsDefaultItem()) cameraViewList.Remove(item);
        }
        //常用属性提取
        Vector2 size = (Vector2Int)BattleBoard.instance?.GetWidthAndHeight();
        Vector2 gaps = (Vector2)BattleBoard.instance?.GetGapsOfGrid();
        Vector2 center = (size - Vector2.one) / 2;
        float heightUnit = gaps.SqrMagnitude();//高度单位
        float highNum = (size.x > size.y ? size.x : size.y) / 3;
        bool canAdd = true;
        //加入新的项
        //1.底面观察图
        CameraViewItem bottomViewItem = new CameraViewItem();
        bottomViewItem.SetIsDefaultItem(true);
        bottomViewItem.syncToControlPlayerPos = false;
        bottomViewItem.syncToControlPlayerRot = false;
        bottomViewItem.shiftControlToFirstPerson = false;
        bottomViewItem.appendBoardOffset = new Vector2(size.x / 2, -1);
        bottomViewItem.posOffset = new Vector3(0, heightUnit * highNum - 9.5f, -10.0f);
        bottomViewItem.rotOffset = new Vector3(50, 0, 0);
        bottomViewItem.alwaysReset = false;
        bottomViewItem.lerpSpeed = 1.5f;
        bottomViewItem.rotateSpeed = 1.5f;
        //检测是否可添加
        canAdd = true;
        foreach(CameraViewItem item in cameraViewList)
        {
            if(item.Equals(bottomViewItem)) 
            {
                canAdd = false;
                break;
            }
        }
        if(canAdd) cameraViewList.Add(bottomViewItem);

        //2.中心区域俯视图
        CameraViewItem planFormItem = new CameraViewItem();
        planFormItem.SetIsDefaultItem(true);
        planFormItem.appendBoardOffset = center;
        planFormItem.posOffset = new Vector3(0, heightUnit * highNum + 5.0f, -3.0f);
        planFormItem.rotOffset = new Vector3(90, 0, 0);
        planFormItem.syncToControlPlayerPos = false;
        planFormItem.syncToControlPlayerRot = false;
        bottomViewItem.shiftControlToFirstPerson = false;
        planFormItem.alwaysReset = false;
        planFormItem.lerpSpeed = 1.5f;
        planFormItem.rotateSpeed = 1.5f;
        //检测是否可添加
        canAdd = true;
        foreach(CameraViewItem item in cameraViewList)
        {
            if(item.Equals(planFormItem)) 
            {
                canAdd = false;
                break;
            }
        }
        if(canAdd) cameraViewList.Add(planFormItem);
    }

    [Serializable]
    public struct CameraViewItem : IEquatable<CameraViewItem>
    {
        //相机偏移量
        public Vector3 posOffset;
        public Vector3 rotOffset;

        //是否同步玩家位置
        public bool syncToControlPlayerPos;
        public bool syncToControlPlayerRot;
        //是否切换玩家控制为第一人称模式
        public bool shiftControlToFirstPerson;
        //额外的棋盘偏移量
        public Vector2 appendBoardOffset;

        //同步属性设置
        public bool alwaysReset;
        public float rotateSpeed;
        public float lerpSpeed;

        //控制字段
        [SerializeField] private bool isDefaultItem;//是否为自动产生的默认项
        public bool IsDefaultItem() => isDefaultItem;
        public void SetIsDefaultItem(bool isDefaultItem) => this.isDefaultItem = isDefaultItem;

        //实现IEquatable<T>的强类型Equals(核心,无装箱)
        public bool Equals(CameraViewItem other)
        {
            return posOffset.Equals(other.posOffset) &&
                rotOffset.Equals(other.rotOffset) &&
                syncToControlPlayerPos == other.syncToControlPlayerPos &&
                appendBoardOffset.Equals(other.appendBoardOffset) &&
                alwaysReset == other.alwaysReset &&
                rotateSpeed.Equals(other.rotateSpeed) &&
                lerpSpeed.Equals(other.lerpSpeed);
        }

        //重写object.Equal(兼容多态,有装箱开销)
        public override bool Equals(object obj)
        {
            return obj is CameraViewItem other && Equals(other);
        }

        // 3.重写GetHashCode(相等对象必须有相同哈希)
        public override int GetHashCode()
        {
            return HashCode.Combine(posOffset, rotOffset, syncToControlPlayerPos, appendBoardOffset, alwaysReset, rotateSpeed, lerpSpeed);
        }


        public static bool operator ==(CameraViewItem left, CameraViewItem right) => left.Equals(right);
        public static bool operator !=(CameraViewItem left, CameraViewItem right) => !left.Equals(right);
    }

    // 切变列表
    [SerializeField] private List<CameraViewItem> cameraViewList = new List<CameraViewItem>();//struct结构体加进去之后就不好改了
    public List<CameraViewItem> GetCameraViewList() => cameraViewList;
    public void AddCameraView(CameraViewItem item) => cameraViewList.Add(item);

    //当前的索引器
    [SerializeField] private int index = 0;
    public int GetIndex() => index;
    public void SetIndex(int index) => this.index = index;

    private void ShiftCameraView()
    {
        if (index < 0) index = 0;
        int length = cameraViewList.Count;
        if (index >= length) index = length - 1;
        index = (index + 1) % length;
        CameraViewItem item = cameraViewList[index];
        ChangeCameraViewByItem(item);
    }

    private void ChangeCameraViewByItem(CameraViewItem item)
    {
        Vector3 posOffset = item.posOffset;
        Vector3 rotOffset = item.rotOffset;
        float rotateSpeed = item.rotateSpeed;
        float lerpSpeed = item.lerpSpeed;
        //获取BattleBoard单例中的CamerController
        BattleBoardCameraSetter cameraSetter = BattleBoard.instance?.GetComponent<BattleBoardCameraSetter>();
        //设置数据
        cameraSetter?.SetPosOffset(posOffset);
        cameraSetter?.SetRotOffset(rotOffset);
        cameraSetter?.SetRotateSpeed(rotateSpeed);
        cameraSetter?.SetLerpSpeed(lerpSpeed);
        cameraSetter?.SetAppendBoardOffset(item.appendBoardOffset);
        cameraSetter?.SetSyncToControlPlayerPos(item.syncToControlPlayerPos);
        cameraSetter?.SetSyncToControlPlayerRot(item.syncToControlPlayerRot);
        cameraSetter?.SetAlawysReset(item.alwaysReset);
        //触发切换
        cameraSetter?.ResetCamera();
        Debug.Log("[BattleCameraViewShifter]: Change Camera View To: " + item.ToString() + ", Now Shifter Index: " + index.ToString());
        //切换玩家控制器    
        //尝试获取玩家移动控制器组件
        PlayerMoveController controller = BattleMessage.instance?.GetControlPlayer().GetComponent<PlayerMoveController>();
        controller?.SetFirstPresentMode(item.shiftControlToFirstPerson);
    }
}
