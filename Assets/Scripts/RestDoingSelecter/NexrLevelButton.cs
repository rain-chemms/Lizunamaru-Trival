using System;
using MapSystem;
using UnityEngine;
using UnityEngine.UI;

namespace RestDoingSystem
{
    //需求操作,每种对应的需求操作都继承这个脚本
    [RequireComponent(typeof(Button))]
    public class NextLevelButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        void OnEnable()
        {
            if(button == null) button = GetComponent<Button>();
            button.onClick.AddListener(OpenMap);
        }
        void OnDisable()
        {
            button.onClick.RemoveListener(OpenMap);
        }

        private void OpenMap()
        {
            Map.instance.SetDisplay(true);
        }
    }
}