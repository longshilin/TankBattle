using System;
using UnityEngine;

namespace TankBattle
{
    [Serializable]
    public class MyTankData : TankData
    {
        [SerializeField]
        private string m_Name = null;

        [SerializeField]
        private int m_Wins = 0;

        public MyTankData(int entityId, int typeId)
            : base(entityId, typeId, CampType.Player)
        {

        }

        /// <summary>
        /// 角色名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        public int Wins { get => m_Wins; set => m_Wins = value; }
    }
}
