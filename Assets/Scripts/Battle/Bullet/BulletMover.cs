using UnityEngine;

namespace BulletSystem
{
    [RequireComponent(typeof(Bullet))]
    public class BulletMover : MonoBehaviour
    {
        [SerializeField] private Bullet bullet;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            if (bullet == null) bullet = GetComponent<Bullet>();
            initOver = false;
        }

        private bool initOver = false;
        // Update is called once per frame
        void FixedUpdate()
        {
            if(!initOver)
            {
                MoveBullet_Init();
                initOver = true;
                return;
            }
            MoveBullet();
        }

        private void MoveBullet_Init()
        {
            if (bullet == null) return;
            if(!bullet.IsInitForceOpen()) return;//生命周期内受力非开启时返回
            Rigidbody rb = bullet.GetRigidBody();
            if (rb == null) return;
            rb.transform.forward = bullet.GetDirection();//设置子弹当前的方向
            rb.AddForce(bullet.GetDirection() * bullet.GetInitForce(), bullet.GetInitForceMode());
            //进行速度限制
            if (rb.linearVelocity.magnitude > bullet.GetMaxSpeed())
            {
                rb.linearVelocity = rb.linearVelocity.normalized * bullet.GetMaxSpeed();
            }
        }

        private void MoveBullet()
        {
            if (bullet == null) return;
            if(!bullet.IsLifeForceOpen()) return;//生命周期内受力非开启时返回
            Rigidbody rb = bullet.GetRigidBody();
            if (rb == null) return;
            rb.transform.forward = bullet.GetDirection();//设置子弹当前的方向
            //应用力的效果
            rb.AddForce(bullet.GetDirection() * bullet.GetForce(), bullet.GetForceMode());
            //进行速度限制
            if (rb.linearVelocity.magnitude > bullet.GetMaxSpeed())
            {
                rb.linearVelocity = rb.linearVelocity.normalized * bullet.GetMaxSpeed();
            }
        }
    }
}