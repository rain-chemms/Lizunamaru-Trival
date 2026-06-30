using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//检测的卡牌过滤器
[RequireComponent(typeof(UIBoarderDisplayer))]
public class BoarderCheckCardFliter : MonoBehaviour
{
    //当卡牌进行检测并显示物体边界时.只有下面列表的卡牌类别才能被检测
    [SerializeField] private List<CardCategory> cardCategories = new List<CardCategory>();
    void Start()
    {
        UniqueTheCategory();
    }

    //类别去重
    private void UniqueTheCategory()
    {
        List<CardCategory> tempList = new List<CardCategory>();
        foreach(CardCategory cardCategory in cardCategories)
        {
            if(!tempList.Contains(cardCategory))
            {
                tempList.Add(cardCategory);
            }
        }
        cardCategories = tempList;    
    }

    
    public bool HaveCategory(CardCategory cardCategory)
    {
        if(cardCategories == null) return false;
        return cardCategories.Contains(cardCategory);
    }

    public void AddCategory(CardCategory cardCategory)
    {
        if(cardCategories == null) cardCategories = new List<CardCategory>();
        if(!cardCategories.Contains(cardCategory))
        {
            cardCategories.Add(cardCategory);
        }
    }

    public void RemoveCategory(CardCategory cardCategory)
    {
        if(cardCategories == null) return;
        if(cardCategories.Contains(cardCategory))
        {
            cardCategories.Remove(cardCategory);
        }
    }
}
