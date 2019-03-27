using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public class MyTank : Tank {

        [SerializeField]
        private MyTankData m_MyTankData = null;

        private CameraControlPro m_CameraControl;

        protected override void OnInit(object userData) {
            base.OnInit(userData);
        }

        protected override void OnShow(object userData) {
            base.OnShow(userData);

            m_MyTankData = userData as MyTankData;
            if (m_MyTankData == null) {
                Log.Error("My tank data is invalid.");
                return;
            }

            // 设置相机跟踪玩家自己
            m_CameraControl = GameObject.Find("Main Camera").AddComponent<CameraControlPro>();
            m_CameraControl.m_Target = this.transform;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}