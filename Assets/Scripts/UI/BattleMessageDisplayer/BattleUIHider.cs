using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BattleMessageDisplayer))]
[RequireComponent(typeof(Animator))]
public class BattleUIHider : MonoBehaviour
{
    [SerializeField] private float displayAlpha = 1f;// 显示时的透明度
    [SerializeField] private float hideAlpha = 0.5f;// 隐藏时的透明度
    [SerializeField] private bool isHidden = true;
    public bool IsHidden()
    {
        return isHidden;
    }
    public void SetIsHidden(bool hidden)
    {
        isHidden = hidden;
    }

    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 设置动画器
        if(animator.GetBool("IsHidden") != isHidden) animator.SetBool("IsHidden", isHidden);
        GetAllChildImage();
        CheckAndSetAllChildImageAlpha();
    }

    List<Image> images = new List<Image>();
    private void GetAllChildImage()
    {
        images.Clear();// 清空
        // 获取所有子节点
        // 获取所有Image
        images = this.GetComponentsInChildren<Image>().ToList();
    }

    private void CheckAndSetAllChildImageAlpha()
    {
        if(isHidden)
        {
            foreach(Image image in images)
            {
                if(image == null) continue;
                image.color = new Color(image.color.r, image.color.g, image.color.b, hideAlpha);
            }
        }
        else
        {
            foreach(Image image in images)
            {
                if(image == null) continue;
                image.color = new Color(image.color.r, image.color.g, image.color.b, displayAlpha);
            }
        }
    }
}
