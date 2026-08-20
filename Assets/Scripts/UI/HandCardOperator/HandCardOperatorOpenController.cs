using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CardSystem;
using System.Collections.Generic;
using System.Linq;
    
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HandCardOperator))]
public class HandCardOperatorOpenController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private HandCardOperator handCardOperator;
    
    void OnEnable()
    {
        if(animator == null) animator = GetComponent<Animator>();
        if(handCardOperator == null) handCardOperator = GetComponent<HandCardOperator>(); 
    }

    [SerializeField] public bool isOpen = false;
    public bool IsOpen() => isOpen;
    public void SetOpen(bool isOpen) => this.isOpen = isOpen;
    
    void Update()
    {
        CheckOpenState();
    }

    public void CheckOpenState()
    {
        //设置动画器参数
        animator.SetBool("IsOpen",isOpen);    
    }
}    
    