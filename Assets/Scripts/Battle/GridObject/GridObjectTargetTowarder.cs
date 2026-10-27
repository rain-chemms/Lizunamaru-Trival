using System;
using UnityEngine;

namespace GridObjectSystem
{
    //该脚本动态禁用GridObjectDirectionSetter组件
    //其LerpSpeed使用GridObjectDirectionSetter组件的LerpSpeed值
    [RequireComponent(typeof(GridObject))]
    [RequireComponent(typeof(GridObjectDirectionSetter))]
    public class GridObjectTargetTowarder : MonoBehaviour
    {
        [SerializeField] private GridObject self;
        [SerializeField] private GridObjectDirectionSetter gods;
        [NonSerialized] private float lerpSpeed = 1.0f;
        void OnEnable()
        {
            if(self == null) self = GetComponent<GridObject>();
            if(gods == null) gods = GetComponent<GridObjectDirectionSetter>();
            if(gods != null) lerpSpeed = gods.GetLerpSpeed();
        }
        [SerializeField] private GridObject target;
        public GridObject Target { get => target; set => target = value; }
        
        void Update()
        {
            CheckAndOpenGODS();
            LerpToTarget();
        }

        public void CheckAndOpenGODS()
        {
            if(gods == null) return;
            if(target == null) gods.enabled = true;
            else gods.enabled = false;
        }
        private const float stopAngleThreshold = 0.01f;
        public void LerpToTarget()
        {
            lerpSpeed = (float)gods?.GetLerpSpeed();
            if(target == null || self == null) return;
            Rigidbody sRb = self?.GetRigidBody();
            Rigidbody tRb = target?.GetRigidBody();
            Vector3 selfPos = sRb != null ? sRb.worldCenterOfMass : self.transform.position;
            Vector3 targetPos = tRb != null ? tRb.worldCenterOfMass : target.transform.position;
            Vector3 direction = (targetPos - selfPos).normalized;
            // 计算当前旋转角度和目标旋转角度
            Quaternion currentRot = sRb.transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(direction);
            // 角度差足够小,直接snap到目标方向,避免无限逼近抖动
            if (Quaternion.Angle(currentRot, targetRot) < stopAngleThreshold)
            {
                sRb.transform.rotation = targetRot;
                return;
            }
            // 执行Lerp旋转
            sRb.transform.rotation = Quaternion.Lerp(
                currentRot,
                targetRot, 
                lerpSpeed * Time.deltaTime
            );
        }
    }
}