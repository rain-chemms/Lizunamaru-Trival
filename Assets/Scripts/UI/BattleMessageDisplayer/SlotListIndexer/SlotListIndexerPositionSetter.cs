using UnityEngine;

[RequireComponent(typeof(SlotListIndexer))]
public class SlotListIndexerPositionSetter : MonoBehaviour
{
    [SerializeField] private SlotListIndexer indexer;
    void OnEnable()
    {
        if(indexer == null) indexer = GetComponent<SlotListIndexer>();
    }

    void Update()
    {
        //更新UI的显示位置
        LerpPosition(GetTargetPosition());
    }

    [SerializeField] private Vector2 offsetPresentOfIndexer;
    public Vector2 GetOffsetPresentOfIndexer() => offsetPresentOfIndexer;
    public void SetOffsetPresentOfIndexer(Vector2 offset) => offsetPresentOfIndexer = offset;
    [SerializeField] private float lerpSpeed = 10f;
    public void SetLerpSpeed(float speed) => lerpSpeed = speed;
    public float GetLerpSpeed() => lerpSpeed;
    private Vector3 GetTargetPosition()
    {
        CardSlot slot = indexer?.GetIndexSlot();    
        RectTransform rtf = slot?.GetComponent<RectTransform>();
        Vector3 target = Vector3.zero;
        if(rtf != null)
        {
            //获取目标位置
            target = rtf.position;
            float width = rtf.rect.width * rtf.lossyScale.x;   // 实际宽度
            float height = rtf.rect.height * rtf.lossyScale.y; // 实际高度(长度)
            target.x += width * offsetPresentOfIndexer.x;// 应用x偏移量
            target.y += height * offsetPresentOfIndexer.y;// 应用y偏移量
        }
        return target;
    }

    //进行位移操作
    private void LerpPosition(Vector3 target)
    {
        if(indexer!=null)
        {
            indexer.transform.position = Vector3.Lerp(
                transform.position, 
                target, 
                lerpSpeed * Time.deltaTime
            );
        }
    }
}
            
