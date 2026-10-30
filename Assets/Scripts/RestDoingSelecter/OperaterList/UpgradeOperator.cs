using System.Collections;
using System.Collections.Generic;
using CardSystem;
using GameSaveSystem;
using UnityEngine;

namespace RestDoingSystem
{
    public class UpgradeOperator : RestOperater
    {
        //一次升级几张牌
        public static uint upgradeCount = 1;
        [SerializeField] private SerializableDictionary<Card,PlayerCardHub.PlayerCardHubItem> dict = new SerializableDictionary<Card, PlayerCardHub.PlayerCardHubItem>();
        private bool isCanceled = false;
        protected override void OperateEffect()
        {
            isCanceled = false;
            dict.Clear();
            //创建Dict和cpList
            List<(Card,Transform)> cpList = new List<(Card,Transform)>();
            foreach(var pchi in PlayerCardHub.instance.GetCardHub_Copy())
            {
                if(pchi == null || pchi.cardPrefab == null) continue;
                if(pchi.cardLevel > 0 && !pchi.canRUG) continue;
                Card cEntity = Instantiate(pchi.cardPrefab,null);
                cpList.Add((cEntity,null));
                dict.Add(cEntity,pchi);
            }
            //连接选择方法
            CardSelectOperator.instance.SetOperateFunc(UpgradeCardInHub);
            CardSelectOperator.instance.SetCancelFunc(CancelUpgrade);
            //调起CardSelectOperator
            StartCoroutine(CallCardSelectOperatorAndDestroyAllCardEntity(cpList));
        }

        //删除所有
        private IEnumerator CallCardSelectOperatorAndDestroyAllCardEntity(List<(Card,Transform)> cpList)
        {
            yield return CardSelectOperator.instance.CallTheCardSelectOperator(upgradeCount,CardOperateCategory.EQUAL,null,cpList,true);
            foreach((Card card,Transform tf) in cpList)
            {
                Destroy(card.gameObject);
            }
            if(!isCanceled) base.OperateEffect();//没取消操作则执行完毕,解锁地图和设置状态为执行完毕
        }

        private IEnumerator CancelUpgrade()
        {
            isCanceled = true;
            yield break;
        }

        private IEnumerator UpgradeCardInHub(Card card)
        {
            PlayerCardHub.PlayerCardHubItem item = null;
            if(!(bool)dict?.ContainsKey(card)) yield break;
            item = dict[card];
            //将对应卡牌数据的值等级提高1
            item.cardLevel++;
            //将其存入SaveSystem中
            GamePlayData data = SaveSystem.instance.GamePlayDatas[SaveSystem.instance.NowSaveIndex];
            data.cardHub = PlayerCardHub.instance.GetFullNameListOfCardHub();
            //但是不保存到文件中,加载下一关时才保存
        }
    }


}