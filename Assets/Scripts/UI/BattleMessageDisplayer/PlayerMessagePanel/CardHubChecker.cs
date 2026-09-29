using System.Collections.Generic;
using CardSystem;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardHubChecker : MonoBehaviour
{
    [SerializeField] private Button button;

    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        button.onClick.AddListener(CallStackCardDisplayer);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(CallStackCardDisplayer);
    }

    private void CallStackCardDisplayer()
    {
        //目标卡牌时玩家的牌组
        List<Card> cardList = PlayerCardHub.instance?.GetCardHub_Copy();
        //设置要显示的卡牌列表
        StackCardDisplayer.instance?.ClearCardList();
        StackCardDisplayer.instance?.SetCardList(cardList);
        //显示卡牌
        StackCardDisplayer.instance?.SetDisplay(true);
        StackCardDisplayer.instance?.OpenDisplayer();
    }
}
