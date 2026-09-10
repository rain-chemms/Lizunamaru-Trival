using UnityEngine;
using GlobalSystem;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace BulletSystem
{

    [RequireComponent(typeof(Bullet))]
    [RequireComponent(typeof(BulletDamageTrigger))]
    public class BulletExplosionGenerator : MonoBehaviour
    {
        [SerializeField] private Bullet explosionPrefab;//爆炸预制体
        public void SetExplosionPrefab(Bullet bt) => explosionPrefab = bt;
        public Bullet GetExplosionPrefab() => explosionPrefab;
        //关联的子弹伤害触发器
        [SerializeField] private Bullet bullet;
        [SerializeField] private BulletDamageTrigger damageTrigger;
        void OnEnable()
        {
            if(bullet == null) bullet = GetComponent<Bullet>();
            if(damageTrigger == null) damageTrigger = GetComponent<BulletDamageTrigger>();
            //关联爆炸产生
            if(damageTrigger != null) damageTrigger.EnterTrigger += GenerateExplosion;
        }

        void OnDisable()
        {
            //取消关联
            if(damageTrigger != null) damageTrigger.EnterTrigger -= GenerateExplosion;
        }

        private void GenerateExplosion(Collider other = null)
        {
            //产生新的爆炸物体
            Bullet explosion = Instantiate(explosionPrefab,null);//设置其父物体为空
            //设置它的属性
            explosion?.SetSide((bool)bullet?.GetSide());//关联爆炸物的阵营
            explosion?.SetDirection((Vector3)bullet?.GetDirection());//关联移动方向
            //同步物体的位置
            Rigidbody eRb = explosion?.GetRigidBody();
            Rigidbody bRb = bullet?.GetRigidBody();
            //获取接触点位置
            Vector3 closestPoint = Vector3.zero;
            if(bRb != null)
            {
                closestPoint = bRb.worldCenterOfMass;    
            }
            else
            {
                closestPoint = bullet.transform.position;
            }
            
            Collider bCld = bullet.GetComponent<Collider>();
            if(other != null && bCld != null)
            {
                closestPoint = bCld.ClosestPoint(other.transform.position);            
            }
            
            if(eRb != null)
            {
                eRb.transform.position = closestPoint;
            }
            else
            {
                explosion.transform.position = closestPoint;
            }
        }
    }
}