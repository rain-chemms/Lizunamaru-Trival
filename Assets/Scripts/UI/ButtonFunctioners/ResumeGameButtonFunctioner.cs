using System.Linq;
using GameSaveSystem;
using LevelLoadSystem;
using MapSystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResumeGameButtonFunctioner : MonoBehaviour
{
    [SerializeField] private Button button;
    public Button GetButton() => button;

    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        button?.onClick.AddListener(ResumeGame);
    }
    
    void OnDisable()
    {
        button?.onClick.RemoveListener(ResumeGame);
    }

    void Update()
    {
        //实时检测当前选择的玩家档案是否可以继续游戏
        bool canResume = SaveSystem.instance.GamePlayDataExist(SaveSystem.instance.NowSaveIndex);
        if(button != null) button.interactable = canResume;
    }

    public void ResumeGame()
    {
        //读取存档继续游戏
        SaveSystem.instance.LoadFileData();
        GamePlayData data = SaveSystem.instance.GamePlayDatas[SaveSystem.instance.NowSaveIndex];//获取游玩数据
        //重构地图
        SeedSetter.instance.SetSeed(data.seed);//设置种子
        MapLoader.instance.RecurMap(
            (int)SeedSetter.instance?.GetSeed_Int(),
            data.mapSize,
            data.mapArea,
            data.mapIndex
        );
        //依据当前的卡牌存储信息设置PlayerCardHub
        
        /*
            测试代码,将当前的卡牌信息存入Data中
        */
        //data.cardHub = PlayerCardHub.instance.GetFullNameListOfCardHub();
        //SaveSystem.instance.SaveDataToFile(SaveSystem.instance.NowSaveIndex);
        PlayerCardHub.instance.GenerateItemByNameWithLevel(data.cardHub);
        //依据当前玩家位置加载合适的关卡
        Vector2Int index = (Vector2Int)Map.instance?.GetPlayerPos();
        //尝试获取当前索引的MapNode的种类
        MapNodeCategory ctg = (MapNodeCategory)Map.instance?.GetNodeList()?.Where(x => x.GetIndex() == data.mapIndex)?.FirstOrDefault()?.GetCategory();
        StartCoroutine(LevelManager.instance.LoadRandomLevel(index,ctg));
        
    }
}
