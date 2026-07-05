using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ConcentratePoint))]
public class ConcentratePointDisplayer : MonoBehaviour
{
    [SerializeField] private ConcentratePoint concentratePoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(concentratePoint == null) concentratePoint = GetComponent<ConcentratePoint>();
        mrl = GetComponentsInChildren<MeshRenderer>().ToList();//获取所有网格渲染器
    }
    [SerializeField] private List<MeshRenderer> mrl;
    // Update is called once per frame
    void Update()
    {
        ChangeDisplayState();
    }

    private void ChangeDisplayState()
    {
        if(concentratePoint == null) return;
        if(mrl == null) return;
        // 开启全部的网格渲染器
        if (concentratePoint.IsDisplay())
        {
            foreach(MeshRenderer mr in mrl)
            {
                mr.enabled = true;
            }
        }
        else//关闭全部的网格渲染器
        {
            foreach (MeshRenderer mr in mrl)
            {
                mr.enabled = false;
            }
        }
    }
}
