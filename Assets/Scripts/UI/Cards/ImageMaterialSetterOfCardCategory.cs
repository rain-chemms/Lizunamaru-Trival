using UnityEngine;
using UnityEngine.UI;
using CardSystem;

[RequireComponent(typeof(Image))]
public class ImageMaterialSetterOfCardCategory : MonoBehaviour
{
    [SerializeField] private Card card;
    public Card GetCard() => card;
    
    [SerializeField] private Image image;
    void OnEnable()
    {
        if(card == null) card = GetComponentInParent<Card>();//尝试从父节点中获取
        if(image == null) image = GetComponent<Image>();
    }

    void Start()
    {
        SetTheMaterialToImage();
    }

    //依据卡牌的类型自动设置Image材质
    [SerializeField] private Material defaultMat;
    public Material GetDefaultMat() => defaultMat;
    
    [SerializeField] private SerializableDictionary<CardCategory, Material> categoryMatDict; 
    public SerializableDictionary<CardCategory, Material> GetCategoryMatDict() => categoryMatDict;

    private void SetTheMaterialToImage()
    {
        if(image == null) return;
        if(card != null)
        {
            CardCategory category = card.GetCardCategory();
            if(categoryMatDict.ContainsKey(category))
            {
                image.material = categoryMatDict[category];
            }
            else image.material = defaultMat;
        }
    }
}
