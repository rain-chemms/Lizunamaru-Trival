using UnityEngine;

[RequireComponent(typeof(Bullet))]
public class BulletScaleSetter : MonoBehaviour
{
    [SerializeField] private Bullet bullet;
    [SerializeField] private float lerpSpeed = 2f;
    public void SetLerpSpeed(float lerpSpeed)
    {
        this.lerpSpeed = lerpSpeed;
    }
    public float GetLerpSpeed()
    {
        return lerpSpeed;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(bullet == null) bullet = GetComponent<Bullet>();
    }
    

    // Update is called once per frame
    void Update()
    {
        SyncScale();
    }

    private void SyncScale()
    {
        if(bullet == null) return;
        Vector3 scale = bullet.GetScale();
        //同步本地缩放大小
        bullet.transform.localScale = Vector3.Lerp(
            bullet.transform.localScale,
            scale,
            lerpSpeed * Time.deltaTime
        );
    }
}
