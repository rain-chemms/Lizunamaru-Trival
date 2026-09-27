using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Reflection;
using System;

namespace CardSystem.CardPoolSystem
{
    //卡池实例:用于为卡牌预制体分类
    //正常来说多角色的卡牌游戏卡池应该按照角色进行分类
    public class CardPool : MonoBehaviour
    {
        //这个Label代表这张卡属于哪一个角色
        //这会依据当前物体的资源标签去获取卡牌
        [SerializeField] private string label = "AllHub";//当前卡池的Label,默认为AllHub,相同Label的不同卡池实体代表同一个逻辑卡池
        //label也是卡池的唯一标识符
        //AllHub代表所有角色都可以获取的卡牌,类似塔2里的白色卡(但是卡牌类别里面没有白卡这个分类)
        public string GetLabel() => label;
        public void SetLable(string lab) => label = lab;

        [Header("注意: cardPrefabDict禁止编辑\n使用SerializableDictionary只是方便调试")]
        [SerializeField] private SerializableDictionary<string,Card> cardPrefabDict = new SerializableDictionary<string, Card>();//当前卡池中存放的卡牌预制体
        public SerializableDictionary<string,Card> GetCardPrefabDict() => cardPrefabDict;

        //使用卡牌完整的命名空间+类名获取唯一的卡牌
        //这个是唯一标识符
        public Card GetCardPrefabByFullClassName(string fullName)
        {
            if((bool)cardPrefabDict?.Contains(fullName))
            {
                return cardPrefabDict[fullName];
            }
            return null;
        }

        //依据当前的Label自动从卡牌AddressableGroup中获取对用标签的卡牌
        //资源标签key的格式为
        //  标签1:"Card.Pool." + label.ToString()
        //  标签2:"Card"
        void OnEnable()
        {
            StartCoroutine(LoadCardPrefabList());
        }

        private IEnumerator LoadCardPrefabList()
        {
            //清空旧的条目
            cardPrefabDict.Clear();
            var keys = new List<string> { "Card", "Card.Pool." + label };
            //分类的时候不是按照卡牌的命名空间分类,而是依据其资源标签分到不同的卡池中
            //卡池存有这张卡的唯一标识符和其预制体
            // 1. 发起 Addressables 异步加载请求
            var handle = Addressables.LoadAssetsAsync<GameObject>(keys, null, Addressables.MergeMode.Intersection);
            // 2. 使用 yield return 等待加载完成（不阻塞主线程）
            yield return handle;
            // 3. 检查加载状态是否成功
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                List<GameObject> pfbs = handle.Result.ToList();

                foreach (GameObject p in pfbs)
                {
                    if (p == null) continue;
                    Card card = p.GetComponent<Card>();
                    if(card != null)
                    {
                        // 获取当前卡牌的完整类型作为键值
                        Type cTpye = card.GetType();
                        string cKey = cTpye.FullName;
                        //string cKey = cTpye.Namespace + "." +cTpye.Name;
                        if(!cardPrefabDict.Contains(cKey))//去除重复项
                        {
                            cardPrefabDict.Add(cKey,card);
                        }
                    }
                }
            }
            else
            {
                // 可选：打印加载失败的日志
                Debug.LogWarning($"[CardPool]: Card Prefab Load Faild: {handle.OperationException}");
            }
            // 4. 重要：释放句柄，防止内存泄漏
            handle.Release();
        }
    }
}
