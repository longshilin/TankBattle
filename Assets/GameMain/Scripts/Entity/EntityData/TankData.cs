using System;
using GameFramework.DataTable;
using UnityEngine;

namespace TankBattle {

    [Serializable]
    public abstract class TankData : TargetableObjectData {

        [SerializeField]
        private ThrusterData m_ThrusterData = null;

        [SerializeField]
        private WeaponData m_WeaponData = null;

        [SerializeField]
        private ArmorData m_ArmorData = null;

        [SerializeField]
        private int m_Defense = 0;

        [SerializeField]
        private int m_DeadEffectId = 0;

        [SerializeField]
        private int m_DeadSoundId = 0;

        [SerializeField]
        private Color m_TankColor = Color.blue;

        public TankData(int entityId, int typeId, CampType camp)
            : base(entityId, typeId, camp) {
            IDataTable<DRTank> dtTank = GameEntry.DataTable.GetDataTable<DRTank>();
            DRTank drTank = dtTank.GetDataRow(TypeId);
            if (drTank == null) {
                return;
            }

            m_ThrusterData = new ThrusterData(GameEntry.Entity.GenerateSerialId(), drTank.ThrusterId, Id, Camp);

            m_WeaponData = new WeaponData(GameEntry.Entity.GenerateSerialId(), drTank.WeaponId, Id, Camp);

            m_ArmorData = new ArmorData(GameEntry.Entity.GenerateSerialId(), drTank.ArmorId, Id, Camp);

            m_DeadEffectId = drTank.DeadEffectId;
            m_DeadSoundId = drTank.DeadSoundId;
            m_TankColor = drTank.TankColor;

            HP = m_ArmorData.MaxHP;
            m_Defense = m_ArmorData.Defense;
        }

        /// <summary>
        /// 最大生命。
        /// </summary>
        public override float MaxHP {
            get {
                return m_ArmorData.MaxHP;
            }
        }

        /// <summary>
        /// 防御。
        /// </summary>
        public int Defense {
            get {
                return m_Defense;
            }
        }

        /// <summary>
        /// 速度。
        /// </summary>
        public float Speed {
            get {
                return m_ThrusterData.Speed;
            }
        }

        /// <summary>
        /// 坦克死亡特效
        /// </summary>
        public int DeadEffectId {
            get {
                return m_DeadEffectId;
            }
        }

        /// <summary>
        /// 坦克死亡声音
        /// </summary>
        public int DeadSoundId {
            get {
                return m_DeadSoundId;
            }
        }


        public ThrusterData GetThrusterData() {
            return m_ThrusterData;
        }

        public WeaponData GetWeaponData() {
            return m_WeaponData;
        }

        public ArmorData GetArmorData() {
            return m_ArmorData;
        }

    }
}
