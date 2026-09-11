using UnityEngine;

namespace GridObjectSystem
{
    public class GridSatellite : GridObjectSatellitePositionSetter
    {
        [SerializeField] private Vector2Int centerIndex = new Vector2Int(0, 0);//同步位置处中心格子索引
        public Vector2Int GetCenterIndex() => centerIndex;
        public void SetCenterIndex(Vector2Int index) => centerIndex = index;

        [SerializeField] private float heightOffset = 0;
        public float GetHeightOffset() => heightOffset;
        public void SetHeightOffset(float offset) => heightOffset = offset;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void SyncSelfPosition()
        {
            //必要的变量
            BattleBoard board = BattleBoard.instance;
            Vector3 boardPos = (Vector3)board?.transform.position;
            Vector3 _00Pos = (Vector3)board?.GetGrid00LocalPosition();
            Vector2 gaps = (Vector2)board?.GetGapsOfGrid();
            //计算中心点位置
            Vector3 centerPos = boardPos + _00Pos + new Vector3(centerIndex.x * gaps.x, heightOffset ,centerIndex.y * gaps.y);
            //计算目标位置
            Vector3 targetPos = centerPos + direction * radius;//direction * radius为旋转半径的空间表示
            //获取自身的刚体
            Rigidbody selfRb = self?.GetRigidBody();
            //设置自身位置
            selfRb?.MovePosition(targetPos);
        }
    }
}