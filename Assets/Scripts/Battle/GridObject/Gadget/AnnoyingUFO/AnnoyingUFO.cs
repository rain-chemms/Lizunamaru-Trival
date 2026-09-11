using UnityEngine;
using BulletSystem;
using System.Collections;

namespace GridObjectSystem.GadgetSystem.UFOs
{
    //烦人的UFO:相关角色封兽鵺
    //在XoZ平面上旋转并消除非激光子弹
    public class AnnoyingUFO : Gadget
    {
        //检测是否有其他子弹进入
        void OnTriggerEnter(Collider other)
        {
            Bullet bt = other.GetComponent<Bullet>();
            if(bt != null)
            {
                bool btSide = bt.GetSide();
                BulletDamageTrigger bdt = bt.GetComponent<BulletDamageTrigger>();
                if(btSide != GetSide() && !(bool)bdt?.IsLaserMode())//若进入的是敌方子弹
                {
                    //尝试使用BulletDestroyController消除
                    BulletDestroyController bdc = bt.GetComponent<BulletDestroyController>();
                    if(bdc != null)
                    {
                        bdc.TriggerTheDestroy();
                    }
                    else
                    {
                        //直接消除子弹物体
                        Destroy(bt.gameObject);
                    }
                }
            }
        }
    }
}