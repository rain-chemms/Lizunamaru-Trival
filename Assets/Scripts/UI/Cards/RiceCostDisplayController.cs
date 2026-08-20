using UnityEngine;
using CardSystem;
using UnityEngine.UI;
using TMPro;

public class RiceCostDisplayController : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Image riceCostImage;
    [SerializeField] private TMP_Text cardFunctioner;
    void OnEnable()
    {
        CheckCardAndDisplay();
    }

    private void CheckCardAndDisplay()
    {
        if(card == null || riceCostImage == null || cardFunctioner == null) return;
        if((bool)card.GetCardKeyWords()?.Contains(CardKeyWord.UNPLAYABLE))
        {
            riceCostImage.enabled = false;
            cardFunctioner.enabled = false;
        }
    }
}
