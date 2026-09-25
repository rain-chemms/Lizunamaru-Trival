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
            if(instance == null)
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
            foreach(CardPool pool in pools)
            {
                if(pool == null) continue;
                if((bool)cardPools?.Contains(pool)) continue;
                cardPools?.Add(pool);
            }
        }
    }
}

