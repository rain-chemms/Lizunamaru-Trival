using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenSettingPanelFunctioner : MonoBehaviour
{
    [SerializeField] private Button button;
    public Button GetButton() => button;

    void OnEnable()
    {
        if(button == null) button = GetComponent<Button>();
        button?.onClick.AddListener(OpenSettingPanel);
    }
    
    void OnDisable()
    {
        button?.onClick.RemoveListener(OpenSettingPanel);
    }

    public void OpenSettingPanel()
    {
        SettingPanel.instance.GetComponent<Animator>().SetBool("IsOpen",true);//控制动画器打开面板
    }
}
