using UnityEngine;

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
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private bool isShow = false;///是否显示
    public void SetShow(bool isShow)
    {
        this.isShow = isShow;
    }

    [SerializeField] private Canvas mainCanvas;///相关联的主画布
    public void Start()
    {
        //尝试自动获取
        if(mainCanvas == null) mainCanvas = GetComponent<Canvas>();
    }

    
    public void Update()
    {
        //更新UI的显示状态
        if(mainCanvas?.enabled != isShow) mainCanvas.enabled = isShow;
    
    }


}
