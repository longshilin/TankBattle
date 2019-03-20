using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public abstract class Entity : EntityLogic {

        [SerializeField]
        private EntityData m_EntityData = null;

        /// <summary>
        /// 实体Id
        /// </summary>
        public int Id {
            get {
                return Entity.Id;
            }
        }

        /// <summary>
        /// 动画组件
        /// </summary>
        public Animation CachedAnimation {
            get;
            private set;
        }

        protected override void OnInit(object userData) {
            base.OnInit(userData);
            CachedAnimation = GetComponent<Animation>();
        }

        /// <summary>
        /// 根据传入的用户数据，进行实体变换
        /// </summary>
        protected override void OnShow(object userData) {
            base.OnShow(userData);

            m_EntityData = userData as EntityData;
            if (m_EntityData == null) {
                Log.Error("Entity data is invalid.");
                return;
            }

            Name = Utility.Text.Format("[Entity {0}]", Id.ToString());
            CachedTransform.localPosition = m_EntityData.Position;
            CachedTransform.localRotation = m_EntityData.Rotation;
            CachedTransform.localScale = Vector3.one;
        }

        protected override void OnHide(object userData) {
            base.OnHide(userData);
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData) {
            base.OnAttached(childEntity, parentTransform, userData);
        }

        protected override void OnDetached(EntityLogic childEntity, object userData) {
            base.OnDetached(childEntity, userData);
        }

        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData) {
            base.OnAttachTo(parentEntity, parentTransform, userData);
        }

        protected override void OnDetachFrom(EntityLogic parentEntity, object userData) {
            base.OnDetachFrom(parentEntity, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}