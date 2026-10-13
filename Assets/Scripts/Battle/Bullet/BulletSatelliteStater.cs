using UnityEngine;

namespace BulletSystem
{
    [RequireComponent(typeof(Bullet))]
    public class BulletSatelliteStater : MonoBehaviour
    {
        [SerializeField] private Bullet bullet;
        [SerializeField] private Rigidbody center;// 要围绕旋转的中心
        public Rigidbody Center { get => center; set => center = value; }
        [SerializeField] private float distanceToCenter = 5f;// 旋转半径
        public float DistanceToCenter { get => distanceToCenter;set => distanceToCenter = value;}
        [SerializeField] private float orbitSpeed = 90f;// 旋转速度（度/秒）
        public float OrbitSpeed { get => orbitSpeed; set => orbitSpeed = value;}
        [SerializeField] private float heighLerpSpeed = 1.0f;
        public float HeighLerpSpeed { get => heighLerpSpeed; set => heighLerpSpeed = value; }
        [SerializeField] private Vector3 offsetHeight = Vector3.zero;
        public Vector3 OffsetHeight { get => offsetHeight ; set => offsetHeight = value; }
        
        [SerializeField] private float angleOffset = 0;
        public float AngleOffset { get => angleOffset; set => angleOffset = value; }
        private float orbitAngle = 0f; // 当前轨道角度 
        void OnEnable()
        {
            if (bullet == null) bullet = GetComponent<Bullet>();
            if (center == null) center = bullet?.GetComponent<Rigidbody>();
            BulletMover mover = bullet?.GetComponent<BulletMover>();
            if(mover != null) mover.enabled = false;
            BulletRotateDirectionSetter dirSetter = bullet.GetComponent<BulletRotateDirectionSetter>();
            if(dirSetter != null) dirSetter.enabled = false;
        }

        void Start()
        {
            //初始化初始角度
            orbitAngle = angleOffset;
        }

        void FixedUpdate()
        {
            SyncOrbit();
            SyncDirection();
        }

        /// 让子弹围绕 center 做圆周运动
        private void SyncOrbit()
        {
            Rigidbody rb = bullet?.GetRigidBody();
            if (rb == null || center == null) return;

            // 1. 每帧累加角度
            orbitAngle += orbitSpeed * Time.fixedDeltaTime;

            // 2. 用 sin/cos 计算圆周上的目标位置（XZ 平面）
            Vector3 centerPos = center.worldCenterOfMass;
            float targetX = centerPos.x + Mathf.Cos(orbitAngle * Mathf.Deg2Rad) * distanceToCenter;
            float targetZ = centerPos.z + Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * distanceToCenter;

            // 3. 高度方向用 lerp 平滑过渡到 center 的高度
            float currentY = rb.worldCenterOfMass.y;
            float targetY = centerPos.y + offsetHeight.y;
            float y = Mathf.Lerp(currentY, targetY, Time.fixedDeltaTime * heighLerpSpeed);

            // 4. 应用偏移并设置位置
            Vector3 targetPos = new Vector3(targetX, y, targetZ) + offsetHeight;
            rb.transform.position = targetPos;
        }

        /// 让子弹面向运动切线方向
        private void SyncDirection()
        {
            Rigidbody rb = bullet?.GetRigidBody();
            if (rb == null) return;

            // 圆周运动的切线方向 = 角度 + 90度
            float tangentAngle = orbitAngle * Mathf.Deg2Rad + Mathf.PI / 2f;
            Vector3 tangent = new Vector3(-Mathf.Sin(tangentAngle), 0f, Mathf.Cos(tangentAngle));

            // 逆时针旋转时切线方向取反
            if (orbitSpeed < 0)
                tangent *= -1f;

            if (tangent.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(tangent);
                rb.transform.rotation = Quaternion.Slerp(
                    rb.transform.rotation,
                    targetRotation,
                    Time.fixedDeltaTime * 10f
                );
            }
        }
    }
}