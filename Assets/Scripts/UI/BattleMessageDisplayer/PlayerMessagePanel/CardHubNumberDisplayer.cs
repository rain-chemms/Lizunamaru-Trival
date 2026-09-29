using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class CardHubNumberDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    void OnEnable()
    {
        if(text ==null) text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        CheckAndSetCardHubNumber();
    }

    public void CheckAndSetCardHubNumber()
    {
        int count = (int)PlayerCardHub.instance?.GetCardHub_Copy()?.Count;   
        if(text != null)text.text = count.ToString();
    }
}   
