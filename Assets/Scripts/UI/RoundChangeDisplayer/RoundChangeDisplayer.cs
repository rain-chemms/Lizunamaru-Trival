using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RectTransform))]
public class RoundChangeDisplayer : MonoBehaviour
{
    public static RoundChangeDisplayer instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private Animator animator;
    public Animator GetAnimator()
    {
        return animator;
    }
    [SerializeField] private Canvas canvas;
    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
        if(canvas == null) canvas = GetComponent<Canvas>();
        if(canvas != null) canvas.worldCamera = Camera.main;
        if(displayText == null)
        {
            displayText = GetComponentInChildren<TMP_Text>();
        }
    }
    [SerializeField] private TMP_Text displayText;
    public TMP_Text GetDisplayText()
    {
        return displayText;
    }
    public void SetDisplayText(string text)
    {
        if(displayText != null) displayText.text = text;
    }
}
