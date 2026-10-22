using System;
using UnityEngine;

//用于多个Spell显示时将单独的Image设置一定的Offset
[RequireComponent(typeof(SpellAttackDisplayItem))]
public class SpellAttackDisplayItemOffsetLerper : MonoBehaviour
{
    [SerializeField] private SpellAttackDisplayItem item;
    [SerializeField] private float lerpSpeed = 0.5f;
    public float LerpSpeed { get => lerpSpeed; set => lerpSpeed = value; }
    void OnEnable()
    {
        if(item == null) item = GetComponent<SpellAttackDisplayItem>();
    }

    // Update is called once per frame
    void Update()
    {
        LerpSelf();
    }

    [NonSerialized] private readonly Vector2 center = new Vector2(0.5f,0.5f); 
    private void LerpSelf()
    {
        if(item != null)
        {
            Vector2 offset = item.Offset;
            RectTransform imgRtf = item.RoleSpellImage?.GetComponent<RectTransform>();
            RectTransform textRtf = item.SpellText?.GetComponent<RectTransform>();
            //中心位置固定为(0.5,0.5)
            if(imgRtf != null)
            {
                imgRtf.pivot = Vector2.Lerp(imgRtf.pivot,center + offset,lerpSpeed * Time.deltaTime);    
            }

            if(textRtf != null)
            {
                textRtf.pivot = Vector2.Lerp(textRtf.pivot,center + offset,lerpSpeed * Time.deltaTime);    
            }
        }
    }
}
