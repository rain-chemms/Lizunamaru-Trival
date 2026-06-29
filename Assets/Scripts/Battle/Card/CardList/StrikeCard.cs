using System.Collections;
using UnityEngine;

public class StrikeCard : Card
{
    void Start()
    {
        this.SetCardCategory(CardCategory.ATTACK);
    }

    public virtual IEnumerator AfterTriggerEffective()
    {
        yield return base.AfterTriggerEffective();
        yield return 2.0f;
    }
}
