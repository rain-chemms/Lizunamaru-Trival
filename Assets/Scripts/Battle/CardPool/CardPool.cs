using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using System.Collections;

namespace CardSystem.CardPoolSystem
{
    //卡池实例:用于为卡牌预制体分类
    //正常来说多角色的卡牌游戏卡池应该按照角色进行分类
    public class CardPool : MonoBehaviour
    {
        //这个Label代表这张卡属于哪一个角色
        [SerializeField] private string label = "AllHub";//当前卡池的Label,默认为AllHub,相同Label的不同卡池实体代表同一个逻辑卡池
        //AllHub代表所有角色都可以获取的卡牌,类似塔2里的白色卡(但是卡牌类别里面没有白卡这个分类)
        public string GetLabel() => label;
        public void SetLable(string lab) => label = lab;

        [SerializeField] private List<Card> cardPrefabList;//当前卡池中存放的卡牌预制体
        public List<Card> GetCardPrefabList() => cardPrefabList;
        public List<Card> GetCardPrefabList_Copy() => cardPrefabList.ToList();

        public Card GetCardPrefabByName(string name)
        {
            Card target = null;
            foreach (Card c in cardPrefabList.ToList())
            {
                if (c == null) continue;
                if (c.name.Equals(name))
                {
                    target = c;
                    break;
                }
            }
            return target;
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
            var keys = new List<string> { "Card", "Card.Pool." + label };
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
                    // 确保列表已初始化且不包含重复项
                    if (cardPrefabList != null && !cardPrefabList.Contains(card))
                    {
                        cardPrefabList.Add(card);
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
