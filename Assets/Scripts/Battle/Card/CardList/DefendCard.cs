using System.Collections;
using UnityEngine;

public class DefendCard : Card
{
    void Start()
    {
        this.SetCardCategory(CardCategory.EFFECTIVE);
    }

    public virtual IEnumerator AfterTriggerEffective()
    {
        yield return base.AfterTriggerEffective();
        yield return 2.0f;
    }
}
