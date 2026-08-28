using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;

[RequireComponent(typeof(Slider))]
public class VoiceSliderSetter : MonoBehaviour
{
    [SerializeField] private Slider slider;
    void OnEnable()
    {
        if (slider == null) slider = GetComponent<Slider>();    
    }

    void Start()
    {
        InitialSliderValue();
    }

    private void InitialSliderValue()
    {
        if (SettingPanel.instance == null || slider == null)
        {
            Debug.LogError("[Settings] 初始化失败: SettingPanel.instance 或 slider 为空！");
            return;
        }

        SettingConfigue settingConfigue = SettingPanel.instance.GetSettingConfigue();
        if (settingConfigue == null)
        {
            Debug.LogError("[Settings] 初始化失败: 获取到的 SettingConfigue 为空！");
            return;
        }

        Type type = settingConfigue.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        object rawValue = null;
        bool found = false;

        // 1. 优先尝试读取字段 (Field)
        FieldInfo field = type.GetField(controlValueName, flags);
        if (field != null)
        {
            rawValue = field.GetValue(settingConfigue);
            found = true;
        }
        else
        {
            // 2. 如果没找到字段，尝试读取属性 (Property)
            PropertyInfo property = type.GetProperty(controlValueName, flags);
            if (property != null && property.CanRead)
            {
                rawValue = property.GetValue(settingConfigue);
                found = true;
            }
        }

        // 3. 未找到目标成员
        if (!found)
        {
            Debug.LogWarning($"[Settings] 初始化跳过: 在 SettingConfigue 中未找到 '{controlValueName}'");
            return;
        }

        // 4. 安全转换为 float 并赋值给 Slider
        try
        {
            // 处理可能的 null 值（例如 string 或 Nullable<T>）
            if (rawValue == null)
            {
                Debug.LogWarning($"[Settings] '{controlValueName}' 的值为 null，Slider 将使用默认值 {slider.value}");
                return;
            }

            float targetValue = Convert.ToSingle(rawValue);
            //Debug.Log($"[VoiceSliderSetter]: targetValue:" + targetValue);
            // 可选：钳制到 Slider 的 min/max 范围，防止越界
            targetValue = Mathf.Clamp(targetValue, (float)slider?.minValue, (float)slider?.maxValue);
            //Debug.Log($"[VoiceSliderSetter]: Voice Value:" + targetValue);
            if(slider != null) slider.value = targetValue;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Settings] 初始化失败: 无法将 '{controlValueName}' ({rawValue.GetType().Name}) 转换为 float。错误: {e.Message}");
        }
    }

    [SerializeField] private string controlValueName;//控制的SettingConfigue中的字段名称
    //按钮点击时调用
    public void SetVoiceVolumToSettingConfigue()
    {
        if (SettingPanel.instance == null)
        {
            Debug.LogError("[VoiceSliderSetter] SettingPanel.instance is null!");
            return;
        }

        // 1. 获取配置对象
        SettingConfigue settingConfigue = SettingPanel.instance.GetSettingConfigue();
        if (settingConfigue == null)
        {
            Debug.LogError("[VoiceSliderSetter]: The SettingConfigue is null!");
            return;
        }

        Type type = settingConfigue.GetType();
        // 定义搜索标志：查找实例成员，包括公开和私有的字段/属性
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // 2. 优先尝试查找字段 (Field)
        FieldInfo field = type.GetField(controlValueName, flags);
        if (field != null)
        {
            AssignValue(field.FieldType, (val) => field.SetValue(settingConfigue, val));
            HandleValueTypeWriteBack(settingConfigue);
            return;
        }

        // 3. 如果没找到字段，尝试查找属性 (Property)
        PropertyInfo property = type.GetProperty(controlValueName, flags);
        if (property != null)
            Debug.LogError($"[VoiceSliderSetter] Not Find the Field Of Property name of \"'{controlValueName}'\" in settingConfigue!");
    }

    /// <summary>
    /// 处理类型转换并执行赋值
    /// </summary>
    private void AssignValue(Type targetType, Action<object> setValueAction)
    {
        try
        {
            // 将 slider 的 float 值安全转换为目标类型 (例如 float -> int)
            object convertedValue = Convert.ChangeType((float)slider?.value, targetType);
            setValueAction(convertedValue);
            //Debug.Log($"[VoiceSliderSetter]: Set '{controlValueName}' To {convertedValue} Successful!");
        }
        catch (Exception e)
        {
            Debug.LogError($"[VoiceSliderSetter]: Set Faild => Can's  Convert slider.value(float) To {targetType.Name}! Error Message: {e.Message}");
        }
    }

    /// <summary>
    /// 处理值类型 (struct) 的写回问题
    /// </summary>
    private void HandleValueTypeWriteBack(SettingConfigue config)
    {
        if (config.GetType().IsValueType)
        {
            Debug.LogWarning("[Settings] SettingConfigue 是值类型 (struct)。" +
                             "如果 GetSettingConfigue() 返回的是副本，请确保调用 SetSettingConfigue() 将修改后的值写回！");

            // 如果你的 SettingPanel 提供了写回方法,请取消下面的注释：
            //SettingPanel.instance.SetSettingConfigue(config);
        }
    }

    public void TriggerTheVoiceChange()
    {
        //将音频设置应用到游戏
        SettingPanel.instance?.ApplyVoiceSettingsToGame();
    }
}
