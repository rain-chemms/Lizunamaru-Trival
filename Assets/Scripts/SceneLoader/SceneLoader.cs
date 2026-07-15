using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

//场景加载器:使用单例模式
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Canvas))]
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private Animator animator;
    public Animator GetAnimator()
    {   
        return animator;
    }

    //传入的Action必须为返回类型为空的,传入参数为空的委托
    //表示在场景加载完之后要执行的逻辑,可以使用一个返回类型为空的且传入参数为空的函数来包装复杂方法
    public void LoadScene(String sceneName,Action callbacks = null)
    {
        StartCoroutine(LoadCoroutine(sceneName,callbacks));
    }

    private IEnumerator LoadCoroutine(String sceneName,Action actionCallbacks = null)
    {
        animator.SetTrigger("Start");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);//等待动画播放完毕
        SceneManager.LoadScene(sceneName);
        AsyncOperation asyncOperation =  SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Single);
        //等待加载完成
        while (!asyncOperation.isDone)
        {
            yield return null; // 每帧检查一次
        }
        //触发Action功能
        actionCallbacks?.Invoke();
        //解构事件
        if(actionCallbacks!=null)
        foreach(Action ac in actionCallbacks.GetInvocationList().ToList())
        {
            if(ac != null) actionCallbacks -= ac;
        }
        animator.SetTrigger("End");
    }
}
