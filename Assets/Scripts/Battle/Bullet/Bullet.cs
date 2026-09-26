using UnityEngine;

namespace BulletSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private int pierce = 1;//子弹的剩余的穿透数
        public void SetPierce(int pierce) => this.pierce = pierce;
        public int GetPierce() => pierce;
        
        [SerializeField] private float lifeTime = 0.0f;//生命时间,小于等于0.0时代表不会自动消失
        public float GetLifeTime() => lifeTime;
        public void SetLifeTime(float lifeTime) => this.lifeTime = lifeTime;
        
        [SerializeField] private float lifeRecorder = 0.0f;
        public float GetLifeRecorder() => lifeRecorder;
        
        [SerializeField] private bool side = true;//子弹所属的阵营,默认为玩家阵营
        public void SetSide(bool side) => this.side = side;
        public bool GetSide() => side;
        
        [SerializeField] private float damage = 0.0f;//伤害
        public void SetDamage(float damage) => this.damage = damage;
        public float GetDamage() => damage;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private Rigidbody rb;
        public Rigidbody GetRigidBody() => rb;
        
        [SerializeField] private Vector3 scale = Vector3.one;//子弹的缩放比例
        public void SetScale(Vector3 scale) => this.scale = scale;
        public Vector3 GetScale() => new Vector3(scale.x, scale.y, scale.z);
        
        [SerializeField] private Vector3 direction = Vector3.zero;//子弹的飞行方向
        public void SetDirection(Vector3 direction) => this.direction = direction;
        public Vector3 GetDirection() => new Vector3(direction.x, direction.y, direction.z);
        [Header("\t")]
        [Header("受力系统选项")]
        [Header("1.生命周期内持续受力")]
        [Header("开关:为true时持续受力才能生效")]
        [SerializeField] private bool lifeForceOpen = true;//是否只是添加一次受力
        public bool IsLifeForceOpen() => lifeForceOpen;
        public void SetLifeForceOpen(bool open) => lifeForceOpen = open;

        [SerializeField] private float force = 0.0f;//子弹的飞行受力
        public void SetForce(float force) => this.force = force;
        public float GetForce() => force;

        [SerializeField] private float maxSpeed = 0.0f;//最大速度限制
        public void SetMaxSpeed(float maxSpeed) => this.maxSpeed = maxSpeed;
        public float GetMaxSpeed() => maxSpeed;
        
        [SerializeField] private ForceMode forceMode = ForceMode.Force;//弹道受力模式
        public void SetForceMode(ForceMode forceMode) => this.forceMode = forceMode;
        public ForceMode GetForceMode() => forceMode;

        [Header("2.初始化时的受力模式")]
        [SerializeField] private bool initForceOpen = false;
        public bool IsInitForceOpen() => initForceOpen;
        public bool SetInitForceOpen(bool open) => initForceOpen = open; 
        
        [SerializeField] private float initForce = 0.0f;
        public float GetInitForce() => initForce;
        public void SetInitForce(float fs) => initForce = fs;

        [SerializeField] private ForceMode initForceMode = ForceMode.Impulse;//初始化力时用到的受力模式
        public ForceMode GetInitForceMode() => initForceMode;
        public void SetInitForceMode(ForceMode initMode) => initForceMode = initMode;
        [Header("\t")]
        [SerializeField] private bool canDefend = true;//是否可以被护盾抵消
        public bool CanDefend() => canDefend;
        public void SetCanDefend(bool canDefend) => this.canDefend = canDefend;

        void OnEnable()
        {
            //尝试自动获取
            if (rb == null) rb = GetComponent<Rigidbody>();
            lifeRecorder = 0.0f;
        }

        void Update()
        {
            //生命时间流逝
            lifeRecorder += Time.deltaTime;
        }

    }
}