﻿using GameFramework.DataTable;
using System;
using UnityEngine;

namespace TankBattle
{
    [Serializable]
    public class ArmorData : AccessoryObjectData
    {
        [SerializeField]
        private float m_MaxHP = 0;

        [SerializeField]
        private int m_Defense = 0;

        public ArmorData(int entityId, int typeId, int ownerId, CampType ownerCamp)
            : base(entityId, typeId, ownerId, ownerCamp)
        {
            IDataTable<DRArmor> dtArmor = GameEntry.DataTable.GetDataTable<DRArmor>();
            DRArmor drArmor = dtArmor.GetDataRow(TypeId);
            if (drArmor == null)
            {
                return;
            }

            m_MaxHP = drArmor.MaxHP;
            m_Defense = drArmor.Defense;
        }

        /// <summary>
        /// 最大生命。
        /// </summary>
        public float MaxHP
        {
            get
            {
                return m_MaxHP;
            }
        }

        /// <summary>
        /// 防御力。
        /// </summary>
        public int Defense
        {
            get
            {
                return m_Defense;
            }
        }
    }
}