using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Card))]
public class CardRiceCostDisplayer : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private TMP_Text displayText;//费用显示文本
    public void SetDisplayText(TMP_Text dt)
    {
        displayText = dt;
    }
    public TMP_Text GetDisplayText()
    {
        return displayText;
    }
    [SerializeField] private Image displayImage;//这个用来设置图标背景
    public void SetDisplayImage(Image img)
    {
        displayImage = img;
    }
    public Image GetDisplayImage()
    {
        return displayImage;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //尝试自动获取
        if(card == null) card = GetComponent<Card>();
        if(displayImage == null)
        {
            foreach(Image img in GetComponentsInChildren<Image>().ToList())
            {
                if(img == null) continue;
                if(img.name.Equals("RiceCost"))
                {
                    displayImage = img;
                    break;
                }
            }
        }
        if(displayText == null)
        {
            foreach(TMP_Text txt in GetComponentsInChildren<TMP_Text>().ToList())
            {
                if(txt == null) continue;
                if(txt.name.Equals("RiceCostPoint"))
                {
                    displayText = txt;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndDisplayCardCost();
    }

    private void CheckAndDisplayCardCost()
    {
        if(card == null || displayText == null) return;
        displayText.text = card.GetRiceCost().ToString();
    }
}
