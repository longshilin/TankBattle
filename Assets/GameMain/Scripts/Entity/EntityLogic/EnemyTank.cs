using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class EnemyTank : Tank {

        [SerializeField]
        private MyTankData m_MyTankData = null;

        protected override void OnInit(object userData) {
            base.OnInit(userData);
        }

        protected override void OnShow(object userData) {
            base.OnShow(userData);

            m_MyTankData = userData as MyTankData;
            if (m_MyTankData == null) {
                Log.Error("Enemy tank data is invalid.");
                return;
            }
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}