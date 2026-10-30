using UnityEngine;
using System.Collections;
using System;
using BonusSystem.BonusFuncs;
using System.Collections.Generic;
using CardSystem;
using RoleChoiceUISystem;
using CardSystem.CardPoolSystem;
using System.Linq;

namespace BonusSystem.BonusFuncs
{
    //卡牌奖励生效器
    public class CardBonus : Bonus
    {
        public static float upgradeRate = 0.3f;//产生升级后的卡牌奖励的概率
        public static uint maxUpgradeLevel = 3;//最大升级的等级
        //出现在筛选器内的稀有度不会生成
        [SerializeField] private List<CardRarity> rariteFilter;//卡牌稀有度筛选器
        public List<CardRarity> RariteFilter { get => rariteFilter; }
        [SerializeField] private  uint generateCount;//生成数量
        public uint GenerateCount { get => generateCount; set => generateCount = value; }
        [SerializeField] private uint choiceCount;//选择数量
        public uint ChoiceCount { get => choiceCount; set => choiceCount = value; }
        
        public override IEnumerator OnEffective()
        {
            effectiveSuccess = false;
            yield return UseFilterToGenerateBonus();
        }

        [SerializeField] private SerializableDictionary<Card,Card> EntityToPrefab = new SerializableDictionary<Card,Card>();//卡牌游戏实体到预制体信息的映射        
        public IEnumerator UseFilterToGenerateBonus()
        {
            //获取当前角色可以获取的卡池中的卡牌
            Debug.Log("[CardBonus]: UseFilterToGenerateBonus!");
            RoleChoiceData roleData = BattleMessage.instance?.GetRoleChoiceData();
            List<string> cardPoolLabel = roleData?.UseCardPool;
            //使用随机种子随机选取卡牌
            List<CardPool> pools = CardPoolManager.instance?.GetCardPools_Copy()?.Where(x => cardPoolLabel.Contains(x.GetLabel()))?.ToList();
            List<Card> cards = new List<Card>();
            foreach(CardPool pool in pools.ToList())
            {
                if(pool == null) continue;
                //进行稀有度筛选
                List<Card> filteredCards = pool.GetAllCardPrefabInPool().Where(x => !RariteFilter.Contains(x.GetRarity())).ToList();
                cards.AddRange(filteredCards);
            }

            int seed = SeedSetter.instance.GetSeed_Int();
            System.Random random = new System.Random(seed);//初始化随机数种子
            List<int> haveSelectIndex = new List<int>();//已经选择的索引
            int loop = 0;
            while(loop < (int)generateCount)
            {
                int index = random.Next(0,cards.Count);
                if(haveSelectIndex.Count >= cards.Count) break;//强制终止条件
                if(haveSelectIndex.Contains(index)) continue;
                haveSelectIndex.Add(index);
                loop++;
            }

            System.Random levelRandom = new System.Random(seed);//新建一个随机取数器
            //建立预制体到实体的映射
            foreach(int index in haveSelectIndex.ToList())
            {
                Card cardEntity = Instantiate(cards[index],null);
                //通过随机数判断是否需要升级
                float rt = (float)levelRandom.NextDouble();//[0,1);
                if(rt > upgradeRate)
                {
                    //随机设置升级等级并设置
                    uint level = (uint)levelRandom.Next(1,(int)maxUpgradeLevel);
                    cardEntity.SetCardUpgradeLevel(level);
                }
                EntityToPrefab.Add(cardEntity,cards[index]);
            }
            //调起CardSelectOperator
            //1.创建cpList
            List<(Card,Transform)> cpList = new List<(Card,Transform)>();
            foreach(Card card in EntityToPrefab.Keys.ToList())
            {
                cpList.Add((card,null));
            }
            CardSelectOperator.instance.SetOperateFunc(AddCardBonusToPlayerCardHub);
            CardSelectOperator.instance.SetCancelFunc(CancelFunc);
            //调起CardSelectOperator
            yield return CardSelectOperator.instance.CallTheCardSelectOperator(choiceCount,CardOperateCategory.EQUAL,null,cpList,true);
        }

        private IEnumerator AddCardBonusToPlayerCardHub(Card card)
        {
            Debug.Log("[CardBonus]: AddCardBonusToPlayerCardHub!");
            //获取选择的卡牌的对应预制体
            if(EntityToPrefab.ContainsKey(card))
            {
                Card prefab = EntityToPrefab[card];
                PlayerCardHub.PlayerCardHubItem newItem = new PlayerCardHub.PlayerCardHubItem();
                newItem.cardPrefab = prefab;
                newItem.cardLevel = card.GetCardUpgradeLevel();
                newItem.canRUG = card.CanRepeatUpgrade();
                //将升级信息存入玩家卡牌库
                PlayerCardHub.instance.GetCardHub().Add(newItem);
                effectiveSuccess = true;
            }

            //销毁所有卡牌实体
            foreach(Card c in EntityToPrefab?.Keys.ToList())
            {
                if(c == null) continue;
                Destroy(c.gameObject);
            }
            EntityToPrefab?.Clear();
            yield return null;
        }

        private IEnumerator CancelFunc()
        {
            //销毁所有卡牌游戏实体
            foreach(Card card in EntityToPrefab?.Keys.ToList())
            {
                if(card == null) continue;
                Destroy(card.gameObject);
            }
            EntityToPrefab?.Clear();
            effectiveSuccess = false;
            yield return null;
            Debug.Log("[CardBonus]: CancelFunc!");
        }


    }
}