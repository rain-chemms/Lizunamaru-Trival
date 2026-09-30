using UnityEngine;
using System.Collections;
using GridObjectSystem.RoleSystem;


namespace CardSystem.AllCardHub
{
    //防御姿态: 相关角色:红美玲
    //打出后: 获得{0}点防御,将{1}张伤口加入手牌中
    public class Effective_DefendPosture : Card
    {
        [SerializeField] private uint addWoundNumber;
        public float GetAddWoundNumber() => addWoundNumber;
        public void SetAddWoundNumber(uint num) => addWoundNumber = num;

        [SerializeField] private int defendPoint;
        public int GetDefendPoint() => defendPoint;
        public void SetDefendPoint(int def) => defendPoint = def;

        [SerializeField] private Card status_wound_prefab;
        public Card GetWoundCardPrefab() => status_wound_prefab;

        public override IEnumerator AfterPlay()
        {
            //角色获得防御
            Role player = BattleMessage.instance?.GetControlPlayer();
            if(player != null)
            {
                RoleDefendGetter rdg = player.GetComponent<RoleDefendGetter>();
                int defPot = defendPoint < 0 ? 0 :defendPoint;
                if(rdg != null)
                {
                    yield return rdg?.GetOrLoseDefend(defPot);
                }
                else
                {
                    player.SetDefend(player.GetDefend() + (uint)defendPoint);
                }
            }
            //添加状态牌到手中
            for(int i = 0;i < addWoundNumber;i ++)
            {
                Card cardEntity = Instantiate(status_wound_prefab,BattleMessageDisplayer.instance?.transform);
                yield return BattleMessage.instance?.AddExistCardToHand(cardEntity);
            }
            yield return base.AfterPlay();
        }
    }
}
