//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// AI 工具类。
    /// </summary>
    public static class AIUtility {
        private static Dictionary<CampPair, RelationType> s_CampPairToRelation = new Dictionary<CampPair, RelationType>();
        private static Dictionary<KeyValuePair<CampType, RelationType>, CampType[]> s_CampAndRelationToCamps = new Dictionary<KeyValuePair<CampType, RelationType>, CampType[]>();

        static AIUtility() {
            s_CampPairToRelation.Add(new CampPair(CampType.Player, CampType.Player), RelationType.Friendly);
            s_CampPairToRelation.Add(new CampPair(CampType.Player, CampType.Enemy), RelationType.Hostile);
            s_CampPairToRelation.Add(new CampPair(CampType.Player, CampType.Neutral), RelationType.Neutral);
            s_CampPairToRelation.Add(new CampPair(CampType.Player, CampType.Player2), RelationType.Hostile);
            s_CampPairToRelation.Add(new CampPair(CampType.Player, CampType.Enemy2), RelationType.Hostile);
            s_CampPairToRelation.Add(new CampPair(CampType.Player, CampType.Neutral2), RelationType.Neutral);

            s_CampPairToRelation.Add(new CampPair(CampType.Enemy, CampType.Enemy), RelationType.Friendly);
            s_CampPairToRelation.Add(new CampPair(CampType.Enemy, CampType.Neutral), RelationType.Neutral);
            s_CampPairToRelation.Add(new CampPair(CampType.Enemy, CampType.Player2), RelationType.Hostile);
            s_CampPairToRelation.Add(new CampPair(CampType.Enemy, CampType.Enemy2), RelationType.Hostile);
            s_CampPairToRelation.Add(new CampPair(CampType.Enemy, CampType.Neutral2), RelationType.Neutral);

            s_CampPairToRelation.Add(new CampPair(CampType.Neutral, CampType.Neutral), RelationType.Neutral);
            s_CampPairToRelation.Add(new CampPair(CampType.Neutral, CampType.Player2), RelationType.Neutral);
            s_CampPairToRelation.Add(new CampPair(CampType.Neutral, CampType.Enemy2), RelationType.Neutral);
            s_CampPairToRelation.Add(new CampPair(CampType.Neutral, CampType.Neutral2), RelationType.Hostile);

            s_CampPairToRelation.Add(new CampPair(CampType.Player2, CampType.Player2), RelationType.Friendly);
            s_CampPairToRelation.Add(new CampPair(CampType.Player2, CampType.Enemy2), RelationType.Hostile);
            s_CampPairToRelation.Add(new CampPair(CampType.Player2, CampType.Neutral2), RelationType.Neutral);

            s_CampPairToRelation.Add(new CampPair(CampType.Enemy2, CampType.Enemy2), RelationType.Friendly);
            s_CampPairToRelation.Add(new CampPair(CampType.Enemy2, CampType.Neutral2), RelationType.Neutral);

            s_CampPairToRelation.Add(new CampPair(CampType.Neutral2, CampType.Neutral2), RelationType.Neutral);
        }

        /// <summary>
        /// 获取两个阵营之间的关系。
        /// </summary>
        /// <param name="first">阵营一。</param>
        /// <param name="second">阵营二。</param>
        /// <returns>阵营间关系。</returns>
        public static RelationType GetRelation(CampType first, CampType second) {
            if (first > second) {
                CampType temp = first;
                first = second;
                second = temp;
            }

            RelationType relationType;
            if (s_CampPairToRelation.TryGetValue(new CampPair(first, second), out relationType)) {
                return relationType;
            }

            Log.Warning("Unknown relation between '{0}' and '{1}'.", first.ToString(), second.ToString());
            return RelationType.Unknown;
        }

        /// <summary>
        /// 获取和指定具有特定关系的所有阵营。
        /// </summary>
        /// <param name="camp">指定阵营。</param>
        /// <param name="relation">关系。</param>
        /// <returns>满足条件的阵营数组。</returns>
        public static CampType[] GetCamps(CampType camp, RelationType relation) {
            KeyValuePair<CampType, RelationType> key = new KeyValuePair<CampType, RelationType>(camp, relation);
            CampType[] result = null;
            if (s_CampAndRelationToCamps.TryGetValue(key, out result)) {
                return result;
            }

            List<CampType> camps = new List<CampType>();
            Array campTypes = Enum.GetValues(typeof(CampType));
            for (int i = 0; i < campTypes.Length; i++) {
                CampType campType = (CampType)campTypes.GetValue(i);
                if (GetRelation(camp, campType) == relation) {
                    camps.Add(campType);
                }
            }

            result = camps.ToArray();
            s_CampAndRelationToCamps[key] = result;

            return result;
        }

        /// <summary>
        /// 获取实体间的距离。
        /// </summary>
        /// <returns>实体间的距离。</returns>
        public static float GetDistance(Entity fromEntity, Entity toEntity) {
            Transform fromTransform = fromEntity.CachedTransform;
            Transform toTransform = toEntity.CachedTransform;
            return (toTransform.position - fromTransform.position).magnitude;
        }

        /// <summary>
        /// get the two entity relative distance
        /// </summary>
        public static float GetRelativeDistance(Entity fromEntity, Entity toEntity, object userData) {
            // get the two entity distance, example: the shell to the Tank Distance.
            float explosionDistance = GetDistance(fromEntity, toEntity);

            BulletData bulletData = userData as BulletData;
            if (bulletData != null)
                return (bulletData.ExplosionRadius - explosionDistance) / bulletData.ExplosionRadius;

            return -1;
        }

        /// <summary>
        /// 目标刚体的表现逻辑：如扣血以及坦克被弹开
        ///     other Entity have two type: TargetableObject or Bullet
        /// </summary>
        /// <param name="entity">Targetable entity component, such as Tank entity</param>
        /// <param name="other">entity component, such as bullet entity</param>
        public static void PerformCollision(TargetableObject entity, Entity other, object userData) {
            if (entity == null || other == null) {
                return;
            }

            /* if other entity is targetable entity */
            TargetableObject target = other as TargetableObject;
            if (target != null) {
                ImpactData entityImpactData = entity.GetImpactData();
                ImpactData targetImpactData = target.GetImpactData();
                if (GetRelation(entityImpactData.Camp, targetImpactData.Camp) == RelationType.Friendly) {
                    return;
                }

                float entityDamageHP = CalcDamageHP(targetImpactData.Attack, entityImpactData.Defense);
                float targetDamageHP = CalcDamageHP(entityImpactData.Attack, targetImpactData.Defense);

                float delta = Mathf.Min(entityImpactData.HP - entityDamageHP, targetImpactData.HP - targetDamageHP);
                if (delta > 0) {
                    entityDamageHP += delta;
                    targetDamageHP += delta;
                }

                entity.ApplyDamage(target, entityDamageHP);
                target.ApplyDamage(entity, targetDamageHP);
                return;
            }

            /* if other entity is only a entity */
            Bullet bullet = other as Bullet;
            BulletData bulletData = userData as BulletData;
            if (bullet != null) {
                ImpactData entityImpactData = entity.GetImpactData();   // get tank impact data
                ImpactData bulletImpactData = bullet.GetImpactData();   // get bullet impact data

                // Block the damage of the same camp 自己发射的子弹，屏蔽其伤害
                //if (GetRelation(entityImpactData.Camp, bulletImpactData.Camp) == RelationType.Friendly) {
                //    return;
                //}

                int entityDamageHP = CalcDamageHP(bulletImpactData.Attack, entityImpactData.Defense);   // the bullet of attack value, and the tank of defense value.
                float bulletDamageHP = GetRelativeDistance(entity, bullet, bulletData) * entityDamageHP;

                //Debug.Log("entityDamageHP : " + bulletDamageHP);

                entity.ApplyDamage(bullet, bulletDamageHP);
                //GameEntry.Entity.HideEntity(bullet);
                return;
            }
        }

        // Calculate real damage
        // if defense = 0, the damageHP is the attack value.
        private static int CalcDamageHP(int attack, int defense) {
            if (attack <= 0) {
                return 0;
            }

            if (defense < 0) {
                defense = 0;
            }

            return attack * attack / (attack + defense);
        }

        private struct CampPair {
            private readonly CampType m_First;
            private readonly CampType m_Second;

            public CampPair(CampType first, CampType second) {
                m_First = first;
                m_Second = second;
            }

            public CampType First {
                get {
                    return m_First;
                }
            }

            public CampType Second {
                get {
                    return m_Second;
                }
            }
        }
    }
}