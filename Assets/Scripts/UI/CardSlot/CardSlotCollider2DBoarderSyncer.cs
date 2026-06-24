using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CardSlot))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(RectTransform))]
public class CardSlotCollider2DBoarderSyncer : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private BoxCollider2D boxCollider2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //尝试自动获取
        if(rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if(boxCollider2D == null) boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColliderSize();
    }
    
    // 在UI大小改变时调用此方法（例如通过事件或Update监听）
    public void UpdateColliderSize()
    {
        if (rectTransform != null && boxCollider2D != null)
        {
            // 将碰撞体的中心对齐UI中心
            boxCollider2D.offset = rectTransform.rect.center; 
            // 将碰撞体大小设置为UI的宽高
            boxCollider2D.size = new Vector2(
                rectTransform.rect.width,
                rectTransform.rect.height
            ); 
        }
    }
}
