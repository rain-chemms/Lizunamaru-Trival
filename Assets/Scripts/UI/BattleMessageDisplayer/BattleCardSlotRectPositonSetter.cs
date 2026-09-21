using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CardSystem;
using System.Linq;

//用于读取所有的卡槽列表,并依据卡槽的索引设置卡槽的UI位置
//必须挂载在BattleMessageDisplayer上
[RequireComponent(typeof(BattleMessageDisplayer))]
public class BattleCardSlotRectPositonSetter : MonoBehaviour
{
    [System.Serializable]
    public struct PreSetOfSlotView
    {
        public float offsetY;
        public float offsetX;
        public float darknessV;
        public float alphaScale;
        public float anchorLerpSpeed;
        public Vector3 scaleOfSlot;
        public float scaleLerpSpeed;
        public float overLengthOffsetFactor;
        public PreSetOfSlotView(float offsetY, float offsetX, float darknessV, float alphaScale, float anchorLerpSpeed, Vector3 scaleOfSlot, float scaleLerpSpeed, float overLengthOffsetFactor)
        {
            this.offsetY = offsetY;
            this.offsetX = offsetX;
            this.darknessV = darknessV;
            this.alphaScale = alphaScale;
            this.anchorLerpSpeed = anchorLerpSpeed;
            this.scaleOfSlot = scaleOfSlot;
            this.scaleLerpSpeed = scaleLerpSpeed;
            this.overLengthOffsetFactor = overLengthOffsetFactor;
        }
    }

    [Header("当前超出的卡槽列表时错开的偏移量")]
    [SerializeField] private float offsetY = 0.1f;
    [SerializeField] private float offsetX = 0.025f;
    [Range(0.0f,1.0f)] [SerializeField] private float darknessV = 0.25f;//暗淡值:降低的亮度
    [Range(0.0f,1.0f)] [SerializeField] private float alphaScale = 0.5f;//透明度缩放
    [SerializeField] private float anchorLerpSpeed = 5.0f;
    [Range(-1.0f, 1.0f)] [SerializeField] private float overLengthOffsetFactor = 0.5f;//超出卡槽列表时,偶数卡牌的X的额外偏移量因子 
    [SerializeField] private Vector3 scaleOfSlot = Vector3.one;//卡槽的缩放比例
    [SerializeField] private float scaleLerpSpeed = 5.0f;//卡槽缩放的插值速度

    [SerializeField] private List<PreSetOfSlotView> preSetOfSlotViews = new List<PreSetOfSlotView>();//卡槽列表外观预设列表
    public List<PreSetOfSlotView> GetPreSetOfSlotViews() => preSetOfSlotViews;
    public List<PreSetOfSlotView> GetPreSetOfSlotViews_Copy() => preSetOfSlotViews.ToList();
    
    [SerializeField] private int preSetIndex = 0;//预设索引
    public void LoopThePreSetIndex()
    {   
        int length = (int)preSetOfSlotViews?.Count;
        preSetIndex = (preSetIndex + 1) % length;
    }
    
    public void UseNowPreSet()
    {
        int length = (int)preSetOfSlotViews?.Count;
        int idx = Mathf.Clamp(preSetIndex, 0, length - 1);
        PreSetOfSlotView preSet = preSetOfSlotViews[idx];
        offsetY = preSet.offsetY;
        offsetX = preSet.offsetX;
        darknessV = preSet.darknessV;
        alphaScale = preSet.alphaScale;
        anchorLerpSpeed = preSet.anchorLerpSpeed;
        scaleOfSlot = preSet.scaleOfSlot;
        scaleLerpSpeed = preSet.scaleLerpSpeed;
        overLengthOffsetFactor = preSet.overLengthOffsetFactor;
    }

    void OnEnable()
    {
        InitColorDict();
        preSetIndex = 0;
        UseNowPreSet();
    }

    void Update()
    {
        SetCardSlotRectPosition();
    } 
    
    //设置卡槽的UI位置
    public void SetCardSlotRectPosition()
    {
        //获取全部的卡槽列表
        List<CardSlotList> cardSlotListList = BattleMessage.instance.GetCardSlotListList();
        //遍历所有卡槽列表,并将它们依次排序
        foreach (CardSlotList cardSlotList in cardSlotListList)
        {
            CheckAllSlotAndSetPosition(cardSlotList);
        }   
    }

    private void DarkCardSlotColor(CardSlot cardSlot)
    {
        if(cardSlot != null)
        {
            Image image = cardSlot.GetComponent<Image>();
            Color source = image.color;
            if(slotsColorDict.ContainsKey(cardSlot))
            {
                //切换获取源颜色
                source = slotsColorDict[cardSlot];
            }
            else//不存在则以当前Image的颜色进行加入
            {
                slotsColorDict.Add(cardSlot, source);
            }
            //设置颜色
            Color.RGBToHSV(source, out float h, out float s, out float v);
            v = Mathf.Clamp01(v - darknessV);
            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = source.a * alphaScale;
            image.color = newColor;        
        }
    }

