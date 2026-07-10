using UnityEngine;
using System.Linq;
using System.Collections.Generic;

//这个脚本只修改Bullet的方向参数
[RequireComponent(typeof(Bullet))]
public class BulletForceAppender : BulletAppender
{
    [SerializeField] private float appendForce = 1.0f;//追加的力
    [SerializeField] private bool appendPreFrame = false;//是否在每帧都追加力
    [SerializeField] private bool haveAppendForce = false;//是否有追加力
    protected override void Start()
    {
        base.Start();
        haveAppendForce = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void AfterHaltFunctionPreFrame()
    {
        base.AfterHaltFunctionPreFrame();
        if(!appendPreFrame)
        {
            if(!haveAppendForce)
            {
                AppendForceToBullet();
                haveAppendForce = true;
            }
        }
        else
        {
            AppendForceToBullet();
        }
    }

    private void AppendForceToBullet()
    {
        if(bullet == null) return;
        bullet.SetForce(bullet.GetForce() + appendForce);
    }
}

