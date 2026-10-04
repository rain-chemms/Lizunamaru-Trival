using CardSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardLevelDisplayer : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image image;

    void OnEnable()
    {
        if(card == null) card = GetComponentInParent<Card>();
        if(image == null) image = GetComponent<Image>();
        if(levelText == null) levelText = GetComponentInChildren<TMP_Text>();
    }

    void Start()
    {
        SyncCardLevel();
    }

    void Update()
    {
        SyncCardLevel();
    }
    
    private int lastLevel = -1;
    private void SyncCardLevel()
    {
        if(card == null) return;
        uint nowLevel = card.GetCardUpgradeLevel();
        if(nowLevel != lastLevel)
        {
            //同步等级信息
            if(nowLevel > 0)
            {
                //打卡等级显示器
                levelText.enabled = true;
                image.enabled = true;
                if(card.CanRepeatUpgrade())
                {
                    levelText.text =  "+" + nowLevel.ToString();
                }
                else levelText.text =  "+";
            }
            else
            {
                //关闭等级显示器
                levelText.enabled = false;
                image.enabled = false;
                levelText.text = nowLevel.ToString();
            }
            lastLevel = (int)nowLevel;
        }
    }
}
