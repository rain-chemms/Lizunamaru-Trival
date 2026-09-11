using UnityEngine;

namespace GridObjectSystem
{
    public enum SatelliteSyncType
    {
        XOZ,//绕Y轴旋转
        XOY,//绕Z轴旋转
        YOZ,//绕X轴旋转
    }

    [RequireComponent(typeof(GridObject))]
    public abstract class GridObjectSatellitePositionSetter : MonoBehaviour
    {
        [SerializeField] protected bool clockwise = true;//顺时针还是逆时针
        public bool GetClockwise() => clockwise;
        public void SetClockwise(bool newClockwise) => clockwise = newClockwise;

        [SerializeField] protected float speed = 90;// 度/秒
        public float GetSpeed() => speed;
        public void SetSpeed(float newSpeed) => speed = newSpeed;

        [SerializeField] protected SatelliteSyncType syncType = SatelliteSyncType.XOZ;//旋转类型
        public SatelliteSyncType GetSyncType() => syncType;
        public void SetSyncType(SatelliteSyncType newType) => syncType = newType;

        [SerializeField] protected float radius = 1.0f;//旋转半径
        public float GetRadius() => radius;
        public void SetRadius(float newRadius) => radius = newRadius;

        [SerializeField] protected Quaternion rotateOffset;//旋转角度偏移
        public Quaternion GetRotateOffset() => rotateOffset;
        public void SetRotateOffset(Quaternion newOffset) => rotateOffset = newOffset;
        //自身GridObject
        [SerializeField] protected GridObject self;
        protected virtual void OnEnable()
        {
            if(self == null) self = GetComponent<GridObject>();
        }

        [SerializeField] protected float angle;//当前的旋转角度,可设置初始角度
        [SerializeField] protected Vector3 direction;//当前的旋转方向向量
        private void CaculateDirection()
        {
            //计算当前角度
            
            if(angle % 360 != 0) angle %= 360;//限制范围(-360)~360度
            float delatAngle = speed * Time.deltaTime;
            if(clockwise) angle -= delatAngle;
            else angle += delatAngle;
            float rad = angle * Mathf.Deg2Rad;
            switch(syncType)
            {
                case SatelliteSyncType.XOY:
                    direction = new Vector3(Mathf.Cos(rad),Mathf.Sin(rad),0);
                    break;
                case SatelliteSyncType.YOZ:
                    direction = new Vector3(0,Mathf.Cos(rad),Mathf.Sin(rad));
                    break;
                case SatelliteSyncType.XOZ:
                default:
                    direction = new Vector3(Mathf.Cos(rad),0,Mathf.Sin(rad));
                    break;
            }
            direction = rotateOffset * direction;//旋转角度偏移
            direction.Normalize();
        }

        protected virtual void Update()
        {
            CaculateDirection();//计算旋转方向向量
        }

        //继承脚本实现具体的卫星同步逻辑
        protected abstract void SyncSelfPosition();

        protected virtual void FixedUpdate()
        {
            SyncSelfPosition();//同步自身位置
        }
    }
}