    //CardSlot原始Image的颜色值字典
    private Dictionary<CardSlot, Color> slotsColorDict = new Dictionary<CardSlot, Color>(); 
    private void InitColorDict()
    {
        //清空旧的字典数据
        slotsColorDict.Clear();
        //获取全部的卡槽列表
        List<CardSlotList> cardSlotListList = BattleMessage.instance?.GetCardSlotListList();
        foreach(CardSlotList cardSlotList in cardSlotListList?.ToList())
        {
            if(cardSlotList == null) continue;
            List<CardSlot> cardSlotList1 = cardSlotList?.GetCardSlotList();
            if(cardSlotList1 == null || cardSlotList1.Count <= 0) continue;
            foreach(CardSlot cardSlot in cardSlotList1)
            {
                if(cardSlot == null) continue;
                Image image = cardSlot?.GetComponent<Image>();
                if(image!=null)
                {
                    Color color = image.color;
                    if(slotsColorDict.ContainsKey(cardSlot)) slotsColorDict[cardSlot] = color;
                    else slotsColorDict.Add(cardSlot, color);
                }
            }
        }
    }

    //count为参考个数
    //返回值代表当前卡槽是否设置完毕
    private void CheckAllSlotAndSetPosition(CardSlotList cardSlotList)
    {
        float allLength = 0.0f;//所有卡槽的长度总和 ,>1时代表长度超长了,需要缩位处理
        
        foreach(CardSlot cardSlot in cardSlotList.GetCardSlotList())
        {
            /*1.设置卡槽的Anchor位置*/
            RectTransform crtf = cardSlot?.GetComponent<RectTransform>();
            Vector2 Min = crtf.anchorMin;
            Vector2 Max = crtf.anchorMax;
            //获取卡槽X方向的min-max Anchor的位置,获取其长度
            float length = Max.x - Min.x;//当前卡槽占有的长度
            //Y方向固定设置为min = 0,max = 1
            //设置min
            crtf.anchorMin = Vector2.Lerp(
                crtf.anchorMin,
                new Vector2(
                    allLength,
                    0.0f//y方向最小为0%
                ),
                anchorLerpSpeed * Time.deltaTime
            );
            //设置max
            crtf.anchorMax = Vector2.Lerp(
                crtf.anchorMax,
                new Vector2(
                    allLength + length,
                    1.0f//y方向最大为100%,确保y方向占满
                ),
                anchorLerpSpeed * Time.deltaTime
            );
            allLength += length;
        }

        //过长时的缩放变换
        //长度超长了,需要缩位处理 =>更换每张卡片的起始Min.x的位置
        if(allLength > 1.0f)
        {
            //缩位置的比例
            float scale = 1.0f / allLength;
            //对起始点应用缩放处理
            int index = 0;
            foreach (CardSlot cardSlot in cardSlotList.GetCardSlotList())
            {
                RectTransform crtf = cardSlot?.GetComponent<RectTransform>();
            
                Vector2 Min = crtf.anchorMin;
                Vector2 Max = crtf.anchorMax;
                
                crtf.anchorMin = Vector2.Lerp(
                    crtf.anchorMin,
                    new Vector2(
                        Min.x * scale + ((int)(index / (int)2)) * offsetX + index % 2 * overLengthOffsetFactor,
                        Min.y - index % 2 * offsetY
                    ),
                    anchorLerpSpeed * Time.deltaTime
                );
                crtf.anchorMax = Vector2.Lerp(
                    crtf.anchorMax,
                    new Vector2(
                        Max.x - (Min.x - Min.x * scale) + ((int)(index / (int)2)) * offsetX + index % 2 * overLengthOffsetFactor,
                        Max.y - index % 2 * offsetY
                    ),
                    anchorLerpSpeed * Time.deltaTime
                );
                //微调卡牌的亮度,使其亮度降低
                if(index % 2 == 1)
                {
                    DarkCardSlotColor(cardSlot);
                }
                //调整图层顺序
                Canvas parentCvs = null;
                if(cardSlot != null) parentCvs = cardSlot?.transform?.parent?.GetComponent<Canvas>();
                if (cardSlot != null)
                {
                    int order = parentCvs == null ? -100 : (int)parentCvs?.sortingOrder;
                    cardSlot?.SetLayerOrder(order + 1 + index);
                }
                //设置卡槽的缩放
                cardSlot.transform.localScale = Vector3.Lerp(
                    cardSlot.transform.localScale,
                    scaleOfSlot,
                    scaleLerpSpeed * Time.deltaTime
                );
                index++;//索引递增
            }
        }
    }    
}
