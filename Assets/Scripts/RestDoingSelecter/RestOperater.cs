using System;
using MapSystem;
using UnityEngine;
using UnityEngine.UI;

namespace RestDoingSystem
{
    //需求操作,每种对应的需求操作都继承这个脚本
    [RequireComponent(typeof(Button))]
    public class RestOperater : MonoBehaviour
    {
        [SerializeField] private Button button;
        void OnEnable()
        {
            if(button == null) button = GetComponent<Button>();
            button.onClick.AddListener(OperateEffect);
        }
        void OnDisable()
        {
            button.onClick.RemoveListener(OperateEffect);
        }

        //OperateOver时设置自身的透明度
        void Update()
        {
            LerpAlphaByData();
        }

        [NonSerialized] private const float alphaLerpSpeed = 0.05f;
        private void LerpAlphaByData()
        {
            if(RestDoingSelecter.instance == null) return;
            bool operateOver = RestDoingSelecter.instance.OperateOver;
            float targetAlpha = operateOver? 0.0f : 1.0f;
            Color source = button.image.color;
            button.image.color = Color.Lerp(source,new Color(source.r,source.g,source.b,targetAlpha),Time.deltaTime * alphaLerpSpeed);
        }

        protected virtual void OperateEffect()
        {
            //操作完成后要将地图解锁
            Map.instance.MapLocked = false;
            RestDoingSelecter.instance.OperateOver = true;//设置状态为执行完毕
        }
    }
}
