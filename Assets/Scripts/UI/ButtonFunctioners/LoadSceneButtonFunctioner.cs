using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadSceneButtonFunctioner : MonoBehaviour
{
    [SerializeField] private string loadSceneName;
    [SerializeField] private bool autoLinkFunction = false;//是否自动链接触发功能
    public void SetLoadSceneName(string sceneName)
    {
        this.loadSceneName = sceneName;
    }
    public string GetLoadSceneName()
    {
        return loadSceneName;
    }

    [SerializeField] private Button button; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(button == null) button = GetComponent<Button>();
        if(autoLinkFunction) button.onClick.AddListener(StartLoadScene);
    }

    public void StartLoadScene()
    {
        if(!string.IsNullOrEmpty(loadSceneName)) SceneLoader.instance?.LoadScene(loadSceneName);
    }
}
