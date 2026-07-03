using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Rendering;

//必须是一个下滑UI物体才能显示
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Canvas))]
public class StackCardDisplayer : MonoBehaviour
{
    //牌堆卡牌显示器单例
    public static StackCardDisplayer instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private bool isDisplay = false;
    public void SetDisplay(bool isDisplay)
    {
        this.isDisplay = isDisplay;
    }
    public bool IsDisplay()
    {
        return isDisplay;
    }
    [SerializeField] private List<Card> cardList = new List<Card>();
    public void SetCardList(List<Card> cardList)
    {
        this.cardList = cardList.ToList();
    }
    public List<Card> GetCardList()
    {
        return cardList.ToList();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //键是临时生成的卡牌,值是原来的卡牌列表中的卡牌
    [SerializeField] private SerializedDictionary<Card,Card> sourceCardDict;
    public SerializedDictionary<Card,Card> GetSourceCardDict()
    {
        return sourceCardDict;
    }
    [SerializeField] private ScrollRect scrollRect;
    public ScrollRect GetScrollRect()
    {
        return scrollRect;
    }
    [SerializeField] private Canvas canvas;
    void Start()
    {
        //尝试自动获取
        if(scrollRect == null) scrollRect = GetComponentsInChildren<ScrollRect>()?.Where(x=>x.name == "DisplayArea")?.FirstOrDefault();
        if(canvas == null) canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndHiddenBattleMessageDisplayer();
    }

    private void CheckAndHiddenBattleMessageDisplayer()
    {
        BattleMessageDisplayer.instance?.GetComponent<BattleUIHider>()?.SetIsHidden(isDisplay);
    }

    public void OpenDisplayer()
    {
        if(scrollRect == null) return;
        if(sourceCardDict == null) return;
        sourceCardDict.Clear();//清空映射关系表
        RectTransform content = scrollRect.content;
        foreach (Card card in cardList)
        {
            if(card == null) continue;
            Card displayCard = Instantiate(card, content) as Card;
            //关闭不i要的组件
            displayCard.GetComponent<CardInsertSlotChecker>().enabled = false;
            displayCard.GetComponent<CardInStackChecker>().enabled = false;
            displayCard.GetComponent<CardPlayAreaChecker>().enabled = false;
            displayCard.GetComponent<CardReturnHandChecker>().enabled = false;
            displayCard.GetComponent<CardHandler>().enabled = false;
            displayCard.GetComponent<UnderCardUIChecker>().enabled = false;
            displayCard.GetComponent<CardSIdeChanger>().enabled = false;
            //设置不显示卡牌的在派对中的图像
            displayCard.GetComponent<CardDisplayer>()?.SetInStack(false);
            displayCard.GetComponent<CardDisplayer>()?.FlipTo(false);
            //设置显示卡牌的位置+旋转+缩放
            displayCard.transform.localPosition = Vector3.zero;
            displayCard.transform.localRotation = Quaternion.identity;
            displayCard.transform.localScale = Vector3.one;
            //设置Canvas排序
            Canvas disCavs = displayCard.GetComponent<Canvas>();
            if(disCavs!=null) disCavs.sortingOrder = (int)canvas?.sortingOrder + 100;
            sourceCardDict.Add(displayCard,card);//添加映射关系
            //将卡牌添加到显示区域
            displayCard.transform.SetParent(content);
        }
    }

    public void CloseDisplayer()
    {
        if(scrollRect == null) return;
        if(sourceCardDict == null) return;
        sourceCardDict.Clear();//清空映射关系表
        RectTransform content = scrollRect.content;
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}
