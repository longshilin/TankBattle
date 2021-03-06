﻿using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// 炮弹类。
    /// </summary>
    public class Bullet : Entity {
        public LayerMask m_TankMask;

        [SerializeField]
        private BulletData m_BulletData = null;

        // Bullet Impact Data
        public ImpactData GetImpactData() {
            return new ImpactData(m_BulletData.OwnerCamp, 0, m_BulletData.Attack, 0);
        }

        // 考虑到对象池，一个Bullet对象会被复用，只有在创建Bullet对象的时候会执行一次OnInit方法。
        protected override void OnInit(object userData) {
            base.OnInit(userData);
            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            // 获取Targetable层
            m_TankMask = LayerMask.GetMask(Constant.Layer.TargetableObjectLayerName);
        }

        // 对象池中的Bullt对象，只要被复用时，都会重新执行OnShow方法。
        protected override void OnShow(object userData) {
            base.OnShow(userData);
            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            m_BulletData = userData as BulletData;
            if (m_BulletData == null) {
                Log.Error("Bullet data is invalid.");
                return;
            }

            // 每次复用Bullet对象时刷新炮弹的发射速率，而炮弹发射的position和rotation已经在show bullet的时候，进行初始化了。
            GetComponent<Rigidbody>().velocity = m_BulletData.Velocity;
        }

        // 赋予实体的刚体组件的触发逻辑脚本
        private void OnTriggerEnter(Collider other) {
            // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_BulletData.ExplosionRadius, m_TankMask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++) {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
                Tank targetTank = colliders[i].GetComponent<Tank>();

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody) { continue; }

                // Add an explosion force. 加入炮弹的爆炸力
                //targetRigidbody.AddExplosionForce(m_BulletData.ExplosionForce, transform.position, m_BulletData.ExplosionRadius);

                // Tank entity
                Tank entity = targetRigidbody.gameObject.GetComponent<Tank>();
                if (!entity) { continue; }

                // 坦克被攻击扣血
                AIUtility.PerformCollision(entity, this, m_BulletData);
            }

            // 播放炮弹爆炸的特效
            GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), m_BulletData.ExplosionEffectId) {
                Position = CachedTransform.position,
            });

            // 播放炮弹爆炸的音效
            GameEntry.Sound.PlaySFX(m_BulletData.ExplosionSoundId);

            // Hide the shell.
            GameEntry.Entity.HideEntity(this);
        }

        private void Update() {
            //Debug.Log(CachedTransform.position);
        }
    }
}