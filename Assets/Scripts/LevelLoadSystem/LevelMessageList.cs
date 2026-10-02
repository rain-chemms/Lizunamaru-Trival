using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using System.Collections;
using System;
using LevelLoadSystem.LoadInMessageSystem;

namespace LevelLoadSystem
{
    //要加载的所有关卡的管理器,分类存储它们的数据
    //包括事件,战斗(Boss战斗)
    //它要放在LevelController之下
    public class LevelMessageList : MonoBehaviour
    {
        [SerializeField] private MapNodeCategory manageCategory;//当前列表管理的类别
        public MapNodeCategory GetManageCategory() => manageCategory;
        public void SetManagerCategory(MapNodeCategory ctg) => manageCategory = ctg; 

        [SerializeField] private List<LevelLoadInMessage> messageList = new List<LevelLoadInMessage>();
        public List<LevelLoadInMessage> GetMessageList() => messageList;
        //在初始化时,自动从资源列表中获取对应类别的关卡加载数据
        void OnEnable()
        {
            StartCoroutine(LoadMessageList());
        }

        private IEnumerator LoadMessageList()
        {
            //清空旧的条目
            messageList.Clear();
            var keys = new List<string> { "LevelLoadInMessage" };
            //分类的时候不是按照卡牌的命名空间分类,而是依据其资源标签分到不同的卡池中
            //卡池存有这张卡的唯一标识符和其预制体
            // 1. 发起 Addressables 异步加载请求
            var handle = Addressables.LoadAssetsAsync<ScriptableObject>(keys, null, Addressables.MergeMode.Intersection);
            // 2. 使用 yield return 等待加载完成（不阻塞主线程）
            yield return handle;
            // 3. 检查加载状态是否成功
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                List<ScriptableObject> objs = handle.Result.ToList();
                //载入对应的资源
                foreach (ScriptableObject oj in objs)
                {
                    LevelLoadInMessage msg = oj as LevelLoadInMessage;
                    if(msg == null) continue;
                    if((bool)messageList?.Contains(msg)) continue;
                    if(msg.GetCategory() != manageCategory) continue;
                    messageList?.Add(msg);
                }
            }
            else
            {
                // 可选：打印加载失败的日志
                Debug.LogWarning($"[LevelMessageList]: Card Prefab Load Faild: {handle.OperationException}");
            }
            // 4. 重要：释放句柄，防止内存泄漏
            handle.Release();
        }
    }
    
}
