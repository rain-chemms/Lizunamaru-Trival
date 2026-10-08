using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

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

        void OnEnable()
        {
            LoadFileData();
        }
        [Range(0, 2)][SerializeField] private int saveIndex = 0;//当前的存档索引
        private List<GamePlayData> gamePlayDatas = new List<GamePlayData>(3) { null, null, null };
        private List<GameHistory> gameHistories = new List<GameHistory>(3) { null, null, null };
        private List<GameSave> gameSaves = new List<GameSave>(3) { null, null, null };

        //通过文件名尝试初始化对应的数据
        public void LoadFileData()
        {
            string playDataStr = "game";
            string saveStr = "save";
            string historyStr = "history";
            //最多只有三个有效存档
            for (int i = 0; i < 3; i++)
            {
                //尝试解析字符串
                string pDp = Path.Combine(savePath, playDataStr, saveIndex.ToString() + ".json");
                string sDp = Path.Combine(savePath, saveStr, saveIndex.ToString() + ".json");
                string hDp = Path.Combine(savePath, historyStr, saveIndex.ToString() + ".json");


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

        public void SaveNowIndexDataToFile()
        {
            //获取数据类
            GameHistory nowHD = gameHistories[saveIndex];
            GamePlayData nowPD = gamePlayDatas[saveIndex];
            GameSave nowSD = gameSaves[saveIndex];
            //转化为Json字符串
            string hJson = JsonUtility.ToJson(nowHD, true);
            string pJson = JsonUtility.ToJson(nowPD, true);
            string sJson = JsonUtility.ToJson(nowSD, true);

            //获取存档路径
            string playDataStr = "game";
            string saveStr = "save";
            string historyStr = "history";
            string pDp = Path.Combine(savePath, playDataStr, saveIndex.ToString() + ".json");
            string sDp = Path.Combine(savePath, saveStr, saveIndex.ToString() + ".json");
            string hDp = Path.Combine(savePath, historyStr, saveIndex.ToString() + ".json");

            //写入历史数据
            if (nowHD != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pDp)); // 确保目录存在
                File.WriteAllText(pDp, pJson);
            }

            //写入存档数据
            if (nowSD != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(sDp)); // 确保目录存在
                File.WriteAllText(sDp, sJson);
            }

            //写入临时游玩数据
            if (nowPD != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(hDp)); // 确保目录存在
                File.WriteAllText(hDp, hJson);
            }
        }
        
        //创建新的存档文件,创建文件history和save
        //注: 开始一局新的游戏的时候才去创建playData,游戏失败时,将saveData存入对应的History并将history保存

        //删除某个索引的存档及其文件,文件包括history,playData和save
    }
}
