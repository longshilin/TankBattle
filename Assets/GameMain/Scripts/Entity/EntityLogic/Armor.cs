//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    /// <summary>
    /// 装甲类。
    /// </summary>
    public class Armor : Entity {
        private const string AttachPoint = "Armor Point";

        [SerializeField]
        private ArmorData m_ArmorData = null;

        protected override void OnInit(object userData) {
            base.OnInit(userData);
            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        protected override void OnShow(object userData) {
            base.OnShow(userData);

            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            m_ArmorData = userData as ArmorData;
            if (m_ArmorData == null) {
                Log.Error("Armor data is invalid.");
                return;
            }

            GameEntry.Entity.AttachEntity(Entity, m_ArmorData.OwnerId, AttachPoint);

            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, "Instantiation a Armor prefab and ArmorData");
        }

        // 将装甲类加载到坦克实体上
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData) {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

            Name = Utility.Text.Format("Armor of {0}", parentEntity.Name);
            CachedTransform.localPosition = Vector3.zero;

            //Debug.LogFormat(Constant.Logger.loggerFormat4, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, Name, "Attach a Armor instance to " + parentEntity.name);
        }
    }
}