using UnityEngine.UI;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpellAttackDisplayer))]
public class SpellDisplayMaskController : MonoBehaviour
{
    [SerializeField] private SpellAttackDisplayer displayer;
    [SerializeField] private float lerpSpeed = 0.1f;
    public float LerpSpeed { get => lerpSpeed; set => lerpSpeed = value; }
    [SerializeField] private Image mask;
    public Image Mask { get => mask; }
    [Range(0.0f,1.0f)] [SerializeField] private float inMaskAlpha = 0.5f;
    void OnEnable()
    {
        if(displayer == null) displayer = GetComponent<SpellAttackDisplayer>();
        if(mask == null)
        {
            mask = displayer?.GetComponentsInChildren<Image>().Where(x=>x.name.Equals("SpellMask")).FirstOrDefault();
        }
    }
    void Update()
    {
        CheckAndLerpMask();
    } 

    [SerializeField] private bool haveItem = false;
    private void CheckAndLerpMask()
    {
        if(mask != null && displayer != null)
        {
            haveItem = displayer.LeftList.Count > 0 && displayer.RightList.Count > 0;
            Color maskColor = mask.color;
            float targetAlpha = haveItem ? inMaskAlpha : 0.0f;
            Color targetColor = new Color(maskColor.r,maskColor.g,maskColor.b,targetAlpha);
            mask.color = Color.Lerp(maskColor,targetColor,LerpSpeed * Time.deltaTime);
        }
    }
}
