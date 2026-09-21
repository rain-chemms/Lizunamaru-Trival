using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace CardVfxSystem
{
    [RequireComponent(typeof(Canvas))]
    public class CardVfxDisplayer : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        public Canvas GetCanvas() => canvas;
        void OnEnable()
        {
            if (canvas == null) canvas = GetComponent<Canvas>();
            //尝试自动获取CardVfx
            if(vfxList == null) vfxList = new List<CardVfx>();
            foreach (CardVfx vfx in GetComponentsInChildren<CardVfx>(true).ToList())
            {
                if(vfx == null) continue;
                if((bool)vfxList?.Contains(vfx)) continue;
                vfxList?.Add(vfx);
            }
        }

        [SerializeField] private bool isOpen = true;
        public bool IsOpen() => isOpen;
        public void SetOpen(bool open) => isOpen = open;

        void Update()
        {
            //同步开启状态
            if (canvas != null)
            {
                canvas.enabled = isOpen;
            }
        }

        [SerializeField] private List<CardVfx> vfxList = new List<CardVfx>();
        public List<CardVfx> GetVfxList() => vfxList;
        public List<CardVfx> GetVfxList_Copy() => new List<CardVfx>(vfxList);

        public CardVfx GetVfx(string vfxName)
        {
            foreach (var vfx in vfxList)
            {
                if (vfx.name.Equals(vfxName))
                {
                    return vfx;
                }
            }
            return null;
        }

        public void CloseAllVfx()
        {
            foreach (var vfx in vfxList)
            {
                if(vfx != null) vfx.gameObject.SetActive(false);
            }
        }

        public void CloseVfx(string vfxName)
        {
            foreach (CardVfx vfx in vfxList.ToList())
            {
                if(vfx == null) continue;
                if (vfx.name.Equals(vfxName))
                {
                    if(vfx != null) vfx.gameObject.SetActive(false);
                }
            }
        }

        public void OpenVfx(string vfxName)
        {
            foreach (CardVfx vfx in vfxList.ToList())
            {
                if(vfx == null) continue;
                if (vfx.name.Equals(vfxName))
                {    
                    if(vfx != null) vfx.gameObject.SetActive(true);
                }
            }
        }
    }
}

