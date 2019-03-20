using System;
using GameFramework.DataTable;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle {

    /// <summary>
    /// 武器数据类，主要进行的功能是：
    ///     1. 注册武器数据对象，区分敌我阵营
    ///     2. 在武器实体对象中，加载表中的数据并对外提供get接口。
    /// </summary>
    [Serializable]
    public class WeaponData : AccessoryObjectData {

        [SerializeField]
        private float m_AttackInterval = 0f;        //武器的攻击间隔时间

        [SerializeField]
        private int m_BulletId = 0;                 // 武器发出的子弹Id编号

        [SerializeField]
        private float m_BulletSpeed = 0f;           // 子弹的射速

        [SerializeField]
        private int m_BulletChargingSoundId = 0;    // 炮弹蓄力时播放的音效编号

        [SerializeField]
        private int m_BulletFiringSoundId = 0;      // 炮弹发射时播放的音效编号

        [SerializeField]
        private float m_MinLaunchForce;            // 在不发射时默认的最小蓄力

        [SerializeField]
        private float m_MaxLaunchForce;            // 在最大充能时间时可获得的最大蓄力

        [SerializeField]
        private float m_MaxChargeTime;             // 炮弹在最大蓄力发射之前所耗费的充能时间

        /// 武器实体的初始化
        ///     主要进行武器对哪一方造成伤害的界定，为武器 绑定实体ID，实体类型编号，实体所有者编号，武器所有者所处的阵营类型。
        ///
        ///     对武器数据的序列化字段进行实例化。
        public WeaponData(int entityId, int typeId, int ownerId, CampType ownerCamp)
            : base(entityId, typeId, ownerId, ownerCamp) {
            IDataTable<DRWeapon> dtWeapon = GameEntry.DataTable.GetDataTable<DRWeapon>();
            DRWeapon drWeapon = dtWeapon.GetDataRow(TypeId);
            if (drWeapon == null) {
                return;
            }

            m_AttackInterval = drWeapon.AttackInterval;
            m_BulletId = drWeapon.BulletId;
            BulletChargingSoundId = drWeapon.BulletChargingSoundId;
            BulletFiringSoundId = drWeapon.BulletFiringSoundId;

            m_MinLaunchForce = drWeapon.MinLaunchForce;
            m_MaxLaunchForce = drWeapon.MaxLaunchForce;
            m_MaxChargeTime = drWeapon.MaxChargeTime;
        }

        /// <summary>
        /// 攻击间隔。
        /// </summary>
        public float AttackInterval {
            get {
                return m_AttackInterval;
            }
        }

        /// <summary>
        /// 子弹编号。
        /// </summary>
        public int BulletId {
            get {
                return m_BulletId;
            }
        }

        /// <summary>
        /// 子弹速度。
        /// </summary>
        public float BulletSpeed {
            get {
                return m_BulletSpeed;
            }
        }

        public float MinLaunchForce {
            get => m_MinLaunchForce;
            set => m_MinLaunchForce = value;
        }

        public float MaxLaunchForce {
            get => m_MaxLaunchForce;
            set => m_MaxLaunchForce = value;
        }

        public float MaxChargeTime {
            get => m_MaxChargeTime;
            set => m_MaxChargeTime = value;
        }

        public int BulletChargingSoundId { get => m_BulletChargingSoundId; set => m_BulletChargingSoundId = value; }
        public int BulletFiringSoundId { get => m_BulletFiringSoundId; set => m_BulletFiringSoundId = value; }
    }
}