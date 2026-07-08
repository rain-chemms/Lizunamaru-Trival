using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RectTransform))]
public class SpellAttackDisplayer : MonoBehaviour
{
    public static SpellAttackDisplayer instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private TMP_Text spellText;
    public TMP_Text GetSpellText()
    {
        return spellText;
    }
    [SerializeField] private Image roleImage;
    public Image GetRoleImage()
    {
        return roleImage;
    }
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    [SerializeField] private Animator animator;
    public Animator GetAnimator()
    {
        return animator;
    }
    public List<AudioSource> GetAudioSources()
    {
        return audioSources;
    }
    void Start()
    {
        //自动获取全部的音频源
        List<AudioSource> AS = GetComponentsInChildren<AudioSource>().ToList();
        foreach (AudioSource aS in AS)
        {
            if(aS == null) continue;
            if(audioSources.Contains(aS)) continue;
            audioSources.Add(aS);
        }
        //尝试自动获取角色spell图片容器
        if(roleImage == null)
        {
            foreach(Image img in GetComponentsInChildren<Image>())
            {
                if(img == null) continue;
                if(img.name.Equals("RoleImage")) 
                {
                    roleImage = img;
                    break;
                }
            }
        }
        //尝试自动获取技能文本容器
        if(spellText == null)
        {
            foreach(TMP_Text txt in GetComponentsInChildren<TMP_Text>())
            {
                if(txt == null) continue;
                if(txt.name.Equals("SpellText"))
                {
                    spellText = txt;
                    break;
                }
            }
        }
        //尝试自动获取动画控制器
        if(animator == null) animator = GetComponent<Animator>();
    }
}
