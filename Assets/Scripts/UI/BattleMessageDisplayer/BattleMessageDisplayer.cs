using UnityEngine;
using TMPro;

//战斗UI显示器,必须挂在含有Canvas的组件上
[RequireComponent(typeof(Canvas))]
public class BattleMessageDisplayer : MonoBehaviour
{
    public static BattleMessageDisplayer instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private bool isShow = false;///是否显示
    public bool IsShow() => isShow;
    public void SetShow(bool isShow) => this.isShow = isShow;
    [SerializeField] private Canvas mainCanvas;///相关联的主画布
    public Canvas GetMainCanvas() => mainCanvas;
    [SerializeField] private CardPlayArea cardPlayArea;///卡牌打出区域,里面存有当前是否已经打完牌的信息isExcuting
    public CardPlayArea GetCardPlayArea() => cardPlayArea;
    public void OnEnable()
    {
        //尝试自动获取
        if(mainCanvas == null) mainCanvas = GetComponent<Canvas>();
        if(cardPlayArea == null) cardPlayArea = GetComponentInChildren<CardPlayArea>();//尝试从自身及子集脚本中获取
    }

    public void Update()
    {
        //更新UI的显示状态
        if(mainCanvas?.enabled != isShow) mainCanvas.enabled = isShow;
        //依据战斗信息实时更新UI显示
    }
}
