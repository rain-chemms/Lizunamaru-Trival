using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StackCardCheckButton : MonoBehaviour
{
    [SerializeField] private string cardStackName;
    public string GetCardStackName() => cardStackName;
    public void SetCardStackName(string name) => cardStackName = name;
    [SerializeField] private List<Card> cardList;
    void Start()
    {
        cardList = BattleMessage.instance?.GetCardListByName(cardStackName);
    }
    
    void OnEnable()
    {
        cardList = BattleMessage.instance?.GetCardListByName(cardStackName);
    }

    public void OpenTheCardStackDisplayer()
    {
        if(cardList == null)
        {
            Debug.LogError("The CheckCardStack :" + cardStackName + " is null OR get error, Please Check!");
            return;
        }
        //设置要显示的卡牌列表
        StackCardDisplayer.instance?.SetCardList(cardList);
        //显示卡牌
        StackCardDisplayer.instance?.SetDisplay(true);   
        StackCardDisplayer.instance?.OpenDisplayer();
    }
}
