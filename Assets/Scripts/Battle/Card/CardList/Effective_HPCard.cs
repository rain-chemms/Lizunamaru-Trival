using UnityEngine;
using System.Collections;
using GridObjectSystem.RoleSystem;


namespace CardSystem.AllCardHub
{ 
    public class Effective_HPCard : Card
    {
        [SerializeField] private float recoverHpPoint = 12;//恢复生命值
        public float GetRecoverHpPoint() => recoverHpPoint;
        public void SetRecoverHpPoint(float point) => recoverHpPoint = point;

        public override IEnumerator AfterPlay()
        {
            Role player = BattleMessage.instance?.GetControlPlayer();
            RoleHealther hler = player.GetComponent<RoleHealther>();
            if(hler != null)
            {
                hler.GetHealth(recoverHpPoint);
            }
            else
            {
                if(recoverHpPoint >= 0)
                {
                    float maxHp = player.GetMaxHp();
                    float newHp = player.GetHp() + recoverHpPoint;
                    if(newHp > maxHp) newHp = maxHp;
                    player.SetHp(newHp);
                }
            }
            yield return base.AfterPlay();
        }
    }
}