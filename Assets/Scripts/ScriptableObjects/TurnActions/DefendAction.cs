using UnityEngine;
using System.Collections;

namespace GridObjectSystem.RoleSystem.AutoSystem
{
    [CreateAssetMenu(fileName = "InTurnAutoAction", menuName = "InTurnAutoAction/Actions/DefendAction")]
    public class DefendAction : TurnAction
    {
        [SerializeField] protected int defendPoint = 0;//获得或失去的防御点数
        public override IEnumerator Excute(Role role)
        {
            yield return base.Excute(role);
            yield return role.GetComponent<RoleDefendGetter>()?.GetOrLoseDefend(defendPoint);
        }
    }
}