using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private int pierce = 1;//子弹的剩余的穿透数
    public void SetPierce(int pierce)
    {
        this.pierce = pierce;
    }
    public int GetPierce()
    {
        return pierce;
    }
    [SerializeField] private float lifeTime = 0.0f;//生命时间,小于等于0.0时代表不会自动消失
    public float GetLifeTime()
    {
        return lifeTime;
    }
    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }
    [SerializeField] private float lifeRecorder = 0.0f;
    public float GetLifeRecorder()
    {
        return lifeRecorder;
    }

    [SerializeField] private bool side = true;//子弹所属的阵营,默认为玩家阵营
    public void SetSide(bool side)
    {
        this.side = side;
    }
    public bool GetSide()
    {
        return side;
    }
    [SerializeField] private float damage = 0.0f;//伤害
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public float GetDamage()
    {
        return damage;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Rigidbody rb;
    public Rigidbody GetRigidBody()
    {
        return rb;
    }
    [SerializeField] private Vector3 scale = Vector3.one;//子弹的缩放比例
    public void SetScale(Vector3 scale)
    {
        this.scale = scale;
    }
    public Vector3 GetScale()
    {
        return new Vector3(scale.x,scale.y,scale.z);
    }
    [SerializeField] private Vector3 direction = Vector3.zero;//子弹的飞行方向
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
    public Vector3 GetDirection()
    {
        return new Vector3(direction.x,direction.y,direction.z);
    }
    [SerializeField] private float force = 0.0f;//子弹的飞行受力
    public void SetForce(float force)
    {
        this.force = force;
    }
    public float GetForce()
    {
        return force;
    }
    [SerializeField] private float maxSpeed = 0.0f;
    public void SetMaxSpeed(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }
    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
    [SerializeField] private ForceMode forceMode = ForceMode.Force;//弹道受力模式
    public void SetForceMode(ForceMode forceMode)
    {
        this.forceMode = forceMode;
    }
    public ForceMode GetForceMode()
    {
        return forceMode;
    }
    void Start()
    {
        //尝试自动获取
        if(rb == null) rb = GetComponent<Rigidbody>();
        lifeRecorder = 0.0f;
    }
    void Update()
    {
        //生命时间流逝
        lifeRecorder += Time.deltaTime;
    }

}
