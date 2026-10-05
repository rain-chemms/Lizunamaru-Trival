using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GridObjectSystem.RoleSystem;
using System.Linq;
using JetBrains.Annotations;
using System;

namespace CardSystem.AllCardHub
{
    //大叫:相关角色:幽谷响子
    //对自身{0}范围内的所有敌人产生你所有牌数量的伤害
    //这张卡在任意牌堆时,你的每回合结束都会添加一张自身的复制品到相应的牌堆中
    //升级后,范围+1
    //具有消耗
    public class Effective_ScreamEcho : Card
    {
        [SerializeField] private uint range = 1;
        public uint Range {get => range;}
        public override IEnumerator AfterPlay()
        {
            //获取前方3*3格子
            List<Role> enermys = new List<Role>();
            Role player = BattleMessage.instance?.GetControlPlayer();
            Vector2Int center = (Vector2Int)player?.GetGridIndex();//中心位置
            int xStart = center.x - (int)range;
            int yStart = center.y - (int)range;
            int length = 2 * (int)range + 1;
            foreach(Role role in BattleMessage.instance.GetRoleList_Copy())
            {
                if(role == null) continue;
                Vector2Int index = (Vector2Int)role?.GetGridIndex();
                if(
                    index.x >= xStart && 
                    index.x < xStart + length && 
                    index.y >= yStart && 
                    index.y < yStart + length
                )//在锁定范围内
                {
                    if(player != null && role.GetSide() != player.GetSide()) enermys.Add(role);   
                }
            }
            int cardNumber = (int)BattleMessage.instance?.GetNowBattleCardNumber();
            //对她们造成伤害
            foreach(Role role in enermys.ToList())
            {
                if(cardNumber >= 0) role.GetComponent<RoleDamageGetter>().GetDamage(cardNumber,true);
            }
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterRoundEnd()
        {
            Role player = BattleMessage.instance?.GetControlPlayer();
            if(BattleMessage.instance?.IsPlayerTurn() == player?.GetSide())
            {
                //检测其在哪个牌堆中
                //将一张复制品加入牌堆
                List<Card> targetList = null;
                List<Card> handCard = BattleMessage.instance?.GetHandCardList();
                List<Card> discardCard = BattleMessage.instance?.GetDiscardCardList();
                List<Card> exhaustCard = BattleMessage.instance?.GetExhaustCardList();
                List<Card> drawCard = BattleMessage.instance?.GetDrawCardList();
                
                if((bool)handCard?.Contains(this)) targetList = handCard;
                else if((bool)discardCard?.Contains(this)) targetList = discardCard;
                else if((bool)exhaustCard?.Contains(this)) targetList = exhaustCard;
                else if((bool)drawCard?.Contains(this)) targetList = drawCard;

                if(targetList != null)
                {
                    Card newCard = Instantiate(this,BattleMessageDisplayer.instance.transform);
                    if(targetList == handCard) yield return BattleMessage.instance.AddExistCardToHand(newCard);
                    else targetList.Add(newCard);
                } 
            }
            yield return base.AfterRoundEnd();
        }

        [NonSerialized] private uint sourceRange = 0;
        new void OnEnable()
        {
            sourceRange = range;
            base.OnEnable();
        }

        public override IEnumerator UpgradeEffective()
        {
            range = sourceRange + cardUpgradeLevel;
            yield return base.UpgradeEffective();
        }

    }
}