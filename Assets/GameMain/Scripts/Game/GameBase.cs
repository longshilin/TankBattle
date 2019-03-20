//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace TankBattle {

    public abstract class GameBase {
        public CameraControlPro m_CameraControl;       // Reference to the CameraControl script for control during different phases.

        private const string SpawnPoint = "SpawnPoint";

        public abstract GameMode GameMode {
            get;
        }

        public bool GameOver {
            get;
            protected set;
        }

        private MyTank m_MyTank = null;

        // 初始化 游戏基础元素
        public virtual void Initialize() {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            // 实例化我的坦克(携带名称及出生点坐标)
            // 在指定点 生成tank
            GameEntry.Entity.ShowMyTank(new MyTankData(GameEntry.Entity.GenerateSerialId(), 10000) {
                Name = "My Tank",
                Position = GameObject.Find(SpawnPoint).transform.position,
                Rotation = GameObject.Find(SpawnPoint).transform.rotation,
            });

            GameOver = false;
            m_MyTank = null;

            m_CameraControl = GameObject.Find("Main Camera").AddComponent<CameraControlPro>();
        }

        // shutdown game
        public virtual void Shutdown() {
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        // Every frame will be executed when ProcedureMain calling Update Method;
        // please care,This method is not a method of Monobehavior, beacuse not override it.
        // function: change the end flag of the game when game over
        public virtual void Update(float elapseSeconds, float realElapseSeconds) {
            if (m_MyTank != null && m_MyTank.IsDead) {
                //Debug.Log("m_MyTank.IsDead : " + m_MyTank.IsDead);
                GameOver = true;
                return;
            }
        }

        // when show entity success, Instantiated tank entity and set tank camera target
        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e) {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(MyTank)) {
                //Debug.LogFormat(Constant.Logger.loggerFormat3, GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, "show tank entity success!");
                m_MyTank = (MyTank)ne.Entity.Logic;
                // 设置相机位置
                SetCameraTargets();
            }
        }

        // when show entity failure, print the error message
        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e) {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }

        private void SetCameraTargets() {
            m_CameraControl.m_Target = m_MyTank.transform;
        }
    }
}