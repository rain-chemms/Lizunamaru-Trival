using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(Button))]
public class KeyMappingResetButton : MonoBehaviour
{
    [SerializeField] private Button button;

    [SerializeField] private List<KeyMappingItemPanel> itemList = new List<KeyMappingItemPanel>();
    public List<KeyMappingItemPanel> GetItemList() => itemList;
    public List<KeyMappingItemPanel> GetItemList_Copy() => new List<KeyMappingItemPanel>(itemList);

    void OnEnable()
    {
        //尝试获取所有的ItemPanel组件
        itemList.Clear();
        itemList = transform.parent.GetComponentsInChildren<KeyMappingItemPanel>(true).ToList();
        if(button == null) button = GetComponent<Button>();
        button.onClick.AddListener(ResetAllKeyMapping);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(ResetAllKeyMapping);
    }

    private void ResetAllKeyMapping()
    {
        foreach(KeyMappingItemPanel item in itemList.ToList())
        {
            if(item == null) continue;
            item?.ResetBinding();
        }
    }
}
