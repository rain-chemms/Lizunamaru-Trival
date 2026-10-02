using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

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
            foreach(LevelMessageList ls in GetComponentsInChildren<LevelMessageList>().ToList())
            {
                if(ls == null) continue;
                if((bool)allLevelMessages?.Contains(ls)) continue;
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
            foreach(MapNodeCategory ctg in Enum.GetValues(typeof(MapNodeCategory)))
            {
                bool haveCtg = false;
                foreach(LevelMessageList ls in allLevelMessages)
                {
                    if(ls == null) continue;
                    if(ls.GetManageCategory() == ctg)
                    {
                        haveCtg = true;
                        break;
                    }
                }
                if(!haveCtg)
                {
                    //产生一个新的游戏物体
                    GameObject newObject = new GameObject(ctg.ToString() + "_LoadInMessageList");
                    LevelMessageList msgLs = newObject.AddComponent<LevelMessageList>();
                    msgLs.SetManagerCategory(ctg);
                    newObject.transform.SetParent(gameObject.transform);//设置其父物体为自身
                }
            }
        }
    }
}
