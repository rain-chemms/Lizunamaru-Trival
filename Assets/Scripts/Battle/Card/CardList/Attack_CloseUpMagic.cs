using System.Collections;
using GridObjectSystem.GadgetSystem.Hakerros;
using UnityEngine;
using GridObjectSystem.RoleSystem;
using GridObjectSystem.GadgetSystem;
using BulletSystem;

namespace CardSystem.AllCardHub
{
    // 星剑飞舞(Close-Up Magic):咲夜623之一
    // 围绕自身 {2} 格产生 {0} 把旋转的飞刀
    // 每次飞刀接触都会对敌人造成 {1} 伤害
    public class Attack_CloseUpMagic : Card, IAttackCardInsertSlotSetter
    {
        [SerializeField] private int recoverPreRound = 1;
        public int RecoverPreRound
        {
            get => recoverPreRound;
            set => recoverPreRound = value;
        }

        public override IEnumerator AfterPlay()
        {
            yield return AfterTriggerEffective();
            yield return base.AfterPlay();
        }

        public override IEnumerator AfterTriggerEffective()
        {
            yield return CreateKnife();
            yield return base.AfterTriggerEffective();
        }
        public override IEnumerator AfterInsertToSolt()
        {
            Debug.Log("[Attack_MiracleMallet]: Insert To Slot!");
            ((IAttackCardInsertSlotSetter)this).SetInsertSlotAttackTime(this);
            yield return base.AfterInsertToSolt();
        }

        [SerializeField] private Bullet satelliteKnife;//星剑预制体
        [SerializeField] private uint numberOfKnife = 6;//产生的星剑的数目
        [SerializeField] private uint bulletDamage = 3;//星剑的伤害
        [SerializeField] private uint gapDistance = 1;//范围,用于设置星剑的环绕半径
        private IEnumerator CreateKnife()
        {
            Role player = BattleMessage.instance.GetControlPlayer();
            Rigidbody center = player?.GetRigidBody();
            Vector3 centerPos = (Vector3)center?.worldCenterOfMass;
            float dirUnit = 360 / numberOfKnife;
            Vector2 gaps = BattleBoard.instance.GetGapsOfGrid();
            float maxGap = gaps.x > gaps.y ? gaps.x : gaps.y;
            float radius = maxGap * gapDistance;
            for (int i = 0; i < numberOfKnife; i++)
            {//产生单个子弹的方法
                Bullet bt = Instantiate(satelliteKnife);
                Rigidbody bRb = bt?.GetRigidBody();
                //设置每个子弹的初始位置
                float dir = dirUnit * i;//对应的偏转方向
                //Quaternion angle = Quaternion.Euler(0, dir, 0);//获取旋转角
                //Vector3 offset = angle * new Vector3(radius, 0, 0);
                /*一个意思
                    Vector3 offset = angle * Vector3.forward * radius;
                */
                //if (bRb != null) bRb.transform.position = offset + centerPos;
                //else bt.transform.position = offset + centerPos;
                bt.SetDamage(bulletDamage);
                bt.SetSide((bool)player?.GetSide());
                BulletSatelliteStater stater = bt.GetComponent<BulletSatelliteStater>();
                //星剑的范围是一格格子的范围需要计算
                stater.Center = center;
                stater.DistanceToCenter = radius;
                stater.AngleOffset = dir;
                //需要手动触发额外效果
                BattleMessage.instance?.BulletPostProcess?.Invoke(bt);
            }
            yield return null;
        }
    }
}
