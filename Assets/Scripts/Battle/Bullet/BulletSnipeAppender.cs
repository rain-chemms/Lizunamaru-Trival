using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BulletSnipeAppender : BulletAppender
{
    protected override void Start()
    {
        base.Start();
        nearestEnermy = SearchNearestEnermy();
    }
    [SerializeField] private bool persistTrace = false;//是否持续追踪
    public bool IsPersistTrace()
    {
        return persistTrace;
    }
    public void SetPersistTrace(bool persistTrace)
    {
        this.persistTrace = persistTrace;
    }
    [SerializeField] private Role nearestEnermy = null;
    public Role GetNearestEnermy()
    {
        return nearestEnermy;
    }
    [SerializeField] private bool haveTrace = false;//是否已经追踪过了
    protected override void Update()
    {
        base.Update();
    }

    protected override void AfterHaltFunctionPreFrame()
    {
        base.AfterHaltFunctionPreFrame();
        if (!persistTrace)
        {
            if (!haveTrace)
            {
                TrackTheEnermy();
                haveTrace = true;
            }
        }
        else
        {
            TrackTheEnermy();
        }
    }

    private void TrackTheEnermy()
    {
        if (nearestEnermy == null) return;
        Vector3 direction = (nearestEnermy.transform.position - transform.position).normalized;
        bullet?.SetDirection(direction);
    }

    //寻找距离最近的敌人
    private Role SearchNearestEnermy()
    {
        if (bullet == null) return null;
        List<Role> roles = Object.FindObjectsByType<Role>(FindObjectsSortMode.None).ToList();
        if (roles == null) return null;
        float minDis = float.MaxValue;
        Role nearestRole = null;
        foreach (Role rl in roles)
        {
            if (rl == null) continue;
            if (rl.GetSide() == bullet.GetSide()) continue;
            float dis = Mathf.Abs(Vector3.Distance(transform.position, rl.transform.position));
            if (dis < minDis)
            {
                minDis = dis;
                nearestRole = rl;
            }
        }
        return nearestRole;
    }
}

