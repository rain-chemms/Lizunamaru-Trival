using System.Collections;

public interface GadgetFunctioner
{
    IEnumerator OnGadgetEffect();
    IEnumerator OnEveryRoundStart();
    IEnumerator OnEveryRoundEnd();
}