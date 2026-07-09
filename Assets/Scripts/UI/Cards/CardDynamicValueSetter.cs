using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class CardDynamicValueSetter : MonoBehaviour
{
    //动态键值映射器
    /*
        自动搜索脚本中的数值属性,获取其名称并为DynamicValueSetter中的对应索引index的映射的value赋值
    */
    [SerializeField] private Card card;
    [SerializeField] private SerializableDictionary<int,string> indexMapDict = new SerializableDictionary<int,string>();
    [SerializeField] private DynamicLocalizedText dynamicValueSetter;
    public SerializableDictionary<int,string> GetIndexMapDict()
    {
        return indexMapDict;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(card == null) card = GetComponent<Card>();
        CheckTheCardField();
        CheckTheCardProperty();
        SetDynamicValue();
        ApplyDynamicValue();
    }

    void OnEnable()
    {
        CheckTheCardField();
        CheckTheCardProperty();
        SetDynamicValue();  
        ApplyDynamicValue();  
    }

    void Update()
    {
        SetDynamicValue();
        if(needSet)
        {
            ApplyDynamicValue();
            needSet = false;
        }
    }

    [SerializeField] private List<PropertyInfo> properties = new List<PropertyInfo>();//属性列表
    [SerializeField] private List<FieldInfo> fields = new List<FieldInfo>();//字段列表
    [SerializeField] private Dictionary<int,float> dynamicValues = new Dictionary<int,float>();//动态值,发生变化时要对调用SetDynamicValue()
    private bool needSet = false;
    public void SetDynamicValue()
    {
        if(card == null) return;
        if(properties == null) return;
        if(indexMapDict == null) return;
        if(dynamicValueSetter == null) return;
        foreach(KeyValuePair<int,string> kv in indexMapDict)
        {
            //获取属性值
            string infoName = kv.Value;
            string value = "";
            bool searched = false;
            //优先寻找属性映射值
            if(!searched)
            foreach(PropertyInfo info in properties)
            {
                if(info.Name.Equals(infoName))
                {
                    value = info.GetValue(card).ToString();
                    searched = true;
                    break;
                }
            }
            if(!searched)
            foreach(FieldInfo info in fields)
            {
                if(info.Name.Equals(infoName))
                {
                    value = info.GetValue(card).ToString();
                    searched = true;
                    break;
                }
            }
            //检测字符串是否可以转化为float数字
            float num = 0;
            if(float.TryParse(value, out num))
            {
                //Debug.Log($"[CardDynamicValueSetter]: Get Property {infoName} value: {num}");
                //加入动态值
                if(!dynamicValues.ContainsKey(kv.Key)) 
                {
                    dynamicValues.Add(kv.Key, num);
                    needSet = true;
                }
                else
                {
                    if(dynamicValues[kv.Key] != num)
                    {
                        //修改键值,并触发修改
                        dynamicValues[kv.Key] = num;
                        needSet = true;
                    }
                }
            }
            else
            {
                //不是数字的属性,跳过
                //Debug.LogWarning($"[CardDynamicValueSetter]: The Property {infoName} is not a number");
                continue;
            }
        }
    }

    public void ApplyDynamicValue()
    {
        if(dynamicValueSetter == null) return;
        foreach(KeyValuePair<int,float> kv in dynamicValues.ToList())
        {
            int keyIndex = kv.Key;
            float num = kv.Value;
            foreach(var ve in dynamicValueSetter.GetVariables().ToList())
            {
                if(ve.index == keyIndex)
                {
                    ve.value = num;
                    //Debug.Log($"[CardDynamicValueSetter]: Set DynamicValueSetter index {"{"+keyIndex.ToString()+"}"} value: {num}");
                    break;
                }
            }
        }
    }
    //获取所有检测字段
    public void CheckTheCardField()
    {
        if(card == null) return;
        foreach(FieldInfo info in card.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if(info == null) continue;
            if(fields.Contains(info)) continue;
            fields.Add(info);
        }
        Debug.Log($"[CardDynamicValueSetter]: Get {fields.Count} fields");
    }
    //获取所有检测属性
    public void CheckTheCardProperty()
    {
        if(card == null) return;
        properties.Clear();
        foreach(PropertyInfo info in card.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if(info == null) continue;
            if(properties.Contains(info)) continue;
            properties.Add(info);
        }
        Debug.Log($"[CardDynamicValueSetter]: Get {properties.Count} properties");
    }
}
