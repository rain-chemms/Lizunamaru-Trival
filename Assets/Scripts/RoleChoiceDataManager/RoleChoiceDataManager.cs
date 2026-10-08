using System.Collections.Generic;
using System.Linq;
using RoleChoiceUISystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections;

public class RoleChoiceDataManager : MonoBehaviour
{
    public static RoleChoiceDataManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private List<RoleChoiceData> choiceDataList;
    public List<RoleChoiceData> ChoiceDataList { get => choiceDataList; }
    public List<RoleChoiceData> ChoiceDataList_Copy { get => choiceDataList.ToList(); }

    //游戏开始时自动获取所有资源列表中的RoleChoiceData标签的物体
    void OnEnable()
    {
        StartCoroutine(InitRoleChoiceDataList());
    }

    private IEnumerator InitRoleChoiceDataList()
    {
        List<string> tags = new List<string>() { "RoleChoiceData" };
        var handle = Addressables.LoadAssetsAsync<ScriptableObject>(tags, null, Addressables.MergeMode.Intersection);
        //等待加载完成
        yield return handle;
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            choiceDataList.Clear();
            List<ScriptableObject> srcs = handle.Result.ToList();
            foreach (ScriptableObject s in srcs)
            {
                RoleChoiceData data = s as RoleChoiceData;
                if(data != null)
                {
                    choiceDataList.Add(data);
                }
            }
        }

        else
        {
            // 可选：打印加载失败的日志
            Debug.LogWarning($"[CardPool]: Card Prefab Load Faild: {handle.OperationException}");
        }
        // 4. 重要：释放句柄，防止内存泄漏
        handle.Release();
    }
}
