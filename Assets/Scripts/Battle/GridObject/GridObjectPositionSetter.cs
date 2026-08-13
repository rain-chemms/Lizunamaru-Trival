using UnityEngine;

namespace GridObjectSystem
{
    [RequireComponent(typeof(GridObject))]
    public class GridObjectPositionSetter : MonoBehaviour
    {
        [SerializeField] private float landHeightOffset = 0.0f;//地面高度Y的偏移量
        public float GetLandHeightOffset() => landHeightOffset;
        public void SetLandHeightOffset(float offset) => landHeightOffset = offset;
        [SerializeField] private float flyHeight;//飞行时的高度
        public void SetFlyHeight(float flyHeight) => this.flyHeight = flyHeight;
        public float GetFlyHeight() => flyHeight;
        [SerializeField] private float grandYOffset;//地面高度Y的偏移量
        public void SetGrandYOffset(float offset) => grandYOffset = offset;
        public float GetGrandYOffset() => grandYOffset;
        [SerializeField] private GridObject gridObject;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnnEnable()
        {
            //尝试自动获取
            if (gridObject == null) gridObject = GetComponent<GridObject>();
            SetRoleParentToBattleBoard();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            ChhangeLocalPositionByRoleData();
        }

        //将物体的父物体设置为战斗的棋盘
        protected void SetRoleParentToBattleBoard()
        {
            if (gridObject == null) return;
            gridObject.GetRigidBody()?.transform.SetParent(BattleBoard.instance?.transform);
        }

        protected void ChhangeLocalPositionByRoleData()
        {
            if (gridObject == null) return;
            BattleBoard btb = BattleBoard.instance;
            if (btb == null) return;
            Rigidbody rb = gridObject.GetRigidBody();
            if (rb == null) return;
            Vector2Int index = gridObject.GetGridIndex();
            Vector3 _00Pos = btb.GetGrid00LocalPosition();
            //_00Pos += btb.transform.position;
            Vector2 _gaps = btb.GetGapsOfGrid();
            bool isFly = gridObject.IsFly();
            //实时计算role的相对位置
            float height = _00Pos.y + grandYOffset;
            float xPos = index.x * _gaps.x + _00Pos.x;
            float zPos = index.y * _gaps.y + _00Pos.z;
            height += landHeightOffset;
            if (isFly) height += flyHeight;
            //设置玩家位置
            rb.transform.localPosition = Vector3.Lerp(
                rb.transform.localPosition,
                new Vector3(xPos, height, zPos),
                gridObject.GetSpeed() * Time.fixedDeltaTime
            );
        }
    }

}