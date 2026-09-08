using UnityEngine;
using System.Collections;
using GridObjectSystem.RoleSystem;


namespace CardSystem.AllCardHub
{ 
    public class Effective_SpellCard : Card
    {
        [SerializeField] private float recoverSpellPrecent = 1.0f;//恢复的符卡点数
        public float GetRecoverSpellPrecent() => recoverSpellPrecent;
        public void SetRecoverSpellPrecent(float point) => recoverSpellPrecent = point;

        public override IEnumerator AfterPlay()
        {
            Role player = BattleMessage.instance?.GetControlPlayer();
            if(player != null)
            {
                float newSpell = player.GetSpellPrecent() + recoverSpellPrecent;
                if(newSpell < 0) newSpell = 0;
                player.SetSpellPrecent(newSpell);//恢复符卡点数
            }
            yield return base.AfterPlay();
        }
    }
}