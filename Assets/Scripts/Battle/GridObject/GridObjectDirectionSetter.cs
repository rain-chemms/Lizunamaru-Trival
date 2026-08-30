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
        void Update()
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

        private void LerpRoleDirection()
        {
            if (gridObject == null) return;
            gridObject.transform.rotation = Quaternion.Lerp(
                gridObject.transform.rotation,//当前方向
                Quaternion.LookRotation(target),//目标方向
                lerpSpeed * Time.deltaTime//插值速度
            );
        }
    }
}