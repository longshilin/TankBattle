using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// 可作为目标的实体类。
    /// </summary>
    public abstract class TargetableObject : Entity {

        [SerializeField]
        private TargetableObjectData m_TargetableObjectData = null;

        public bool IsDead {
            get {
                return m_TargetableObjectData.HP <= 0;
            }
        }

        public abstract ImpactData GetImpactData();

        public void ApplyDamage(Entity attacker, float damageHP) {
            float fromHPRatio = m_TargetableObjectData.HPRatio;
            m_TargetableObjectData.HP -= damageHP;
            float toHPRatio = m_TargetableObjectData.HPRatio;   // Percentage of MaxHP
            if (fromHPRatio > toHPRatio) {
                // if the percentage of MaxHP has reduced,then refresh HPBar on the tank entity
                GameEntry.HPBar.ShowHPBar(this, fromHPRatio, toHPRatio);
            }

            if (m_TargetableObjectData.HP <= 0) {
                // if the targetableObject HP <= 0, then the object is dead, hide the HPBar item.
                OnDead(attacker);
            }

            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
        }

        protected override void OnInit(object userData) {
            base.OnInit(userData);
            gameObject.SetLayerRecursively(Constant.Layer.TargetableObjectLayerId);

            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
        }

        protected override void OnShow(object userData) {
            base.OnShow(userData);
            // 复用Tank对象的时候将层级进行初始化
            gameObject.SetLayerRecursively(Constant.Layer.TargetableObjectLayerId);
            m_TargetableObjectData = userData as TargetableObjectData;
            if (m_TargetableObjectData == null) {
                Log.Error("Targetable object data is invalid.");
                return;
            }

            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
        }

        protected virtual void OnDead(Entity attacker) {
            GameEntry.Entity.HideEntity(this);

            //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
        }

        // Other rigid bodies touch the rigid body and trigger this OnTriggerEnter() method
        //private void OnCollisionEnter(Collision other) {
        //    Debug.Log("TargetableObject OnCollisionEnter - 1");
        //    // bullet entity
        //    Entity entity = other.gameObject.GetComponent<Entity>();
        //    if (entity == null) {
        //        return;
        //    }

        //    if (entity is TargetableObject && entity.Id >= Id) {
        //        // 碰撞事件由 Id 小的一方处理，避免重复处理
        //        return;
        //    }

        //    Debug.Log("TargetableObject OnCollisionEnter - 2");

        //    AIUtility.PerformCollision(this, entity);

        //    Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name);
        //}
    }
}