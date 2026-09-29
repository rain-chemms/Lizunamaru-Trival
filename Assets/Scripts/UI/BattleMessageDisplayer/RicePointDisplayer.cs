using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(TMP_Text))]
public class RicePointDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if(text == null) text = GetComponent<TMP_Text>();    
    }
    
    void Start()
    {
        lastRicePoint = (uint)BattleMessage.instance?.GetRicePoint();
        if(text != null) text.text = lastRicePoint.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        SyncRicePoint();
    }

    [NonSerialized] private uint lastRicePoint;
    private void SyncRicePoint()
    {
        if(text == null) return;
        uint nowRicePoint = (uint)BattleMessage.instance?.GetRicePoint();
        if(nowRicePoint == lastRicePoint) return;
        text.text = nowRicePoint.ToString();
        lastRicePoint = nowRicePoint;
    } 
}
