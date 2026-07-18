using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//存储一个表情对应的数据
[CreateAssetMenu(fileName = "MMD4FaceMorphData", menuName = "MMD4Mecanim Extension/MMD4FaceMorphData")]
public class MMD4FacceMorphData : ScriptableObject
{
    /*
    [SerializeField] private string morphName;
    public void SetMorphName(string newName) => morphName = newName;
    public string GetMorphName() => morphName;
    [SerializeField][Range(0f, 1f)] private float weight;
    public void SetWeight(float newWeight) => weight = newWeight;
    public float GetWeight() => weight;
    */
    [SerializeField] private SerializableDictionary<string, float> morphsDict;
    public SerializableDictionary<string, float> GetMorphsDict() => morphsDict;
}
