using System;
using UnityEngine;

namespace TankBattle {

    [Serializable]
    public class EnemyTankData : TankData {

        [SerializeField]
        private string m_TankId = null;

        [SerializeField]
        private string m_Name = null;

        [SerializeField]
        private int m_Wins = 0;

        public EnemyTankData(int entityId, int typeId, string tankId)
            : base(entityId, typeId, CampType.Player, tankId) {
            m_TankId = base.TankId;
        }

        /// <summary>
        /// 角色名称。
        /// </summary>
        public string Name {
            get {
                return m_Name;
            }
            set {
                m_Name = value;
            }
        }

        public int Wins { get => m_Wins; set => m_Wins = value; }
        public string TankId { get => m_TankId; set => m_TankId = value; }
    }
}