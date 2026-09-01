using UnityEngine;

namespace BulletSystem
{
    [RequireComponent(typeof(Bullet))]
    public class BulletRotateDirectionSetter : MonoBehaviour
    {
        [SerializeField] private bool syncDirectionOpen = true;//同步方向开启
        public bool IsSyncDirectionOpen() => syncDirectionOpen;
        public void SetSyncDirectionOpen(bool open) => syncDirectionOpen = open;

        [SerializeField] private float lerpSpeed = 10f;
        public float GetLerpSpeed() => lerpSpeed;
        public void SetLerpSpeed(float lerpSpeed) => this.lerpSpeed = lerpSpeed;
        

        [SerializeField] private Bullet bullet;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            if (bullet == null) bullet = GetComponent<Bullet>();
        }

        void Start()
        {
            //开始时立即切换方向
            ChangeBulletDirection_Instant();
        }

        // Update is called once per frame
        void Update()
        {
            if(syncDirectionOpen) ChangeBulletDirection();
        }

        public void ChangeBulletDirection()
        {
            if (bullet == null) return;
            Quaternion targetRotation = Quaternion.LookRotation(bullet.GetDirection().normalized);
            bullet.transform.rotation = Quaternion.Slerp(
                (Quaternion)bullet.GetRigidBody()?.transform.rotation,
                targetRotation,
                Time.deltaTime * lerpSpeed
            );
        }

        //可供外界调用,同时在游戏物体Start()中调用
        //立即设置到旋转方向
        public void ChangeBulletDirection_Instant()
        {
            if (bullet == null) return;
            Quaternion targetRotation = Quaternion.LookRotation(bullet.GetDirection().normalized);
            bullet.transform.rotation = targetRotation;            
        }
    }
}