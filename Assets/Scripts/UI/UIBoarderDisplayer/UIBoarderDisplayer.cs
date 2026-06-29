using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Animator))]
public class UIBoarderDisplayer : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private bool hideBoarder = false;
    public void SetHideBoarder(bool hide)
    {
        hideBoarder = hide;
    }
    public bool IsHidden()
    {
        return hideBoarder;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetBool("IsHidden") != hideBoarder) animator.SetBool("IsHidden", hideBoarder);
    }
}
