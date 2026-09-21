using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CardVfxSystem
{
    [RequireComponent(typeof(Image))]
    public class CardVfx : MonoBehaviour
    {
        [SerializeField] private Image image;
        public Image GetImage() => image;
        void OnEnable()
        {
            if(image == null) image = GetComponent<Image>();
        }

        private Material tempMat = null;
        public void SetMaterial(Material mat)
        {
            if(tempMat != null) Destroy(tempMat);// 销毁上一次的临时材质
            tempMat = Instantiate(mat);
            image.material = mat;
        }

        void OnDestroy()
        {
            if(tempMat != null) Destroy(tempMat);
        }
    }
}