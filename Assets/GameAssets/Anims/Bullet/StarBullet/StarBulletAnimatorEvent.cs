using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class StarBulletAnimatorEvent : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bullet == null) bullet = GetComponent<Bullet>();
    }

    public void AfterFlyOver()
    {
        //销毁游戏物体
        if(bullet!=null) Destroy(bullet.gameObject);
    }
}
