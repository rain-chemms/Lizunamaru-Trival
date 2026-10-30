using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Events;
using TMPro;
using System.Collections;

namespace BonusSystem.BonusViewSystem
{
    //奖励外观设置器
    [RequireComponent(typeof(BonusItem))]
    public class BonusItemViewer : MonoBehaviour
    {
        [Header("奖励数量设置")]
        [SerializeField] private TMP_Text countText;
        public TMP_Text CountText => countText;
        [SerializeField] private TMP_Text descriptionText;
        public TMP_Text DescriptionText => descriptionText;
        [SerializeField] private Image previewImage;
        public Image PreviewImage => previewImage;
        //刷新物体外观
        public void FreshView(Sprite previewSprite, string description, int number = 1)
        {
            if(countText != null) countText.text = "*" + number.ToString();
            if(descriptionText != null) descriptionText.text = description;
            if(previewImage != null) previewImage.sprite = previewSprite;
        }
    }

}