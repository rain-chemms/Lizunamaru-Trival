using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using CardSystem;
using System.Linq;

//该脚本用于触发卡牌加入手牌选择器的效果
//默认情况下它是关闭的

[RequireComponent(typeof(Card))]
public class CardHandCardOperatorController : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private Card card;
    void OnEnable()
    {
        if(card == null) card = GetComponent<Card>();
        InitSourceAlphaDict();
        if(IsFiltedByHandCardOperator()) SetFilterAlpha();
    }

    void OnDisable()
    {
        RevertAlpha();
    }

    //点击时触发的事件
    public void OnPointerClick(PointerEventData eventData)
    {
        //将当前卡牌在HandOperator和手牌列表中进行切换
        HandCardOperator instance = HandCardOperator.instance;
        if(instance == null) return;
        if(IsFiltedByHandCardOperator()) return;//若当前卡牌被过滤掉则返回
        //当前卡牌已经被选中在HandOperator中了
        if(instance.IsCardSelected(card)) HandCardOperator.instance.RemoveCard(card);//将其从HandOperator中移除,并加入手牌
        else
        {
            //若当前超出了选中数量则返回
            int num = (int)instance?.GetSelectedCards()?.Count;
            if(num >= (int)instance.GetOperateCount()) return;
            HandCardOperator.instance.AddCard(card);//否则加入HandOperator中
        }
    }

    //检测卡牌的种类是否被过滤器过滤掉了,过滤掉了不能选择且设置其Image的透明度为0.5f
    [SerializeField] private float filterAlphaFactor = 0.5f;//乘积因子
    private Dictionary<Image,float> sourceAlphaDict = new Dictionary<Image,float>();
    
    //初始化sourceAlphaDict
    private void InitSourceAlphaDict()
    {
        sourceAlphaDict.Clear();
        //获取卡牌所有Image组件
        List<Image> cardImgs = card?.GetComponentsInChildren<Image>().ToList();
        foreach(Image img in cardImgs)
        {
            if(img == null) continue;
            sourceAlphaDict.Add(img,img.color.a);
        }
    }

    //设置过滤时的透明度
    private void SetFilterAlpha()
    {
        foreach(KeyValuePair<Image,float> kvp in sourceAlphaDict)
        {
            if(kvp.Key != null) kvp.Key.color = new Color(kvp.Key.color.r,kvp.Key.color.g,kvp.Key.color.b,kvp.Value*filterAlphaFactor);
        }
    }

    //恢复Image的透明度
    private void RevertAlpha()
    {
        foreach(KeyValuePair<Image,float> kvp in sourceAlphaDict)
        {
            if(kvp.Key != null) kvp.Key.color = new Color(kvp.Key.color.r,kvp.Key.color.g,kvp.Key.color.b,kvp.Value);
        }
    }

    private bool IsFiltedByHandCardOperator()
    {
        HandCardOperator instance = HandCardOperator.instance;
        if(instance == null) return false;
        if((bool)instance?.GetCardFilter_Copy()?.Contains((CardCategory)card?.GetCardCategory())) return true;
        return false;
    }
}