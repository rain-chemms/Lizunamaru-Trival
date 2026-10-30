using System.Linq;
using GameSaveSystem;
using GridObjectSystem.RoleSystem;
using LevelLoadSystem;
using MapSystem;
using RoleChoiceUISystem;
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
        //读取存档继续游戏:总而言之,就是从文件中读取了一遍,并依据其中的信息重构地图,再将重建之后的数据从新写入文件中
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
        PlayerCardHub.instance.GenerateItemByNameWithLevel(data.cardHub);
        //设置选择的玩家初始数据
        RoleChoiceData roleChoiceData = RoleChoiceDataManager.instance.GetRoleChoiceDataByName(data.selectRole);
        if(roleChoiceData != null)
        {
            BattleMessage.instance.SetRoleChoiceData(roleChoiceData);
        }
        //同步金币
        BattleMessage.instance.SetCoins(data.coins);

        //角色 当前生命值|最大生命值 相关的部分还没有同步
        Role player = BattleMessage.instance?.GetControlPlayer();
        if(player == null)
        {
            //依据RoleChoiceData创建一个新的玩家
            player = Instantiate(roleChoiceData.RolePrefab,BattleBoard.instance?.transform);
            player.SetSide(true);
            player?.SetID(0);
            BattleMessage.instance?.SetControlPlayerID((uint)player?.GetID());
            BattleMessage.instance?.GetRoleList()?.Add(player);
        }
        //设置玩家的数据
        float hp = data.currentHp;
        float maxHp = data.maxHp;
        if(hp > maxHp) hp = maxHp;
        player?.SetMaxHp(maxHp);
        player?.SetHp(hp);                
        player?.SetGridIndex(Vector2Int.zero);
        player?.SetRoundOperateEnd(false);

        /*
            测试代码,将当前的卡牌信息存入Data中
        */
        //data.cardHub = PlayerCardHub.instance.GetFullNameListOfCardHub();
        //SaveSystem.instance.SaveDataToFile(SaveSystem.instance.NowSaveIndex);
        //依据当前玩家位置加载合适的关卡
        Vector2Int index = (Vector2Int)Map.instance?.GetPlayerPos();
        //尝试获取当前索引的MapNode的种类
        MapNodeCategory ctg = (MapNodeCategory)Map.instance?.GetNodeList()?.Where(x => x.GetIndex() == data.mapIndex)?.FirstOrDefault()?.GetCategory();
        StartCoroutine(LevelManager.instance.LoadRandomLevel(index,ctg));
    }
}
