using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardScaleController : MonoBehaviour
{
    [SerializeField] private Vector3 scale = Vector3.one;
    public void SetScale(Vector3 scale)
    {
        this.scale = scale;
    }
    public Vector3 GetScale()
    {
        return scale;
    }
    [SerializeField] private float lerpSpeed = 5.0f;
    public void SetLerpSpeed(float lerpSpeed)
    {
        this.lerpSpeed = lerpSpeed;
    }
    public float GetLerpSpeed()
    {
        return lerpSpeed;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Card card;
    void Start()
    {
        if(card == null) card = GetComponent<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTheCardScale();
    }

    private void ChangeTheCardScale()
    {
        if(card == null) return;
        card.transform.localScale = Vector3.Lerp(
            card.transform.localScale,
            scale,
            lerpSpeed * Time.deltaTime
        );
    }
}
