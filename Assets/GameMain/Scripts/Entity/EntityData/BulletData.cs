using System;
using GameFramework.DataTable;
using UnityEngine;

namespace TankBattle {

    [Serializable]
    public class BulletData : EntityData {

        [SerializeField]
        private int m_OwnerId = 0;

        [SerializeField]
        private CampType m_OwnerCamp = CampType.Unknown;

        [SerializeField]
        private int m_Attack = 0;

        [SerializeField]
        private float m_ExplosionForce = 0;

        [SerializeField]
        private float m_MaxLifeTime = 0;

        [SerializeField]
        private float m_ExplosionRadius = 0;

        [SerializeField]
        private Vector3 m_Velocity = Vector3.zero;

        [SerializeField]
        private Vector3 m_ShootTransform = Vector3.zero;

        [SerializeField]
        private int m_ExplosionEffectId = 0;

        [SerializeField]
        private int m_ExplosionSoundId = 0;

        /// 一个炮弹对象拥有的属性
        public BulletData(int entityId, int typeId, int ownerId, CampType ownerCamp) : base(entityId, typeId) {
            IDataTable<DRBullet> dtBullet = GameEntry.DataTable.GetDataTable<DRBullet>();
            DRBullet drBullet = dtBullet.GetDataRow(TypeId);
            if (drBullet == null) {
                return;
            }

            m_OwnerId = ownerId;
            m_OwnerCamp = ownerCamp;
            m_Attack = drBullet.Attack;
            m_ExplosionForce = drBullet.ExplosionForce;
            m_MaxLifeTime = drBullet.MaxLifeTime;
            m_ExplosionRadius = drBullet.ExplosionRadius;
            m_ExplosionEffectId = drBullet.ExplosionEffectId;
            m_ExplosionSoundId = drBullet.ExplosionSoundId;
        }

        // Bullet rigidBody velocity
        public Vector3 Velocity {
            get {
                return m_Velocity;
            }
            set {
                m_Velocity = value;
            }
        }

        // Bullet Shoot Transform
        public Vector3 ShootTransform {
            get {
                return m_ShootTransform;
            }
            set {
                m_ShootTransform = value;
            }
        }

        public int OwnerId {
            get {
                return m_OwnerId;
            }
        }

        public CampType OwnerCamp {
            get {
                return m_OwnerCamp;
            }
        }

        public int Attack {
            get => m_Attack;
        }

        public float ExplosionForce {
            get => m_ExplosionForce;
        }

        public float MaxLifeTime {
            get => m_MaxLifeTime;
        }

        public float ExplosionRadius {
            get => m_ExplosionRadius;
        }

        public int ExplosionEffectId { get => m_ExplosionEffectId; set => m_ExplosionEffectId = value; }
        public int ExplosionSoundId { get => m_ExplosionSoundId; set => m_ExplosionSoundId = value; }
    }
}