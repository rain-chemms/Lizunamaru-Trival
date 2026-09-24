using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;

namespace GlobalSystem
{
    [RequireComponent(typeof(Image))]
    public class ImageHoverMaterialSetter : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        void OnEnable()
        {
            if(image == null) image = GetComponent<Image>();
            sourceMaterial = image.material;
        }
        [SerializeField] private Image image;
        [NonSerialized] private Material sourceMaterial;
        public Image Image
        {
            get
            {
                if (image == null)
                {
                    image = GetComponent<Image>();
                    sourceMaterial = image.material;
                }
                return image;
            }
            set
            {
                image = value;
            } 
        }

        [SerializeField] private Material hoverMaterial = null;
        public Material HoverMaterial
        {
            get { return hoverMaterial; }
            set { hoverMaterial = value; }
        }

        // 进入时切换为悬停光标
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(image != null) image.material = hoverMaterial;
        }

        // 离开时恢复默认
        public void OnPointerExit(PointerEventData eventData)
        {
            image.material = sourceMaterial;
        }

        void OnDisable()
        {
            image.material = sourceMaterial;
            sourceMaterial = null;
        }
    }
}
