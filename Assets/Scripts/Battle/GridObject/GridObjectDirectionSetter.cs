using System;
using System.Linq;
using UnityEngine;

namespace GridObjectSystem
{
    [RequireComponent(typeof(GridObject))]
    public class GridObjectDirectionSetter : MonoBehaviour
    {
        [SerializeField] private GridObject gridObject;

        void OnEnable()
        {
            if (gridObject == null) gridObject = GetComponent<GridObject>();
            if (gridObject != null)
            {
                gridObject.SetDirection(gridObject.GetDirection());
                gridObject.directionChangeAction += CaculateRoleDirection;
            }
        }

        void OnDisable()
        {
            if (gridObject != null)
            {
                gridObject.directionChangeAction -= CaculateRoleDirection;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            LerpRoleDirection();
        }

        [SerializeField] private Vector3 target = Vector3.forward;
        [SerializeField] private float lerpSpeed = 6;
        public float GetLerpSpeed() => lerpSpeed;
        public void SetLerpSpeed(float newLerpSpeed) => lerpSpeed = newLerpSpeed;

        private void CaculateRoleDirection()
        {
            if (gridObject == null) return;
            BattleDirection direction = gridObject.GetDirection();
            target = Vector3.zero;
            switch (direction)
            {
                case BattleDirection.UP:
                    target = Vector3.forward;
                    break;
                case BattleDirection.DOWN:
                    target = Vector3.back;
                    break;
                case BattleDirection.LEFT:
                    target = Vector3.left;
                    break;
                case BattleDirection.RIGHT:
                default:
                    target = Vector3.right;
                    break;
            }
        }

        [SerializeField] private float stopAngleThreshold = 0.5f;   // 角度阈值，小于此值直接snap
        [SerializeField] private float minTargetMagnitude = 0.01f;   // 目标向量最小长度，防止零向量

        private void LerpRoleDirection()
        {
            if (gridObject == null) return;
            Rigidbody rb = gridObject.GetRigidBody();
            if (rb == null) return;

            // 1. 目标向量太短，说明没有有效方向，不旋转
            if (target.sqrMagnitude < minTargetMagnitude * minTargetMagnitude)
                return;

            Quaternion currentRot = rb.transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(target);

            // 2. 角度差足够小，直接snap到目标方向，避免无限逼近抖动
            if (Quaternion.Angle(currentRot, targetRot) < stopAngleThreshold)
            {
                rb.transform.rotation = targetRot;
                return;
            }

            // 3. 正常Lerp旋转
            rb.transform.rotation = Quaternion.Lerp(
                currentRot,
                targetRot,
                lerpSpeed * Time.fixedDeltaTime
            );
        }
    }
}