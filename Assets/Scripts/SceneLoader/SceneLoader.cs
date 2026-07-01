using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private Animator animator;
    public Animator GetAnimator()
    {   
        return animator;
    }

    public void LoadScene(String sceneName,Action callbacks = null)
    {
        StartCoroutine(LoadCoroutine(sceneName,callbacks));
    }

    private IEnumerator LoadCoroutine(String sceneName,Action callbacks = null)
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
        callbacks?.Invoke();
        animator.SetTrigger("End");
    }
}
