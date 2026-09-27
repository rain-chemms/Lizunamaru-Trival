using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardSystem.CardPoolSystem
{
    //卡池管理器:用于自动获取所有卡池并为外界提供获取接口
    //为单例对象
    public class CardPoolManager : MonoBehaviour
    {
        public static CardPoolManager instance;
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

        [SerializeField] private List<CardPool> cardPools;
        public List<CardPool> GetCardPools() => cardPools;
        public List<CardPool> GetCardPools_Copy() => cardPools.ToList();

        //获取所有的CardPool的子物体
        void OnEnable()
        {
            List<CardPool> pools = GetComponentsInChildren<CardPool>()?.ToList();
            foreach (CardPool pool in pools)
            {
                if (pool == null) continue;
                if ((bool)cardPools?.Contains(pool)) continue;
                //如果有重复的卡池,合并并删除当前卡池
                bool isRepeat = false;
                foreach (CardPool p in cardPools?.ToList())
                {
                    //存在卡池的键值重复
                    if (p == null) continue;
                    if (p.GetLabel().Equals(pool))
                    {
                        var pDict = p.GetCardPrefabDict();
                        //合并卡池内容
                        foreach (var kv in pool.GetCardPrefabDict())
                        {
                            string key = kv.Key;
                            Card vPrefab = kv.Value;
                            if (!(bool)pDict?.Contains(key))//是非重复键值的卡牌
                            {
                                pDict.Add(key, vPrefab);
                            }
                        }
                        //销毁这个卡池的游戏实体
                        Destroy(pool);
                        isRepeat = true;
                        break;
                    }
                }
                if (isRepeat) continue;
                cardPools?.Add(pool);
            }
        }

        /// <summary>
        /// 尝试从一个指定标签label的卡池中获取键值为key的卡牌
        /// </summary>
        /// <param name="poolLabel">要在哪个卡池中搜索</param>
        /// <param name="cardKey">卡牌的键值,是其完整的类名(命名空间.类名)</param>
        /// <returns>卡牌预制体</returns>
        public Card GetCardPrefabFormPool(string poolLabel, string cardKey)
        {
            //搜索所有相同标签目标卡池
            CardPool targetPool = cardPools.Where(x => x.GetLabel().Equals(poolLabel)).FirstOrDefault();
            if (targetPool == null) return null;//所要寻找的卡池不存在,返回一张空卡
            var dict = targetPool.GetCardPrefabDict();
            Card target = null;
            if ((bool)dict?.Contains(cardKey))
            {
                target = dict[cardKey];
            }
            return target;
        }

        /// <summary>
        /// 尝试从所有的卡池中获取键值为key的卡牌
        /// </summary>
        /// <param name="cardKey">卡牌的键值,是其完整的类名(命名空间.类名)</param>
        /// <returns>卡牌预制体</returns>
        public Card GetCardPrefabFormPool(string cardKey)
        {
            foreach (CardPool pool in cardPools.ToList())
            {
                if (pool == null) continue;
                var dict = pool.GetCardPrefabDict();
                if ((bool)dict?.Contains(cardKey))
                {
                    return dict[cardKey];
                }
            }
            return null;
        }

        /// <summary>
        /// 使用随机种子随机获取指定卡池中的一张卡
        /// </summary>
        /// <param name="poolLabel">要在哪个卡池中搜索</param>
        /// <param name="filter">卡牌过滤器,能对输入的卡牌进行额外的操作</param>
        /// <returns>卡牌预制体</returns>
        public Card GetRandomCardFromPool(string poolLabel, Action<List<Card>> filter = null)
        {
            int seed = (int)SeedSetter.instance?.GetSeed_Int() + (int)BattleMessage.instance?.GetRound();
            System.Random rng = new System.Random(seed);

            CardPool targetPool = cardPools.Where(x => x.GetLabel().Equals(poolLabel)).FirstOrDefault();
            if (targetPool == null) return null;//所要寻找的卡池不存在,返回一张空卡
            //获取卡牌的顺序列表
            var dict = targetPool.GetCardPrefabDict();
            List<Card> stableCardList = dict
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value)
                .ToList();
            filter?.Invoke(stableCardList);//触发过滤器
            //返回卡牌
            Card target = null;
            int n = stableCardList.Count;
            int index = rng.Next(n);//随机数索引
            target = stableCardList[index];
            return target;
        }

        /// <summary>
        /// 使用随机种子随机地从所有卡池中
        /// </summary>
        /// <param name="filter">卡牌过滤器,能对输入的卡牌进行额外的操作</param>
        /// <returns></returns>
        public Card GetRandomCardFromPool(Action<List<Card>> filter = null)
        {
            // 1.初始化大卡池字典
            SerializableDictionary<string, Card> bigPool = new SerializableDictionary<string, Card>();

            // 2.遍历所有卡池,将内容合并到 bigPool 中(后出现的卡池会覆盖先出现的同名卡)
            foreach (CardPool pool in cardPools)
            {
                var currentDict = pool.GetCardPrefabDict();
                if (currentDict == null || currentDict.Count == 0) continue;

                foreach (var kvp in currentDict)
                {
                    // 如果 bigPool 中已存在该 Key,则覆盖,不存在则添加
                    bigPool[kvp.Key] = kvp.Value;
                }
            }
            // 3.边界检查: 如果合并后大卡池为空,直接返回 null
            if (bigPool.Count == 0) return null;
            // 4. 【关键】按 Key 排序提取 Value，生成顺序稳定的卡牌列表
            List<Card> stableCardList = bigPool
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value)
                .ToList();
            filter?.Invoke(stableCardList);//触发过滤器

            // 5. 生成随机种子并获取随机卡牌
            int seed = (int)SeedSetter.instance?.GetSeed_Int() + (int)BattleMessage.instance?.GetRound();
            System.Random rng = new System.Random(seed);
            int index = rng.Next(stableCardList.Count);
            return stableCardList[index];
        }

    }
}

