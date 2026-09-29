using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TMP_Text))]
public class IcePointDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    void OnEnable()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        lastIcePoint = (uint)BattleMessage.instance?.GetIcePoint();
        if (text != null) text.text = lastIcePoint.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        SyncIcePoint();
    }

    [NonSerialized] private uint lastIcePoint;
    private void SyncIcePoint()
    {
        if (text == null) return;
        uint nowIcePoint = (uint)BattleMessage.instance?.GetIcePoint();
        if (nowIcePoint == lastIcePoint) return;
        text.text = nowIcePoint.ToString();
        lastIcePoint = nowIcePoint;
    }
}
