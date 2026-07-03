using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StackCardCheckButton : MonoBehaviour
{
    [SerializeField] private bool drawOrDiscard;
    public void SetDrawOrDiscard(bool isDraw)
    {
        this.drawOrDiscard = isDraw;
    }

    public void OpenTheCardStackDisplayer()
    {
        List<Card> cards = null;
        if(drawOrDiscard)//检测抽牌堆
        {
            cards = BattleMessage.instance?.GetDrawCardList();
        }
        else//检测弃牌堆
        {
            cards = BattleMessage.instance?.GetDiscardCardList();
        }
        if(cards == null)
        {
            Debug.LogError("The CheckCardStack :" + (drawOrDiscard? "Draw" : "Discard") + " is null, Please Check!");
            return;
        }
        //设置要显示的卡牌列表
        StackCardDisplayer.instance?.SetCardList(cards);
        //显示卡牌
        StackCardDisplayer.instance?.SetDisplay(true);   
        StackCardDisplayer.instance?.OpenDisplayer();
    }
}
