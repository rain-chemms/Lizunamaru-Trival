using System.Collections;

public interface GadgetFunctioner
{
    IEnumerator OnGadgetEffect();
    IEnumerator OnEveryRoundStart();
    IEnumerator OnEveryRoundEnd();
    IEnumerator AfterACardPlayed();//在一张牌被打出后的效果
}