using UnityEngine;
using System.Collections;

public class LogoAnimatorEvent : MonoBehaviour
{
    //Logo播放完之后加载主场景
    [SerializeField] private string loadSceneName = "MenuScene";
    public void AfterTheLoadEnd()
    {
        SceneLoader.instance.LoadScene(loadSceneName);
    }
}
