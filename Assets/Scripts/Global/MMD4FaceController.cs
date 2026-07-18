using UnityEngine;
using System.Collections.Generic;
using MMD4Mecanim;
using System.Linq;

[RequireComponent(typeof(MMD4MecanimModel))]
public class MMD4FaceController : MonoBehaviour
{
    [SerializeField] private MMD4MecanimModel model;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (model == null) model = GetComponent<MMD4MecanimModel>();
        FreshMorphList();
    }
    [SerializeField] List<MMD4MecanimModelImpl.Morph> morphList = new List<MMD4MecanimModelImpl.Morph>();
    private void FreshMorphList()
    {
        morphList.Clear();
        foreach (MMD4MecanimModelImpl.Morph morph in model.morphList.ToList())
        {
            morphList.Add(morph);
        }
    }
    //设置MMD转化后模型的表情权重
    //这个方法在动画器中调用
    public void SetMorph(MMD4FacceMorphData data)
    {
        SerializableDictionary<string, float> morphsDict = data.GetMorphsDict();
        foreach (KeyValuePair<string, float> morphData in morphsDict)
        {
            string dataName = morphData.Key;
            float weight = morphData.Value > 1f? 1f : morphData.Value < 0f ? 0f : morphData.Value;
            foreach (MMD4MecanimModelImpl.Morph morph in morphList)
            {
                if (morph.name.Equals(dataName))
                {
                    morph.weight = weight;
                    break;
                }
            }
        }
    }
}
