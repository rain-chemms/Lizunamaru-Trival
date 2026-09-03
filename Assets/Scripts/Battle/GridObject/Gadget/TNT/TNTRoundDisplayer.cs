using UnityEngine;
using TMPro;

namespace GridObjectSystem.GadgetSystem.Tnts
{

    [RequireComponent(typeof(TMP_Text))]
    public class TNTRoundDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text roundText;
        void OnEnable()
        {
            if (roundText == null) roundText = GetComponent<TMP_Text>();
        }
        
        [SerializeField] private TNT tnt;
        void Update()
        {
            SetTheTntDisplayText();
        }

        private void SetTheTntDisplayText()
        {
            if(roundText != null) roundText.text = ((int)tnt?.GetClockNumber() - (int)tnt?.GetRoundRecorder()).ToString();
        }
    }
}

