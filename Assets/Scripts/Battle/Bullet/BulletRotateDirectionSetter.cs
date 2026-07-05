using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class BulletRotateDirectionSetter : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 10f;
    public float GetLerpSpeed()
    {
        return lerpSpeed;
    }
    public void SetLerpSpeed(float lerpSpeed)
    {
        this.lerpSpeed = lerpSpeed;
    }
    
    [SerializeField] private Bullet bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bullet == null) bullet = GetComponent<Bullet>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeBulletDirection();
    }

    public void ChangeBulletDirection()
    {
        if(bullet == null) return ;
        Quaternion targetRotation = Quaternion.LookRotation(bullet.GetDirection().normalized);
        bullet.transform.rotation = Quaternion.Slerp(
            bullet.transform.rotation,
            targetRotation,
            Time.deltaTime * lerpSpeed
        );
    }
}
