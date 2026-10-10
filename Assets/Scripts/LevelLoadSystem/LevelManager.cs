using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;
using LevelLoadSystem.LoadInMessageSystem;
using GridObjectSystem.RoleSystem;
using GridObjectSystem.GadgetSystem;
using CardSystem;
using GridObjectSystem;
using MapSystem;
using Unity.VisualScripting;
using GameSaveSystem;

//关卡控制器
//控制有关战斗内关卡数据的加载,角色敌人的初始化
//采用单例模式
namespace LevelLoadSystem
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [SerializeField] private List<LevelMessageList> allLevelMessages = new List<LevelMessageList>();//管理的所有MessageList
        void OnEnable()
        {
            //尝试自动获取
            foreach (LevelMessageList ls in GetComponentsInChildren<LevelMessageList>().ToList())
            {
                if (ls == null) continue;
                if ((bool)allLevelMessages?.Contains(ls)) continue;
                allLevelMessages?.Add(ls);
            }
        }

        void Start()
        {
            FillTheNotExistCategoryList();
        }
        //补全还未加入的类别
        private void FillTheNotExistCategoryList()
        {
            foreach (MapNodeCategory ctg in Enum.GetValues(typeof(MapNodeCategory)))
            {
                bool haveCtg = false;
                foreach (LevelMessageList ls in allLevelMessages)
                {
                    if (ls == null) continue;
                    if (ls.GetManageCategory() == ctg)
                    {
                        haveCtg = true;
                        break;
                    }
                }
                if (!haveCtg)
                {
                    //产生一个新的游戏物体
                    GameObject newObject = new GameObject(ctg.ToString() + "_LoadInMessageList");
                    LevelMessageList msgLs = newObject.AddComponent<LevelMessageList>();
                    msgLs.SetManagerCategory(ctg);
                    newObject.transform.SetParent(gameObject.transform);//设置其父物体为自身
                    //加入管理器中
                    allLevelMessages.Add(msgLs);
                }
            }
        }

        //依据关卡类别,所处的地图区域和当前玩家的位置选择合适的随机项进行加载
        public IEnumerator LoadRandomLevel(Vector2Int mapIndex ,MapNodeCategory nodeCategory)
        {
            yield return null;
            int layer = mapIndex.y;
            int seed = (int)SeedSetter.instance?.GetSeed_Int();
            System.Random random = new System.Random(seed);
            //筛选目标的加载列表
            LevelMessageList targetMessageList = null;
            foreach(var msLs in allLevelMessages.ToList())
            {
                if(msLs.GetManageCategory() == nodeCategory)
                {
                    targetMessageList = msLs;
                    break;
                }
            }
            /*
            targetMessageList = allLevelMessages.ToList()
                .Where(
                    x => x.GetManageCategory() == nodeCategory
                )
                .FirstOrDefault();
            */
            //筛选符合玩家位置的ScriptableObject
            List<LevelLoadInMessage> targets = targetMessageList
                .GetMessageList()
                .Where(
                    x => 
                        !(bool)x?.GetAreaFilter()?.Contains((MapAreaCategory)Map.instance?.GetMapArea())
                    && (int)x?.GetLayerRestrict() <= layer
                )
                .ToList();

            switch (nodeCategory)
            {
                //从target中通过种子随机获取一个关卡进行加载
                case MapNodeCategory.BATTLE_NORMAL:
                case MapNodeCategory.BATTLE_BOSS:
                case MapNodeCategory.BATTLE_ELITE:
                    int count = targets.Count;
                    int index = random.Next(0,count);
                    BattleLoadInMessage msg = count > 0 ? targets[index] as BattleLoadInMessage : null;
                    yield return LodeInBattle(msg);//加载对应的关卡
                    break;
                default:
                    //其他节点的加载逻辑还没做
                    break;
            }
            //将地图锁定并将进度跳转到下一关
            Map.instance.MapLocked = true;
            Map.instance.SetPlayerPos(mapIndex);
            //保存当前进度到SaveSystem和文件系统
            if(!(bool)SaveSystem.instance?.GamePlayDataExist(SaveSystem.instance.NowSaveIndex))
            {
                SaveSystem.instance.CreateANewGamePlayData(SaveSystem.instance.NowSaveIndex);
                SaveSystem.instance.LoadFileData();
            }
            SaveSystem.instance.GamePlayDatas[SaveSystem.instance.NowSaveIndex].mapIndex = mapIndex;
            SaveSystem.instance.SaveDataToFile(SaveSystem.instance.NowSaveIndex,true,false,false);
        }
        //一下为LoadRandomLevel服务的私有函数
        //主要扩展功能
        /*
            1.根据BattleLodeInMessage信息重置场景,包括棋盘格的信息
        */
        public IEnumerator LodeInBattle(BattleLoadInMessage loadInMessage)
        {
            //确保参数有效
            if (loadInMessage == null)
            {
                Debug.LogError("[LevelManager]: BattleLodeInMessage is null, Please Check!");
                yield break;;
            }
            BattleBoard board = BattleBoard.instance;
            if (board == null)
            {
                Debug.LogError("[LevelManager]: BattleBoard is null, Please Check the Instance is really exist!");
                yield break;
            }
            //一下部分为清除已有的战斗场景信息
            //清除战斗信息中的所有角色(包括控制的玩家)
            List<Role> roles = BattleMessage.instance?.GetRoleList();
            foreach (Role role in roles?.ToList())
            {
                if (role != null)
                {
                    Destroy(role.gameObject);
                }
            }
            roles?.Clear();
            //清除所有道具
            List<Gadget> gadgets = BattleMessage.instance?.GetGadgetList();
            foreach(Gadget gd in gadgets)
            {
                if (gd != null)
                {
                    Destroy(gd.gameObject);
                }
            }
            gadgets.Clear();

            //清空卡牌列表
            List<Card> tempList = BattleMessage.instance?.GetHandCardList();
            foreach (Card card in tempList)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            tempList?.Clear();

            tempList = BattleMessage.instance?.GetDiscardCardList();
            foreach (Card card in tempList)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            tempList?.Clear();

            tempList = BattleMessage.instance?.GetDrawCardList();
            foreach (Card card in tempList)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            tempList.Clear();

            //清除卡槽中的卡牌
            List<CardSlot> cardSlots = BattleMessage.instance?.GetAllCardSlot();
            foreach (CardSlot cardSlot in cardSlots.ToList())
            {
                if (cardSlot == null) continue;
                if (cardSlot.GetInnerCard() != null)
                {
                    Destroy(cardSlot.GetInnerCard().gameObject);
                    cardSlot.SetInnerCard(null);
                }
            }

            
            //刷新卡槽信息
            BattleMessage.instance?.ResetCardSlotListFromBattleMessageDisplayer();
            //初始化卡槽数据
            var sllCtr = BattleMessageDisplayer.instance?.GetComponent<BattleCardSlotListController>();
            yield return sllCtr?.SetCardSlotListSortingLayer();
            yield return sllCtr?.DeleteAllCardSlotCategoryNotMatch();
            yield return sllCtr?.FreshCardSlotListCount();
            
            
            //重新依据当前信息设置并生成棋盘格
            board.SetWidthAndHeight(loadInMessage.GetWidthAndHeight());
            board.SetGapsOfGrid(loadInMessage.GetGapsOfGrids());
            board.SetGrid00LocalPosition(loadInMessage.GetGrid00LocalPosition());
            board.SetEmptyGridIndex(loadInMessage.GetEmptyGridsIndex());
            //清空BattleBoard中的棋盘格
            BattleBoardController bCtr = board.GetComponent<BattleBoardController>();
            bCtr.ResetTheBattleGrids();
            board.GetComponent<BattleBoardPhysicBoarderController>()?.ResizeTheBoundaryByBoardSize();//设置子弹的物理边界

            //一下部分为依据LodeInMessage信息初始化战斗场景
            int id_append = 0;
            //这部分需要从玩家信息获取器中实时读取玩家信息
            //信息包括:玩家的默认阵营,玩家血量,玩家最大生命值,玩家金币量,玩家卡牌列表+未加入的其他控制信息

            //初始化玩家角色,依据lodeInMessage设置玩家位置,并设置控制的玩家ID为当前玩家
            Role playerPrefab = BattleMessage.instance?.GetRoleChoiceData()?.RolePrefab;
            Role player = null;
            if(playerPrefab != null) player = Instantiate(playerPrefab,board.transform);
            player?.SetSide(true);
            player?.SetID((uint)id_append);
            player?.SetRoundOperateEnd(false);
            id_append++;
            if(player != null) 
            {
                BattleMessage.instance?.SetControlPlayerID((uint)player?.GetID());
                BattleMessage.instance?.GetRoleList()?.Add(player);
            }
            //设置玩家的数据
            player?.SetGridIndex(loadInMessage.GetPlayerStartIndex());
            
            //某些遗物可以在这里产生效果

            //初始化敌人及其位置
            foreach (KeyValuePair<Vector2Int, Role> role_pair in loadInMessage?.GetEnermyDict()?.ToList())
            {
                if (role_pair.Value == null) continue;//敌人角色为空时跳过
                Role role = Instantiate(role_pair.Value, board.transform);//向棋盘中加入角色
                //初始化敌人角色位置
                role?.SetGridIndex(role_pair.Key);
                //初始化敌人角色ID
                role?.SetID((uint)id_append);
                //初始化敌人角色的阵营,不同于玩家角色
                role?.SetSide(player == null ? false : !(bool)player?.GetSide());
                role?.SetRoundOperateEnd(false);
                //将敌人加入列表
                BattleMessage.instance?.GetRoleList()?.Add(role);
                id_append++;
            }
            
            //接下来,战斗就正式开始了
            //加载战斗场景
            SceneLoader.instance?.LoadScene("BattleScene",AppendLoadInBattle);
        }

        private void AppendLoadInBattle()
        {
            StartCoroutine(WaitAndSyncCamera());
            //开启所有和战斗相关的单例物体
            ConcentratePoint.instance?.gameObject.SetActive(true);
            BattleMessage.instance?.gameObject.SetActive(true);
            BattleMessageDisplayer.instance?.gameObject.SetActive(true);
            RoundChangeDisplayer.instance?.gameObject.SetActive(true);
            BattleBoard.instance?.gameObject.SetActive(true);
            //设置当前回合为玩家回合
            BattleMessage.instance.SetIsPlayerTurn(true);
            //初始化玩家的手牌
            PlayerCardHub.instance?.InitCardToBattle();
            //抽一个回合的牌
            StartCoroutine(BattleMessage.instance?.DrawCard((int)BattleMessage.instance?.GetDrawCardPreRound()));
            BattleMessage.instance.SetRound(0);
        }

        //等待新场景中 MainCamera 完成初始化后,再同步摄像机数据
        private IEnumerator WaitAndSyncCamera()
        {
            float timeout = 3f;
            float elapsed = 0f;
            while (Camera.main == null && elapsed < timeout)
            {
                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
            if (Camera.main == null)
            {
                Debug.LogError("[LevelManager]: WaitAndSyncCamera timeout, Main Camera is still null!");
                yield break;
            }
            //再等一帧,让 CinemachineBrain 有时间确定 ActiveVirtualCamera
            yield return null;
            BattleBoard.instance?.GetComponent<BattleBoardCameraSetter>()?.AutoSetCineCamera();
        }

    }
}
