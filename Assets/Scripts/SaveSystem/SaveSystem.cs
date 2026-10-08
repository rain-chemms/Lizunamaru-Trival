using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

//游戏的存档系统:每当玩家需要存储临时进度或者是开启新的玩家档案的时候都会启动
//目录结构
/*
    --默认路径
        *.json
        --Saves
            --游戏版本号
                game_idx.json //玩家的临时游戏进度,游戏失败时,将内部数据存入history中并删除当前临时数据
                save_idx.json /玩家档案数据
                history_idx.json //
        --其他文件
*/
//是单例物体
namespace GameSaveSystem
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem instance;//单例对象
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
            //初始化存储路径
            savePath = Path.Combine(Application.persistentDataPath, "Saves", Application.version);
            //若存档路径不存在则创建存档对应的文件夹路径
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }

        [NonSerialized] private string savePath;

        void Start()
        {
            LoadFileData();
        }
        [Range(0, 2)][SerializeField] private int nowSaveIndex = 0;//当前的存档索引
        public int NowSaveIndex { get => nowSaveIndex; set => nowSaveIndex = value; }
        private List<GamePlayData> gamePlayDatas = new List<GamePlayData>(3) { null, null, null };
        public List<GamePlayData> GamePlayDatas {get => gamePlayDatas;}
        //public List<GamePlayData> GamePlayDatas_Copy {get => gamePlayDatas.ToList();}
        private List<GameHistory> gameHistories = new List<GameHistory>(3) { null, null, null };
        public List<GameHistory> GameHistories { get => gameHistories; }
        //public List<GameHistory> GameHistories_Copy { get => gameHistories.ToList(); }
        private List<GameSave> gameSaves = new List<GameSave>(3) { null, null, null };
        public List<GameSave> GameSaves { get => gameSaves; }
        //public List<GameSave> GameSaves_Copy { get => gameSaves.ToList(); }
        //一下为索引路径的前缀
        [NonSerialized] private string playDataStr = "game";
        [NonSerialized] private string saveStr = "save";
        [NonSerialized] private string historyStr = "history";
        //通过文件名尝试初始化对应的数据
        public void LoadFileData()
        {
            //最多只有三个有效存档
            for (int i = 0; i < 3; i++)
            {
                //尝试解析字符串
                string pDp = Path.Combine(savePath, playDataStr + i.ToString() + ".json");
                string sDp = Path.Combine(savePath, saveStr + i.ToString() + ".json");
                string hDp = Path.Combine(savePath, historyStr + i.ToString() + ".json");


                //解析GamePlayData
                if (!File.Exists(pDp)) gamePlayDatas[i] = null;
                else
                {
                    string json = File.ReadAllText(pDp);
                    if (!string.IsNullOrEmpty(json))
                    {
                        try
                        {
                            GamePlayData data = JsonUtility.FromJson<GamePlayData>(json);
                            gamePlayDatas[i] = data;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[SaveSystem] JSON Serialized <GamePlayData> Failed: {e.Message}" + $"\nFile Path: {pDp}");
                        }
                    }
                }

                //解析SavesData
                if (!File.Exists(sDp)) gameSaves[i] = null;
                else
                {
                    string json = File.ReadAllText(sDp);
                    if (!string.IsNullOrEmpty(json))
                    {
                        try
                        {
                            GameSave data = JsonUtility.FromJson<GameSave>(json);
                            gameSaves[i] = data;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[SaveSystem] JSON Serialized <GameSave> Failed: {e.Message}" + $"\nFile Path: {sDp}");
                        }
                    }
                }

                //解析HistoryData
                if (!File.Exists(hDp)) gameHistories[i] = null;
                else
                {
                    string json = File.ReadAllText(hDp);
                    if (!string.IsNullOrEmpty(json))
                    {
                        try
                        {
                            GameHistory data = JsonUtility.FromJson<GameHistory>(json);
                            gameHistories[i] = data;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[SaveSystem] JSON Serialized GameData Failed: {e.Message}" + $"\nFile Path: {hDp}");
                        }
                    }
                }
            }
        }

        public bool SaveExist(int index)
        {
            //获取存档路径
            string sDp = Path.Combine(savePath, saveStr + index.ToString() + ".json");
            return File.Exists(sDp);
        }

        public bool HistoryExist(int index)
        {
            //获取存档路径
            string hDp = Path.Combine(savePath, historyStr + index.ToString() + ".json");
            return File.Exists(hDp);
        }

        public bool GamePlayDataExist(int index)
        {
            //获取存档路径
            string pDp = Path.Combine(savePath, playDataStr + index.ToString() + ".json");
            return File.Exists(pDp);
        }

        public void SaveNowIndexDataToFile()
        {
            SaveDataToFile(nowSaveIndex);
        }
        
        public void SaveDataToFile(int index,bool writePlayData = true,bool writeHistory = true,bool writeSave = true)
        {
            //获取数据类
            GameHistory nowHD = gameHistories[index];
            GamePlayData nowPD = gamePlayDatas[index];
            GameSave nowSD = gameSaves[index];
            //转化为Json字符串
            string hJson = JsonUtility.ToJson(nowHD, true);
            string pJson = JsonUtility.ToJson(nowPD, true);
            string sJson = JsonUtility.ToJson(nowSD, true);

            //获取存档路径
            string pDp = Path.Combine(savePath, playDataStr + index.ToString() + ".json");
            string sDp = Path.Combine(savePath, saveStr + index.ToString() + ".json");
            string hDp = Path.Combine(savePath, historyStr + index.ToString() + ".json");

            //写入临时游玩数据
            if (nowPD != null && writePlayData)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pDp)); // 确保目录存在
                File.WriteAllText(pDp, pJson);
            }

            //写入存档数据
            if (nowSD != null && writeSave)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(sDp)); // 确保目录存在
                File.WriteAllText(sDp, sJson);
            }

            //写入历史数据
            if (nowHD != null && writeHistory)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(hDp)); // 确保目录存在
                File.WriteAllText(hDp, hJson);
            }
        }

        //创建新的存档文件,创建文件history和save
        //注: 开始一局新的游戏的时候才去创建playData,游戏失败时,将saveData存入对应的History并将history保存
        //只负责生成新的存档
        public void CreateANewSave(int index)
        {
            if (index < 0 || index >= 3)
            {
                Debug.Log("[SaveSystem]: The Index of Save To Create is Out of Range!");
                return;
            }
            string sDp = Path.Combine(savePath, saveStr + index.ToString() + ".json");
            string hDp = Path.Combine(savePath, historyStr + index.ToString() + ".json");
            //检查当前是否已经存在对应的存档文件了
            //若存在则报错返回
            bool saveExist = File.Exists(sDp);
            bool historyExist = File.Exists(hDp);
            if (saveExist && historyExist)
            {
                Debug.LogError("[SaveSystem]: You want to Destroy A Exist Save, it's Not Allow. Please Delete The Save First!");
                return;
            }
            else if (!historyExist)//优先检测存档
            {
                GameHistory newHistory = new GameHistory();
                string json = JsonUtility.ToJson(newHistory, true);
                Directory.CreateDirectory(Path.GetDirectoryName(hDp)); // 确保目录存在
                File.WriteAllText(hDp, json);
                Debug.Log($"Create new History Successful! File Path: {hDp}");
            }
            //
            if (!saveExist)
            {
                GameSave newSave = new GameSave();
                string json = JsonUtility.ToJson(newSave, true);
                Directory.CreateDirectory(Path.GetDirectoryName(sDp)); // 确保目录存在
                File.WriteAllText(sDp, json);
                Debug.Log($"Create new Save Successful! File Path: {sDp}");
            }
        }

        //删除某个索引的存档及其文件,文件包括history,playData和save
        //只负责删除对应存档
        public void DeleteSave(int index)
        {
            if (index < 0 || index >= 3)
            {
                Debug.Log("[SaveSystem]: The Index of Save is Out of Range, Delete Error!");
                return;
            }

            //获取存档路径
            string pDp = Path.Combine(savePath, playDataStr + nowSaveIndex.ToString() + ".json");
            string sDp = Path.Combine(savePath, saveStr + nowSaveIndex.ToString() + ".json");
            string hDp = Path.Combine(savePath, historyStr + nowSaveIndex.ToString() + ".json");

            //尝试将路径下文件删除
            try
            {
                //删除GamePlayData
                if (File.Exists(pDp))
                {
                    File.Delete(pDp);
                    Debug.Log($"[SaveSystem]: Delete GamePlayData: {pDp}");
                }
                else
                {
                    Debug.LogWarning($"[SaveSystem]: Not Find GamePlayData: {pDp}");
                }
                
                //删除GameSave
                if (File.Exists(sDp))
                {
                    File.Delete(sDp);
                    Debug.Log($"[SaveSystem]: Delete Gamesave: {sDp}");
                }
                else
                {
                    Debug.LogWarning($"[SaveSystem]: Not Find GamePlayData: {sDp}");
                }
                
                //删除GameHistory
                if (File.Exists(hDp))
                {
                    File.Delete(hDp);
                    Debug.Log($"[SaveSystem]: Delete GameHistory: {hDp}");
                }
                else
                {
                    Debug.LogWarning($"[SaveSystem]: Not Find GameHistory: {hDp}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem]:Delete File Error: {e.Message}");
            }

            //移除SaveSystem中存储的临时数据
            gameHistories[index] = null;
            gamePlayDatas[index] = null;
            gameSaves[index] = null;
        }
    }
}
