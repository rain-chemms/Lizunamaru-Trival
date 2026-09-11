using UnityEngine;

namespace GridObjectSystem
{
    public class GridObjectSatellite : GridObjectSatellitePositionSetter
    {
        [SerializeField] private GridObject center;//围绕哪一个中心物体进行旋转
        public void SetCenter(GridObject center) => this.center = center;
        public GridObject GetCenter() => center;

        protected override void OnEnable()
        {
            base.OnEnable();
            if(center == null) center = self;//默认自身为旋转中心,即不进行旋转
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
            Vector3 centerPos = (Vector3)center?.GetRigidBody()?.worldCenterOfMass;
            //计算目标位置
            Vector3 targetPos = centerPos + direction * radius;//direction * radius为旋转半径的空间表示
            //获取自身的刚体
            Rigidbody selfRb = self?.GetRigidBody();
            //设置自身位置
            selfRb?.MovePosition(targetPos);    
        }

    }
}
