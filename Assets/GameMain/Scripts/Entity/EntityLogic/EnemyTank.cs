using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class EnemyTank : Tank {

        [SerializeField]
        private EnemyTankData m_EnemyTankData = null;

        protected override void OnInit(object userData) {
            base.OnInit(userData);
        }

        protected override void OnShow(object userData) {
            base.OnShow(userData);

            m_EnemyTankData = userData as EnemyTankData;
            if (m_EnemyTankData == null) {
                Log.Error("Enemy tank data is invalid.");
                return;
            }

            string m_TankId = m_EnemyTankData.TankId;
            Debug.Log("Add Tank Id" + m_TankId);
            GameEntry.LockManager.AddActor(m_TankId, this);
            Debug.Log("!~!!!!!!! AddActor - " + m_TankId + " + " + this.Name);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}