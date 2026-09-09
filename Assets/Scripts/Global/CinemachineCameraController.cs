using UnityEngine;
using Unity.Cinemachine;

//战斗摄像机物体:需要是一个CinemachineCamera
namespace GlobalSystem
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class BattleCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        void OnEnable()
        {
            //初始化摄像机
            if (virtualCamera == null) virtualCamera = GetComponent<CinemachineCamera>();
        }

        [SerializeField] private Vector3 targetPos;//目标摄像机位置
        public void SetTargetPos(Vector3 targetPos) => this.targetPos = targetPos;
        public Vector3 GetTargetPos() => targetPos;

        [SerializeField] private float posLerpSpeed = 2.0f;//摄像机位置插值速度
        public void SetPosLerpSpeed(float posLerpSpeed) => this.posLerpSpeed = posLerpSpeed;
        public float GetPosLerpSpeed() => posLerpSpeed;

        [SerializeField] private Quaternion targetRot;//目标摄像机旋转
        public void SetTargetRot(Quaternion targetRot) => this.targetRot = targetRot;
        public Quaternion GetTargetRot() => targetRot;

        [SerializeField] private float rotLerpSpeed = 2.0f;//摄像机旋转插值速度
        public void SetRotLerpSpeed(float rotLerpSpeed) => this.rotLerpSpeed = rotLerpSpeed;
        public float GetRotLerpSpeed() => rotLerpSpeed;


        [SerializeField] private bool alwaysSync = false;//始终同步
        public bool IsAlwaysSync() => alwaysSync;
        public void SetAlwaysSync(bool alwaysSync) => this.alwaysSync = alwaysSync;

        [Header("检测器")]
        [SerializeField] private Vector3 posVelocity;// 帧率无关的 posVelocity
        public void Update()
        {
            if (alwaysSync && virtualCamera != null)
            {
                // 1. 使用 SmoothDamp 替代 Lerp，天然帧率无关且自带阻尼防抖
                virtualCamera.transform.position = Vector3.SmoothDamp(
                    virtualCamera.transform.position,
                    targetPos,
                    ref posVelocity,          // 需要在类中声明: private Vector3 posVelocity;
                    1f / Mathf.Max(posLerpSpeed, 0.001f), // 将速度转换为平滑时间，防止除零
                    float.MaxValue,           // maxSpeed 不限速
                    Time.deltaTime            // 必须传入 deltaTime
                );

                // 2. 旋转使用 Slerp + 帧率无关的 t 值计算
                float rotT = 1f - Mathf.Exp(-rotLerpSpeed * Time.deltaTime);
                virtualCamera.transform.rotation = Quaternion.Slerp(
                    virtualCamera.transform.rotation,
                    targetRot,
                    rotT
                );
            }
        }

        //瞬间设置摄像机
        public void InstantSetCamera()
        {
            if (virtualCamera != null)
            {
                virtualCamera.transform.position = targetPos;
                virtualCamera.transform.rotation = targetRot;
            }
        }
    }
}