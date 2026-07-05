using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class BulletMover : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bullet == null) bullet = GetComponent<Bullet>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        if(bullet == null) return;
        Rigidbody rb = bullet.GetRigidBody();
        if(rb == null)  return ;
        rb.transform.forward = bullet.GetDirection();//设置子弹当前的方向
        //应用力的效果
        rb.AddForce(bullet.GetDirection() * bullet.GetForce(), bullet.GetForceMode());
        //进行速度限制
        if(rb.linearVelocity.magnitude > bullet.GetMaxSpeed())
        {
            rb.linearVelocity = rb.linearVelocity.normalized * bullet.GetMaxSpeed();
        }
    }
}
