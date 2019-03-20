using GameFramework.DataTable;
using UnityEngine;

namespace TankBattle {

    public class ThrusterData : AccessoryObjectData {

        [SerializeField]
        private float m_Speed = 0f;

        [SerializeField]
        private float m_TurnSpeed = 0f;

        [SerializeField]
        private float m_PitchRange = 0f;

        [SerializeField]
        private int m_IdleSound = 0;

        [SerializeField]
        private int m_DriveSound = 0;

        public ThrusterData(int entityId, int typeId, int ownerId, CampType ownerCamp) : base(entityId, typeId, ownerId, ownerCamp) {
            IDataTable<DRThruster> dtTrack = GameEntry.DataTable.GetDataTable<DRThruster>();
            DRThruster drTrack = dtTrack.GetDataRow(TypeId);
            if (drTrack == null) {
                return;
            }

            m_Speed = drTrack.Speed;
            m_TurnSpeed = drTrack.TurnSpeed;
            m_PitchRange = drTrack.PitchRange;
            m_IdleSound = drTrack.IdleSound;
            m_DriveSound = drTrack.DriveSound;
        }

        /// <summary>
        /// 坦克移速
        /// </summary>
        public float Speed { get => m_Speed; }

        /// <summary>
        /// 坦克转方向速度
        /// </summary>
        public float TurnSpeed { get => m_TurnSpeed; }

        /// <summary>
        /// 坦克引擎音调
        /// </summary>
        public float PitchRange { get => m_PitchRange; }

        public int IdleSound { get => m_IdleSound; set => m_IdleSound = value; }
        public int DriveSound { get => m_DriveSound; set => m_DriveSound = value; }
    }
}